Param()

$ErrorActionPreference = 'Stop'

Write-Host "Creating database (if not exists) and applying migrations..."
$projectPath = "$(Split-Path -Parent $PSScriptRoot)\project1\project1.csproj"

dotnet ef database update --project "project1" --startup-project "project1"

Write-Host "Done."