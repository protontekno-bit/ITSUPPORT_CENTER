@echo off
setlocal EnableDelayedExpansion
title Audit Kesehatan Fisik Disk S.M.A.R.T
color 0B

echo ==============================================================================
echo        🩺 AUDIT KESEHATAN FISIK DISK & S.M.A.R.T (SSD / HDD)
echo ==============================================================================
echo.

echo [1/3] Memeriksa status kesehatan dasar disk (WMI)...
wmic diskdrive get model,status,mediatype,size 2>nul
echo.

echo [2/3] Memeriksa S.M.A.R.T Predictive Failure...
powershell -Command "Get-CimInstance -Namespace root\wmi -ClassName MSStorageDriver_FailurePredictStatus | Select-Object InstanceName, PredictFailure, Reason"
echo.

echo [3/3] Memeriksa Status Operasional Drive Fisik...
powershell -Command "Get-PhysicalDisk | Select-Object DeviceId, FriendlyName, MediaType, HealthStatus, OperationalStatus | Format-Table -AutoSize"

echo.
echo ==============================================================================
echo  INFO: Jika status "PredictFailure = True" atau "Unhealthy", segera backup data!
echo ==============================================================================
pause
