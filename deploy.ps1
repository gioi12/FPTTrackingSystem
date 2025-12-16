# Script deploy backend lên GCP VM (PowerShell)
# Sử dụng: .\deploy.ps1

Write-Host "🚀 Bắt đầu deploy FPT Tracking System Backend..." -ForegroundColor Green

# Kiểm tra Docker
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Docker chưa được cài đặt. Vui lòng cài đặt Docker trước." -ForegroundColor Red
    exit 1
}

# Kiểm tra Docker Compose
if (-not (Get-Command docker-compose -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Docker Compose chưa được cài đặt. Vui lòng cài đặt Docker Compose trước." -ForegroundColor Red
    exit 1
}

# Dừng containers cũ nếu có
Write-Host "🛑 Dừng containers cũ..." -ForegroundColor Yellow
docker-compose down

# Build và chạy containers
Write-Host "🔨 Build và chạy containers..." -ForegroundColor Yellow
docker-compose up -d --build

# Kiểm tra trạng thái
Write-Host "⏳ Đợi containers khởi động..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# Khởi tạo database
Write-Host "🔧 Khởi tạo database..." -ForegroundColor Yellow
.\init-database.ps1

# Kiểm tra logs
Write-Host "📋 Logs của backend:" -ForegroundColor Cyan
docker-compose logs --tail=50 backend

Write-Host ""
Write-Host "✅ Deploy hoàn tất!" -ForegroundColor Green
Write-Host "🌐 Backend đang chạy tại: http://localhost:5000" -ForegroundColor Cyan
Write-Host "📚 Swagger UI: http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "🐰 RabbitMQ Management: http://localhost:15672 (guest/guest)" -ForegroundColor Cyan
Write-Host ""
Write-Host "Để xem logs: docker-compose logs -f backend" -ForegroundColor Yellow
Write-Host "Để dừng: docker-compose down" -ForegroundColor Yellow

