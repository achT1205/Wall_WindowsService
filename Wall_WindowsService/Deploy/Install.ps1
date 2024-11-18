$serviceName =  "EmailService"
$displayName =  "Wall_Windows Service (EmailService)"
$description =  "Un service pour rattraper les communications."

function Delete-ExistingService {
    Write-Host "Check if service exists"
    if (Get-Service $serviceName -ErrorAction SilentlyContinue)
    {
        Write-Host "Service exists."
        Write-Host "Stopping the existing service"
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

function Install-NewService {
    
    Write-Host "installing service"

    $secpasswd = ConvertTo-SecureString $Password -AsPlainText -Force
    #$mycreds = New-Object System.Management.Automation.PSCredential ($Loggin, $secpasswd) 


    $binaryPath = "C:\Users\achil\source\repos\Wall_WindowsService\Wall_WindowsService\bin\Debug\Wall_WindowsService.exe"
    New-Service -name $serviceName -binaryPathName $binaryPath -displayName $displayName -description $description -startupType Automatic -credential #$mycreds

    Write-Host "installation completed"
    Start-Sleep -seconds 2
    $integrationService = Get-Service -Name $ServiceName

    while ($integrationService.Status -ne 'Running')
    {
        Start-Service $ServiceName
        write-host $integrationService.status
        write-host 'Service starting'
        Start-Sleep -seconds 5
        $integrationService.Refresh()
        if ($integrationService.Status -eq 'Running')
        {
            Write-Host 'Service is now Running'
        }
    }
}



Delete-ExistingService 
$trial = 2
while((Get-Service $serviceName -ErrorAction SilentlyContinue) -and  $trial -gt 0)
{   
    Start-Sleep -seconds 2
    $emailService = Get-Service -Name $serviceName
    Write-Host 'The Service still exists after deletion process. Status:' + $emailService.status
    
    $trial--
    Write-Host "Try to delete the " + (3 - $trial) + " time"
    Delete-ExistingService
}

if (Get-Service $serviceName -ErrorAction SilentlyContinue){
    Write-Host "Can not delete the existing service after 3 trials"
    Write-Host "Aborting the installation" 
}else {
    Install-NewService
}