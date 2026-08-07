####################################################################
# Script to update the Jira Fix Build Number custom field for a release.
#
# This script is intentionally best-effort: it logs warnings and returns
# successfully for missing configuration, validation failures, and Jira errors.
####################################################################
[CmdletBinding()]
param (
	[Parameter(Mandatory=$true)]
	[string] $jiraUrl,
	[string] $buildNumber,
	[string] $headCommitMessage,
	[string] $fieldId,
	[string] $releaseRunUrl,
	[string] $email,
	[string] $apiToken,
	[switch] $dryRun,
	[switch] $diagnostic
)

$ErrorActionPreference = 'Stop'
$issueKey = $null

function Write-JiraOutcome([string] $IssueKey, [string] $Outcome) {
	Write-Warning "Jira Fix Build Number: issue=$IssueKey build=$buildNumber outcome=$Outcome release_run=$releaseRunUrl"
}

function Write-JiraDiagnostic([string] $Message) {
	if ($diagnostic) {
		Write-Host "Jira Fix Build Number diagnostic: $Message"
	}
}

try {
	Write-JiraDiagnostic "jira_url=$jiraUrl build=$buildNumber field_id=$fieldId release_run=$releaseRunUrl credentials_configured=$(-not [string]::IsNullOrWhiteSpace($email) -and -not [string]::IsNullOrWhiteSpace($apiToken)) dry_run=$dryRun"
	$issueKeys = @(
		[regex]::Matches($headCommitMessage ?? '', '(?i)\bVIX-\d+\b') |
			ForEach-Object { $_.Value.ToUpperInvariant() } |
			Sort-Object -Unique
	)

	if ($issueKeys.Count -ne 1) {
		Write-JiraOutcome ($issueKeys -join ',') 'skipped: expected exactly one distinct VIX issue key in the head commit message'
		return
	}

	$issueKey = $issueKeys[0]
	Write-JiraDiagnostic "normalized_issue_key=$issueKey"
	if ([string]::IsNullOrWhiteSpace($buildNumber)) {
		Write-JiraOutcome $issueKey 'skipped: missing build number from setup job'
		return
	}

	if ([string]::IsNullOrWhiteSpace($fieldId)) {
		Write-JiraOutcome $issueKey 'skipped: repository variable JIRA_FIX_BUILD_NUMBER_FIELD_ID is not configured'
		return
	}

	if ([string]::IsNullOrWhiteSpace($email) -or [string]::IsNullOrWhiteSpace($apiToken)) {
		Write-JiraOutcome $issueKey 'skipped: Jira credentials are not configured'
		return
	}

	$credentialBytes = [Text.Encoding]::UTF8.GetBytes("${email}:${apiToken}")
	$headers = @{
		Authorization = "Basic $([Convert]::ToBase64String($credentialBytes))"
		Accept = 'application/json'
		'Content-Type' = 'application/json'
	}
	$jiraBaseUrl = $jiraUrl.TrimEnd('/')

	$requestDescription = 'Jira field metadata request'
	$fieldMetadataUri = "$jiraBaseUrl/rest/api/3/field"
	Write-JiraDiagnostic "requesting Jira field metadata uri=$fieldMetadataUri"
	$allFields = Invoke-RestMethod -Method Get -Uri $fieldMetadataUri -Headers $headers -TimeoutSec 15
	$namedFields = @($allFields | Where-Object { $_.name -ceq 'Fix Build Number' })
	Write-JiraDiagnostic "exact_name_match_count=$($namedFields.Count)"
	if ($namedFields.Count -ne 1) {
		Write-JiraOutcome $issueKey 'skipped: Fix Build Number field is missing or ambiguous in Jira'
		return
	}

	$field = $namedFields[0]
	Write-JiraDiagnostic "discovered_field_id=$($field.id) discovered_field_type=$($field.schema.type)"
	if ($field.id -notmatch '^customfield_\d+$') {
		Write-JiraOutcome $issueKey "skipped: Fix Build Number is not a Jira custom field ('$($field.id)')"
		return
	}

	if ($field.id -ne $fieldId) {
		Write-JiraOutcome $issueKey "skipped: configured field ID does not match Jira field '$($field.id)'"
		return
	}

	if ($field.schema.type -ne 'number') {
		Write-JiraOutcome $issueKey "skipped: Fix Build Number has unsupported Jira value type '$($field.schema.type)'"
		return
	}

	$requestDescription = 'Jira edit metadata request'
	$editMetadataUri = "$jiraBaseUrl/rest/api/3/issue/$issueKey/editmeta"
	Write-JiraDiagnostic "requesting edit metadata for $issueKey uri=$editMetadataUri"
	$editMeta = Invoke-RestMethod -Method Get -Uri $editMetadataUri -Headers $headers -TimeoutSec 15
	if ($null -eq $editMeta.fields.$($field.id)) {
		Write-JiraOutcome $issueKey 'skipped: Fix Build Number is not editable for this issue'
		return
	}

	$requestDescription = 'Jira current field value request'
	$currentValueUri = "$jiraBaseUrl/rest/api/3/issue/${issueKey}?fields=$([uri]::EscapeDataString($field.id))"
	Write-JiraDiagnostic "requesting current value for $issueKey uri=$currentValueUri"
	$issue = Invoke-RestMethod -Method Get -Uri $currentValueUri -Headers $headers -TimeoutSec 15
	$currentValue = $issue.fields.$($field.id)
	Write-JiraDiagnostic "current_value_is_empty=$($null -eq $currentValue)"
	$numericBuildNumber = [long]$buildNumber

	if ($null -eq $currentValue) {
		if ($dryRun) {
			Write-JiraOutcome $issueKey 'dry run: would set Fix Build Number'
			return
		}

		$body = @{ fields = @{ $field.id = $numericBuildNumber } } | ConvertTo-Json -Compress
		$requestDescription = 'Jira Fix Build Number update request'
		Write-JiraDiagnostic "updating $issueKey uri=$jiraBaseUrl/rest/api/3/issue/$issueKey"
		Invoke-RestMethod -Method Put -Uri "$jiraBaseUrl/rest/api/3/issue/$issueKey" -Headers $headers -Body $body -TimeoutSec 15 | Out-Null
		Write-JiraOutcome $issueKey 'updated'
	} elseif ([long]$currentValue -eq $numericBuildNumber) {
		Write-JiraOutcome $issueKey 'success: field already has this build number'
	} else {
		Write-JiraOutcome $issueKey "skipped: field already contains '$currentValue'"
	}
} catch {
	$statusCode = $null
	if ($null -ne $_.Exception.Response) {
		$statusCode = [int] $_.Exception.Response.StatusCode
	}
	$statusText = if ($null -ne $statusCode) { ", HTTP $statusCode" } else { '' }
	Write-JiraOutcome ($issueKey ?? 'unknown') "skipped: $($requestDescription ?? 'Jira request or validation') failed ($($_.Exception.GetType().Name)$statusText)"
}
