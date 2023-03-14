
$linuxFrameworks = @(
    "netcoreapp3.1",
    "net5.0",
    "net6.0",
    "net7.0"
)

foreach ($framework in $linuxFrameworks) {

    $tag = $framework.Replace("netcoreapp", "").Replace("net","")

    $name = "reactiveinjection-generator-$($framework.Replace(".", '' ))"

    docker build -t $name `
        --platform "linux" `
        --build-arg "TAG=${tag}" `
        --build-arg "FRAMEWORK=${framework}" `
        -f (Join-Path $PSScriptRoot "Dockerfile") `
        (Join-Path $PSScriptRoot "..")

    Write-Host "Built Linux ${framework}" -ForegroundColor Cyan
}

foreach ($framework in $linuxFrameworks) {
    $name = "reactiveinjection-generator-$($framework.Replace(".", '' ))"

    docker run --platform "linux" --rm $name

    Write-Host "Tested Linux ${framework}" -ForegroundColor Cyan
}