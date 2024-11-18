$serviceName =  "EmailService"

function Delete-ExistingService {
    Write-Host "Check if service exists"
    if (Get-Service $serviceName -ErrorAction SilentlyContinue)
    {
        Write-Host "Service exists."
        Write-Host "stopping the existing service"
        Stop-Service -Name $serviceName
        Start-Sleep -seconds 2
        Write-Host "Deleting the existing service"
        $serviceToRemove = Get-WmiObject -Class Win32_Service -Filter "name='$serviceName'"
        $serviceToRemove.delete()
        Start-Sleep -seconds 2
        Write-Host "service removed"
    }
    else
    {
        Write-Host "service does not exists"
    }
}

Delete-ExistingService
$trial = 2
while((Get-Service $serviceName -ErrorAction SilentlyContinue) -and  $trial -gt 0)
{   $emailService = Get-Service -Name $serviceName
    Write-Host 'The Service still exists after deletion process. Status:' + $emailService.status
   
    $trial--
    Write-Host "Try to delete the " + (3 - $trial) + " time"
    Delete-ExistingService
   
}


if (Get-Service $serviceName -ErrorAction SilentlyContinue){
    Write-Host "Can not delete the existing service after 3 trials"
    Write-Host "Aborting the installation"
}else {
    "Uninstallation completed"
}