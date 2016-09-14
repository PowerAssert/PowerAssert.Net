dotnet test src\PowerAssertTests\
$exitcode = $LastExitCode
$testresult = Resolve-Path .\TestResult.xml
$wc = New-Object 'System.Net.WebClient'
$wc.UploadFile("https://ci.appveyor.com/api/testresults/nunit3/$($env:APPVEYOR_JOB_ID)", $testresult)
exit $exitcode