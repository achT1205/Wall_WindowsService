$serviceName = "EmailService"

function Write-Log {
    param([string]$message)
    $logFile = ".\UninstallLog.txt"
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Add-Content $logFile "$timestamp - $message"
    Write-Host $message
}

function Delete-ExistingService {
    Write-Log "Checking if service '$serviceName' exists..."
    if (Get-Service $serviceName -ErrorAction SilentlyContinue) {
        Write-Log "Service exists. Attempting to stop and delete it..."
        try {
            Stop-Service -Name $serviceName -Force
            Start-Sleep -Seconds 2
            Get-CimInstance -ClassName Win32_Service -Filter "Name='$serviceName'" | Remove-CimInstance
            Write-Log "Service '$serviceName' deleted successfully."
        } catch {
            Write-Log "ERROR: Failed to delete service '$serviceName'. Exception: $_"
            throw
        }
    } else {
        Write-Log "Service '$serviceName' does not exist."
    }
}

# Main script logic
try {
    Write-Log "Starting uninstallation process for service '$serviceName'."

    Delete-ExistingService

    $trial = 3
    while ((Get-Service $serviceName -ErrorAction SilentlyContinue) -and $trial -gt 0) {
        Start-Sleep -Seconds 2
        $trial--
        Write-Log "Retrying to delete the service ($trial attempts remaining)..."
        Delete-ExistingService
    }

    if (Get-Service $serviceName -ErrorAction SilentlyContinue) {
        Write-Log "ERROR: Could not delete the service after multiple attempts. Aborting uninstallation."
        throw "Service deletion failed."
    } else {
        Write-Log "Uninstallation process completed successfully."
    }
} catch {
    Write-Log "ERROR: Uninstallation process failed. Exception: $_"
    throw
}
