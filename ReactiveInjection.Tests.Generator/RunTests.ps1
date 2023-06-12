$targets = @(
    @{
        Name = "net31"
        Framework = "netcoreapp3.1"
        RunImage = "sdk:3.1"
    },
    @{
        Name = "net5"
        Framework = "net5.0"
        RunImage = "sdk:5.0"
    },
    @{
        Name = "net6"
        Framework = "net6.0"
        RunImage = "sdk:6.0"
    },
    @{
        Name = "net7"
        Framework = "net7.0"
        RunImage = "sdk:7.0"
    }
)

foreach ($target in $targets) {
    $name = "reactiveinjection-generator-$($target.Name)"

    docker build -t $name `
        --platform "linux" `
        --build-arg "BUILD_IMAGE=$($target.BuildImage)" `
        --build-arg "RUN_IMAGE=$($target.RunImage)" `
        --build-arg "FRAMEWORK=$($target.Framework)" `
        -f (Join-Path $PSScriptRoot "Dockerfile") `
        (Join-Path $PSScriptRoot "..")

    Write-Host "Built $($target.Name)" -ForegroundColor Cyan
}

foreach ($target in $targets) {
    $name = "reactiveinjection-generator-$($target.Name)"

    docker run --platform "linux" --rm $name

    Write-Host "Tested $($target.Name)" -ForegroundColor Cyan
}