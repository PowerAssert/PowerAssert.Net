$ReleaseVersionNumber = $env:APPVEYOR_BUILD_VERSION
$PreReleaseName = ''
$PullRequestNumber =$env:APPVEYOR_PULL_REQUEST_NUMBER
$BranchName = $env:APPVEYOR_REPO_BRANCH

If($ReleaseVersionNumber -eq $null){
    $ReleaseVersionNumber = '1.0.0'
}


If($PullRequestNumber -ne $null) {
  $PreReleaseName = '0-PR-' + $PullRequestNumber
} ElseIf($BranchName -ne 'master' -and $BranchName -ne $null) {
  $PreReleaseName = '-' + $BranchName
} Else {
  $PreReleaseName = '0'
}

$PSScriptFilePath = (Get-Item $MyInvocation.MyCommand.Path).FullName
$ScriptDir = Split-Path -Path $PSScriptFilePath -Parent
$SolutionRoot = Split-Path -Path $ScriptDir -Parent

$ProjectJsonPath = Join-Path -Path $SolutionRoot -ChildPath "src\PowerAssert\project.json"
$re = [regex]"(?<=`"version`":\s`")[.\w-]*(?=`",)"
$re.Replace([string]::Join("`r`n", (Get-Content -Path $ProjectJsonPath)), "$ReleaseVersionNumber.$PreReleaseName", 1) |
    Set-Content -Path $ProjectJsonPath -Encoding UTF8