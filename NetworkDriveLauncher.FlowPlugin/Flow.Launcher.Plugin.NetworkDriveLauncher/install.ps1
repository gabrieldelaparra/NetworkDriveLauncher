dotnet publish -c Release -r win-x64

$processName = "Flow.Launcher"

$myProcess = Get-Process $processName -ErrorAction SilentlyContinue
if ($myProcess) {
  Write-Host "  > Stop:" $myProcess.Name
  Stop-Process -Name $myProcess.Name -Force
}

Write-Host "  > Copy files..."
Copy-Item -Path '.\bin\Release\win-x64\publish\*' -Destination 'C:\Users\chgadel1\AppData\Roaming\FlowLauncher\Plugins\NetworkDriveLauncher-1.0.0\' -Recurse -Force

if($myProcess.HasExited)
{
  Write-Host "  > Start:" $myProcess.Name
  Start-Process -FilePath "C:\Users\chgadel1\AppData\Local\FlowLauncher\Flow.Launcher.exe"
}

if(!$myProcess){
  Write-Host "  > Start:" $processName
  Start-Process -FilePath "C:\Users\chgadel1\AppData\Local\FlowLauncher\Flow.Launcher.exe"
}
