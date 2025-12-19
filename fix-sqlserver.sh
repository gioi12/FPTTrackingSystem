#!/bin/bash

# Script để sửa lỗi SQL Server container

echo "🔧 Đang sửa lỗi SQL Server..."

# Dừng containers
echo "🛑 Dừng containers..."
docker-compose down

# Xóa volume SQL Server nếu có vấn đề (⚠️ Mất dữ liệu!)
read -p "Bạn có muốn xóa volume SQL Server? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "🗑️ Đang xóa volume SQL Server..."
    docker volume rm fpttrackingsystem_sqlserver_data 2>/dev/null || echo "Volume không tồn tại"
fi

# Khởi động lại chỉ SQL Server trước
echo "🚀 Khởi động SQL Server..."
docker-compose up -d sqlserver

# Đợi SQL Server khởi động
echo "⏳ Đợi SQL Server khởi động (có thể mất 1-2 phút)..."
sleep 30

# Kiểm tra logs
echo "📋 Logs SQL Server:"
docker-compose logs --tail 30 sqlserver

# Kiểm tra health
echo ""
echo "💚 Kiểm tra health:"
for i in {1..30}; do
    HEALTH=$(docker inspect --format='{{.State.Health.Status}}' fpt-tracking-sqlserver 2>/dev/null)
    echo "Attempt $i: $HEALTH"
    if [ "$HEALTH" = "healthy" ]; then
        echo "✅ SQL Server đã healthy!"
        break
    fi
    sleep 2
done

# Nếu vẫn không healthy, thử kết nối trực tiếp
echo ""
echo "🔧 Thử kết nối trực tiếp:"
docker exec fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P "YourStrong@Password123" \
    -C \
    -Q "SELECT 1" 2>&1

echo ""
echo "✅ Hoàn tất! Nếu SQL Server đã healthy, chạy:"
echo "   docker-compose up -d"

