####################################################################
# Script to generate Jira Release Notes.
#
# Example:
#   .\jira-release-notes.ps1 -jiraUrl 'http://jira.some.com' -project 'Potatoes'
#   .\jira-release-notes.ps1 -jiraUrl 'http://jira.some.com' -fixVersion '1.2.0'
#   .\jira-release-notes.ps1 -jiraUrl 'http://jira.some.com' -statuses @('Done')
#
####################################################################
param (
    [Parameter(Mandatory=$true)]
    [string] $jiraUrl,
    [string[]] $statuses,
    [string] $project,
    [string] $fixVersion,
	[string] $buildType
)

$nl = [Environment]::NewLine

if($fixVersion.EndsWith('u0')){
	$fixVersion = $fixVersion -replace ".{2}$"
}

# Trim the Jira URL for trailing slashes
if ($jiraUrl.EndsWith('/'))
{
    $jiraUrl = $jiraUrl.Trim('/')
}

# Set the main Rest API and Jira endpoints
$jiraRestApi = "$jiraUrl/rest/api/latest/search"
$jiraBrowse = "$jiraUrl/browse"

# Construct the JQL
$jql = [string]::Empty
$jql += "Project='$project' AND "
$jql += "fixVersion='$fixVersion' AND "

if ($statuses -ne $null -and $statuses.Length -gt 0)
{
    $jql += ("Status IN ('{0}')" -f ($statuses -join "','"))
}

if ($jql.EndsWith('AND '))
{
    $jql = $jql.Trim('AND ')
}

$jql += " ORDER BY Key"

$jql = $jql.Trim() -replace ' ', '%20'

# Invoke the Rest API for the constructed JQL
$jiraSearchUri = ("{0}?jql=$jql&startAt=0&maxResult=1000" -f $jiraRestApi)

Write-Host $jiraSearchUri

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$result = Invoke-RestMethod -Method Get -Uri $jiraSearchUri -ContentType 'application/json'

if (!$?)
{
    Write-Error 'The call to the JIRA REST API was not successful.'
    return
}

# Gather up all of the issues
$issues = $result.issues

$issueMap = New-Object System.Collections.Specialized.OrderedDictionary
foreach ($issue in $issues)
{
    $code = $issue.key
    $type = $issue.fields.issueType.name
    $summary = $issue.fields.summary

    if (!$issueMap.Contains($type))
    {
		$list = New-Object System.Collections.Specialized.OrderedDictionary
        $issueMap.Add($type, $list)
    }

    $issueMap[$type].Add($code, $summary)
}

# Generate the output
$output = [string]::Empty

$output += "Release Notes - Vixen 3 - " + $buildType + " Build "
$output += "$nl$nl"


foreach ($type in $issueMap.keys)
{
	$output += "** $type$nl"

	foreach ($issue in $issueMap[$type].keys)
	{
		$summary = $issueMap[$type][$issue]
		$output += "    * [$issue] - $summary$nl"
	}

	$output += "$nl"
}

# Store the release notes as Github Actions output to use in release body
$actionsOutput = $output.Trim()
# Transform into markdown
$actionsOutput = $actionsOutput -replace '\A', '## '
$actionsOutput = $actionsOutput -replace '(?m)^\*\* ', '### '
$actionsOutput = $actionsOutput -replace '(?m)^    \* ', '* '

$file = 'Release Notes.md'
Out-File -FilePath $file -InputObject $actionsOutput -Encoding UTF8

$file = './Release Notes.txt'
$regex = '^Release Notes - Vixen 3$'
(Get-Content $file) -replace $regex, $output | Set-Content $file
