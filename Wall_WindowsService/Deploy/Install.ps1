$serviceName = "EmailService"
$displayName = "Wall_Windows Service (EmailService)"
$description = "Un service pour rattraper les communications."
$binaryPath = "D:\EDF WALL-E\WALLSDIN_WINDOWS_SERVICE\Wall_WindowsService.exe"

function Write-Log {
    param([string]$message)
    $logFile = ".\InstallLog.txt"
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Add-Content $logFile "$timestamp - $message"
    Write-Host $message
}

function Validate-BinaryPath {
    if (-Not (Test-Path $binaryPath)) {
        Write-Log "ERROR: Binary file not found at path: $binaryPath"
        throw "Binary file not found at path: $binaryPath"
    }
}

function Delete-ExistingService {
    Write-Log "Checking if service exists..."
    if (Get-Service $serviceName -ErrorAction SilentlyContinue) {
        Write-Log "Service exists. Stopping and deleting it..."
        try {
            Stop-Service -Name $serviceName -Force
            Start-Sleep -Seconds 2
            Get-CimInstance -ClassName Win32_Service -Filter "Name='$serviceName'" | Remove-CimInstance
            Write-Log "Service removed successfully."
        } catch {
            Write-Log "ERROR: Failed to delete service. Exception: $_"
            throw
        }
    } else {
        Write-Log "Service does not exist."
    }
}

function Install-NewService {
    Write-Log "Installing service..."
    try {
        New-Service -Name $serviceName -BinaryPathName $binaryPath -DisplayName $displayName -Description $description -StartupType Automatic
        Write-Log "Service installed successfully."
        Start-Sleep -Seconds 2

        $integrationService = Get-Service -Name $serviceName
        $timeout = 30
        while ($integrationService.Status -ne 'Running' -and $timeout -gt 0) {
            Start-Service $serviceName
            Write-Log "Waiting for the service to start. Status: $($integrationService.Status)"
            Start-Sleep -Seconds 5
            $integrationService.Refresh()
            $timeout -= 5
        }

        if ($integrationService.Status -eq 'Running') {
            Write-Log "Service is now running."
        } else {
            Write-Log "ERROR: Service failed to start within the timeout period."
            throw "Service failed to start."
        }
    } catch {
        Write-Log "ERROR: Failed to install or start the service. Exception: $_"
        throw
    }
}

# Main Script Execution
try {
    Write-Log "Starting installation process for $serviceName."
    Validate-BinaryPath

    Delete-ExistingService
    $trial = 3

    while ((Get-Service $serviceName -ErrorAction SilentlyContinue) -and $trial -gt 0) {
        Start-Sleep -Seconds 2
        $trial--
        Write-Log "Retrying to delete the service ($trial attempts remaining)..."
        Delete-ExistingService
    }

    if (Get-Service $serviceName -ErrorAction SilentlyContinue) {
        Write-Log "ERROR: Could not delete the service after multiple attempts. Aborting installation."
        throw "Service deletion failed."
    }

    Install-NewService
    Write-Log "Installation process completed successfully."
} catch {
    Write-Log "ERROR: Installation process failed. Exception: $_"
    throw
}
