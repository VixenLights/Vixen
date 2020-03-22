param (
    [Parameter(Mandatory=$true)]
    [string] $version,
	[string] $user,
	[string] $repo,
	[string] $buildType,
	[string] $token
)

if($version.EndsWith('u0')){
	$version = $version -replace ".{2}$"
}

$testUrl = "http://www.vixenlights.com/downloads/devbuild/"
$releaseUrl = "http://www.vixenlights.com/downloads/vixen-3-downloads/"

$status = git tag -l $version

Write-Host $status

if([string]::IsNullOrEmpty($status)){
	Write-Host "Creating tag $($version)"
	git tag $version
	git push -f --tags
}

$description = "Test builds can be downloaded here: <br/>"
$command = '.\Build\github-release release -s $token -u $user -r $repo -t $version -d $description'

if($buildType -ieq "dev"){
	$description += $testUrl
	$command += ' -p'
}
else
{
	$description += $releaseUrl
}

Write-Host "Testing to see if release exists."

$status = .\Build\github-release info -s $token -u $user -r $repo -t $version

if(-Not[string]::IsNullOrEmpty($status) -And ($status.StartsWith("error: could not find the release corresponding to tag"))){
	Write-Host "Release Exists: Attempting to delete it."
	.\Build\github-release delete -s $token -u $user -r $repo -t $version
}

Write-Host "Creating release with description $($description) for version $($version)"
Invoke-Expression -Command $command


