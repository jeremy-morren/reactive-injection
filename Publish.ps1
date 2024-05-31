$artifacts = Join-Path $PSScriptRoot "Artifacts"

function Get-Version()
{
    #Note: Packages sort lexically
    $last = Get-ChildItem (Join-Path $artifacts "ReactiveInjection.SourceGenerators.*.nupkg") `
        | Sort-Object -Property Name -Descending `
        | Select-Object -First 1

    if ($null -eq $last) {
        [Version]::New(0, 1, 0)
        return
    }

    $fileName = $last.Name
    $v = [Version]::New($fileName.Substring("ReactiveInjection.SourceGenerators.".Length, "0.0.0".Length))
    [Version]::New($v.Major, $v.Minor, $v.Build + 1)
}

$version = (Get-Version).ToString()

Write-Host "Packing ReactiveInjection ${version}" -ForegroundColor Cyan

@("ReactiveInjection", "ReactiveInjection.SourceGenerators") | ForEach-Object {
    dotnet pack (Join-Path $PSScriptRoot "${_}/${_}.csproj") `
        -c Release "-p:Version=${version}" `
        -o $artifacts

    Copy-Item -Path (Join-Path $artifacts "${_}.${version}.nupkg") -Destination (Join-Path $artifacts "${_}.${version}.zip")
}

$version #Return version
