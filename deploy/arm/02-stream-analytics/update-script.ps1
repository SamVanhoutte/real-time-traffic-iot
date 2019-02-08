#This script updates the arm template to include the actual query from the Stream Analytics project
$pathToJson = "cloud-template.json"
$template = Get-Content $pathToJson -Raw | ConvertFrom-Json
$query = Get-Content "..\..\..\src\cloud\StreamAnalyticsCloud\Script.asaql" -Raw | Out-String
$template.resources[0].properties.transformation.properties.query = $query
$templateContent = $template | ConvertTo-Json -Depth 10 | % { [System.Text.RegularExpressions.Regex]::Unescape($_) }
$templateContent | set-content $pathToJson
