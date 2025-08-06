####################################################################
# Script to generate Jira Release Notes.
#
# Example:
#   .\Build\CreateReleaseNotes.ps1 -jiraUrl 'https://bugs.vixenlights.com' -project 'Vixen 3' -fixVersion 'DevBuild' -buildType 'Development'
#   .\Build\CreateReleaseNotes.ps1 -jiraUrl 'https://bugs.vixenlights.com' -project 'Vixen 3' -fixVersion 'DevBuild' -buildType '3.10u0'
#
#   Release Notes.txt must be in the folder this script is run from.
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
$jiraRestApi = "$jiraUrl/rest/api/latest/search/jql"

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

$nextPageToken=""

[bool] $moreRecords = 1

$issueMap = New-Object System.Collections.Specialized.OrderedDictionary
$issueCount = 0

while($moreRecords)
{
	# Invoke the Rest API for the constructed JQL
	$jiraSearchUri = ("{0}?jql=$jql&maxResults=100&fieldsByKeys=true&fields=summary,issuetype,key&nextPageToken={1}" -f $jiraRestApi, $nextPageToken)

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
	$isLast = $result.isLast
	$issueCount += $issues.Count
	
	foreach ($issue in $issues)
	{
		$code = $issue.key
		$type = $issue.fields.issuetype.name
		$summary = $issue.fields.summary
		if (!$issueMap.Contains($type))
		{
			$list = New-Object System.Collections.Specialized.OrderedDictionary
			$issueMap.Add($type, $list)
		}

		$issueMap[$type].Add($code, $summary)
	}

	if ($isLast -ne "True")
	{
		$nextPageToken = $result.nextPageToken
	}
	else
	{
		$moreRecords = 0
	}
	
}

# Generate the output
$output = [string]::Empty

$output += "Release Notes - Vixen 3 - " + $buildType + " Build "
$output += "$nl$nl"

#Append the count of issues
$output += "** $issueCount issues resolved in this release. $nl$nl"

if($buildType -eq "Development"){
	$output += "Note: The issues listed as part of this development build are cumulative since the last full release.$nl$nl"
}

foreach ($type in $issueMap.keys)
{
	$output += "** ${type}s$nl"

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
