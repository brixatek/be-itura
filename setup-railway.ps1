$env:RAILWAY_PROJECT_ID = "8e2c29c0-6d86-41b5-9dc1-4b6acacc51ae"

$services = @(
    "gateway",
    "auth",
    "user",
    "mood",
    "journal",
    "coach",
    "booking",
    "payment",
    "notification",
    "community",
    "content",
    "media",
    "corporate",
    "gamification",
    "analytics",
    "search",
    "ai"
)

foreach ($svc in $services) {
    Write-Host "Creating service: $svc..."
    railway service create $svc
    Write-Host "Done: $svc"
}

Write-Host "All 17 services created."
