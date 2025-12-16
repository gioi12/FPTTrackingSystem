# Script để sửa file SQL cho Docker (PowerShell)
# Loại bỏ đường dẫn Windows cụ thể trong CREATE DATABASE

param(
    [string]$InputFile = "../FptTrackingSystem_FiNAL.sql",
    [string]$OutputFile = "../FptTrackingSystem_FiNAL_Docker.sql"
)

Write-Host "🔧 Đang sửa file SQL cho Docker..." -ForegroundColor Yellow

if (-not (Test-Path $InputFile)) {
    Write-Host "❌ Không tìm thấy file: $InputFile" -ForegroundColor Red
    exit 1
}

# Đọc file và thay thế đường dẫn Windows
$content = Get-Content $InputFile -Raw

# Thay thế đường dẫn mdf
$content = $content -replace "FILENAME = N'C:\\Program Files\\Microsoft SQL Server\\MSSQL15\\.SQLEXPRESS\\MSSQL\\DATA\\FPTTrackingSystem\.mdf'", "FILENAME = N'/var/opt/mssql/data/FPTTrackingSystem.mdf'"

# Thay thế đường dẫn ldf
$content = $content -replace "FILENAME = N'C:\\Program Files\\Microsoft SQL Server\\MSSQL15\\.SQLEXPRESS\\MSSQL\\DATA\\FPTTrackingSystem_log\.ldf'", "FILENAME = N'/var/opt/mssql/data/FPTTrackingSystem_log.ldf'"

# Ghi file mới
$content | Out-File -FilePath $OutputFile -Encoding UTF8

Write-Host "✅ Đã tạo file SQL cho Docker: $OutputFile" -ForegroundColor Green

