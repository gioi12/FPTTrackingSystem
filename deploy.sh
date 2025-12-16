#!/bin/bash

# Script deploy backend lên GCP VM
# Sử dụng: ./deploy.sh

echo "🚀 Bắt đầu deploy FPT Tracking System Backend..."

# Kiểm tra Docker
if ! command -v docker &> /dev/null; then
    echo "❌ Docker chưa được cài đặt. Vui lòng cài đặt Docker trước."
    exit 1
fi

# Kiểm tra Docker Compose
if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose chưa được cài đặt. Vui lòng cài đặt Docker Compose trước."
    exit 1
fi

# Dừng containers cũ nếu có
echo "🛑 Dừng containers cũ..."
docker-compose down

# Build và chạy containers
echo "🔨 Build và chạy containers..."
docker-compose up -d --build

# Kiểm tra trạng thái
echo "⏳ Đợi containers khởi động..."
sleep 15

# Khởi tạo database
echo "🔧 Khởi tạo database..."
chmod +x init-database.sh
chmod +x scripts/prepare-sql.sh 2>/dev/null || true
./init-database.sh

# Kiểm tra logs
echo "📋 Logs của backend:"
docker-compose logs --tail=50 backend

echo ""
echo "✅ Deploy hoàn tất!"
echo "🌐 Backend đang chạy tại: http://localhost:5000"
echo "📚 Swagger UI: http://localhost:5000/swagger"
echo "🐰 RabbitMQ Management: http://localhost:15672 (guest/guest)"
echo ""
echo "Để xem logs: docker-compose logs -f backend"
echo "Để dừng: docker-compose down"

