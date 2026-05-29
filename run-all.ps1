# run-all.ps1 - Start all Itura services locally
# Usage:
#   .\run-all.ps1                  # start infrastructure + run migrations + launch all services
#   .\run-all.ps1 -SkipMigrations  # skip migrations (faster on subsequent runs)
#   .\run-all.ps1 -SkipInfra       # skip docker compose up (already running)

param(
    [switch]$SkipMigrations,
    [switch]$SkipInfra
)

$Root = $PSScriptRoot
$ServicesPath = Join-Path $Root "src\Services"

# Service definitions
$services = @(
    @{ Name="Auth";         Dir="Auth";         Port=5197 }
    @{ Name="User";         Dir="User";         Port=5208 }
    @{ Name="AI";           Dir="AI";           Port=5193 }
    @{ Name="Mood";         Dir="Mood";         Port=5160 }
    @{ Name="Journal";      Dir="Journal";      Port=5170 }
    @{ Name="Coach";        Dir="Coach";        Port=5136 }
    @{ Name="Booking";      Dir="Booking";      Port=5003 }
    @{ Name="Payment";      Dir="Payment";      Port=5254 }
    @{ Name="Notification"; Dir="Notification"; Port=5239 }
    @{ Name="Community";    Dir="Community";    Port=5149 }
    @{ Name="Content";      Dir="Content";      Port=5053 }
    @{ Name="Media";        Dir="Media";        Port=5098 }
    @{ Name="Corporate";    Dir="Corporate";    Port=5289 }
    @{ Name="Gamification"; Dir="Gamification"; Port=5041 }
    @{ Name="Analytics";    Dir="Analytics";    Port=5110 }
    @{ Name="Search";       Dir="Search";       Port=5192 }
)
# Gateway has no Infrastructure/migrations
$gateway = @{ Name="Gateway"; ApiPath="src\Services\Gateway\Itura.Gateway"; Port=5055 }

# -----------------------------------------------------------------------
# Step 1: Start infrastructure
# -----------------------------------------------------------------------
if (-not $SkipInfra) {
    Write-Host ""
    Write-Host "[1/3] Starting infrastructure (postgres, redis, rabbitmq, mongodb)..." -ForegroundColor Cyan
    Set-Location $Root
    docker compose up -d
    if ($LASTEXITCODE -ne 0) { Write-Error "docker compose failed"; exit 1 }

    Write-Host "      Waiting for postgres to be healthy..." -ForegroundColor Gray
    $attempts = 0
    do {
        Start-Sleep -Seconds 3
        $health = docker inspect itura-postgres --format "{{.State.Health.Status}}" 2>$null
        $attempts++
    } while ($health -ne "healthy" -and $attempts -lt 20)

    if ($health -ne "healthy") {
        Write-Error "Postgres did not become healthy after 60s. Run: docker compose logs postgres"
        exit 1
    }
    Write-Host "      Infrastructure ready." -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "[1/3] Skipping infrastructure start (-SkipInfra)." -ForegroundColor Yellow
}

# -----------------------------------------------------------------------
# Step 2: Run migrations
# -----------------------------------------------------------------------
if (-not $SkipMigrations) {
    Write-Host ""
    Write-Host "[2/3] Running EF Core migrations for all services in parallel..." -ForegroundColor Cyan

    $jobs = @()
    foreach ($svc in $services) {
        $infraPath = Join-Path $ServicesPath "$($svc.Dir)\Itura.$($svc.Name).Infrastructure"
        $apiPath   = Join-Path $ServicesPath "$($svc.Dir)\Itura.$($svc.Name).API"

        if (-not (Test-Path $infraPath)) {
            Write-Host "      [SKIP] $($svc.Name) - no Infrastructure project found" -ForegroundColor Yellow
            continue
        }

        $jobs += Start-Job -Name "migrate-$($svc.Name)" -ScriptBlock {
            param($infra, $api, $name)
            Set-Location $infra
            $output = dotnet ef database update --startup-project $api 2>&1
            if ($LASTEXITCODE -ne 0) {
                return @{ Name=$name; Success=$false; Output=$output }
            }
            return @{ Name=$name; Success=$true; Output=$output }
        } -ArgumentList $infraPath, $apiPath, $svc.Name
    }

    Write-Host "      Migrations running for $($jobs.Count) services..." -ForegroundColor Gray
    $results = $jobs | Wait-Job | Receive-Job
    $jobs | Remove-Job

    $failed = @()
    foreach ($r in $results) {
        if ($r.Success) {
            Write-Host "      [OK]   $($r.Name)" -ForegroundColor Green
        } else {
            Write-Host "      [FAIL] $($r.Name)" -ForegroundColor Red
            $failed += $r.Name
        }
    }

    if ($failed.Count -gt 0) {
        Write-Warning "Migrations failed for: $($failed -join ', '). Those services may not start correctly."
        $continue = Read-Host "Continue anyway? (y/n)"
        if ($continue -ne 'y') { exit 1 }
    }
} else {
    Write-Host ""
    Write-Host "[2/3] Skipping migrations (-SkipMigrations)." -ForegroundColor Yellow
}

# -----------------------------------------------------------------------
# Step 3: Launch all services in separate windows
# -----------------------------------------------------------------------
Write-Host ""
Write-Host "[3/3] Launching all services..." -ForegroundColor Cyan

foreach ($svc in $services) {
    $apiPath = Join-Path $ServicesPath "$($svc.Dir)\Itura.$($svc.Name).API"
    $title   = "Itura :: $($svc.Name) :$($svc.Port)"
    $cmd     = "`$host.UI.RawUI.WindowTitle = '$title'; Write-Host '--- $title ---' -ForegroundColor Cyan; Set-Location '$apiPath'; dotnet run --launch-profile http"

    Start-Process powershell -ArgumentList "-NoExit", "-Command", $cmd
    Write-Host "      Started $($svc.Name) -> http://localhost:$($svc.Port)/swagger" -ForegroundColor Green
    Start-Sleep -Milliseconds 300
}

# Gateway
$gwPath  = Join-Path $Root $gateway.ApiPath
$gwTitle = "Itura :: Gateway :$($gateway.Port)"
$gwCmd   = "`$host.UI.RawUI.WindowTitle = '$gwTitle'; Write-Host '--- $gwTitle ---' -ForegroundColor Cyan; Set-Location '$gwPath'; dotnet run --launch-profile http"
Start-Process powershell -ArgumentList "-NoExit", "-Command", $gwCmd
Write-Host "      Started Gateway   -> http://localhost:$($gateway.Port)" -ForegroundColor Green

# -----------------------------------------------------------------------
# Summary
# -----------------------------------------------------------------------
Write-Host ""
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "  All services launching. Give them ~20s to start up." -ForegroundColor Cyan
Write-Host ""
Write-Host "  Service         Swagger URL" -ForegroundColor White
Write-Host "  -------------------------------------------------------" -ForegroundColor Gray
foreach ($svc in $services) {
    Write-Host ("  {0,-15} http://localhost:{1}/swagger" -f $svc.Name, $svc.Port)
}
Write-Host ("  {0,-15} http://localhost:{1} (no Swagger)" -f "Gateway", $gateway.Port)
Write-Host ""
Write-Host "  RabbitMQ UI  ->  http://localhost:15672  (guest/guest)" -ForegroundColor Gray
Write-Host "=======================================================" -ForegroundColor Cyan
