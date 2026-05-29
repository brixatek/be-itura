# One-time Railway project setup
# Prerequisites:
#   1. npm install -g @railway/cli
#   2. railway login
#   3. Set RAILWAY_PROJECT_ID below after creating a blank project at railway.app

$PROJECT_ID = "8e2c29c0-6d86-41b5-9dc1-4b6acacc51ae"

$SERVICES = @(
    @{ name="gateway";       dir="Auth";         assembly="Itura.Gateway.dll";         dockerfile="deploy/Dockerfile.gateway" },
    @{ name="auth";          dir="Auth";         assembly="Itura.Auth.API.dll";         dockerfile="deploy/Dockerfile" },
    @{ name="user";          dir="User";         assembly="Itura.User.API.dll";         dockerfile="deploy/Dockerfile" },
    @{ name="mood";          dir="Mood";         assembly="Itura.Mood.API.dll";         dockerfile="deploy/Dockerfile" },
    @{ name="journal";       dir="Journal";      assembly="Itura.Journal.API.dll";      dockerfile="deploy/Dockerfile" },
    @{ name="coach";         dir="Coach";        assembly="Itura.Coach.API.dll";        dockerfile="deploy/Dockerfile" },
    @{ name="booking";       dir="Booking";      assembly="Itura.Booking.API.dll";      dockerfile="deploy/Dockerfile" },
    @{ name="payment";       dir="Payment";      assembly="Itura.Payment.API.dll";      dockerfile="deploy/Dockerfile" },
    @{ name="notification";  dir="Notification"; assembly="Itura.Notification.API.dll"; dockerfile="deploy/Dockerfile" },
    @{ name="community";     dir="Community";    assembly="Itura.Community.API.dll";    dockerfile="deploy/Dockerfile" },
    @{ name="content";       dir="Content";      assembly="Itura.Content.API.dll";      dockerfile="deploy/Dockerfile" },
    @{ name="media";         dir="Media";        assembly="Itura.Media.API.dll";        dockerfile="deploy/Dockerfile" },
    @{ name="corporate";     dir="Corporate";    assembly="Itura.Corporate.API.dll";    dockerfile="deploy/Dockerfile" },
    @{ name="gamification";  dir="Gamification"; assembly="Itura.Gamification.API.dll"; dockerfile="deploy/Dockerfile" },
    @{ name="analytics";     dir="Analytics";    assembly="Itura.Analytics.API.dll";    dockerfile="deploy/Dockerfile" },
    @{ name="search";        dir="Search";       assembly="Itura.Search.API.dll";       dockerfile="deploy/Dockerfile" },
    @{ name="ai";            dir="AI";           assembly="Itura.AI.API.dll";           dockerfile="deploy/Dockerfile" }
)

$env:RAILWAY_PROJECT_ID = $PROJECT_ID

foreach ($svc in $SERVICES) {
    Write-Host "Creating service: $($svc.name)..."

    # Create the service
    railway service create $svc.name

    # Set build variables for services using the shared Dockerfile
    if ($svc.dockerfile -eq "deploy/Dockerfile") {
        railway variables set `
            --service $svc.name `
            SERVICE_DIR=$($svc.dir) `
            SERVICE_NAME=$($svc.dir) `
            ASSEMBLY=$($svc.assembly)
    }

    # Set the Dockerfile path
    railway variables set --service $svc.name RAILWAY_DOCKERFILE_PATH=$($svc.dockerfile)

    # Set shared env vars for all services
    railway variables set --service $svc.name `
        ASPNETCORE_ENVIRONMENT=Production `
        ASPNETCORE_URLS="http://+:8080" `
        ConnectionStrings__DefaultConnection="Host=gondola.proxy.rlwy.net;Port=37181;Database=railway;Username=postgres;Password=iRjeiDgqViuqVDRyVboZIUZeWVsYwrEd"

    Write-Host "✓ $($svc.name) created"
}

Write-Host ""
Write-Host "All services created. Now:"
Write-Host "1. Go to Railway dashboard and connect each service to your GitHub repo"
Write-Host "2. Add RAILWAY_TOKEN to GitHub secrets for CI/CD"
