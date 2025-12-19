#!/bin/bash

# Script để kiểm tra và debug SQL Server container

echo "🔍 Kiểm tra SQL Server container..."

# Kiểm tra container có chạy không
echo "📊 Trạng thái container:"
docker ps -a | grep fpt-tracking-sqlserver

echo ""
echo "📋 Logs SQL Server (50 dòng cuối):"
docker logs --tail 50 fpt-tracking-sqlserver

echo ""
echo "💚 Healthcheck status:"
docker inspect --format='{{json .State.Health}}' fpt-tracking-sqlserver 2>/dev/null | python3 -m json.tool 2>/dev/null || docker inspect --format='{{json .State.Health}}' fpt-tracking-sqlserver

echo ""
echo "🔧 Thử kết nối SQL Server:"
docker exec fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P "YourStrong@Password123" \
    -C \
    -Q "SELECT @@VERSION" 2>&1

echo ""
echo "💡 Nếu SQL Server chưa sẵn sàng, đợi thêm vài phút rồi thử lại:"
echo "   docker-compose logs -f sqlserver"

