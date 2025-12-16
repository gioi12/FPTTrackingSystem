#!/bin/bash

# Script để verify ports và kết nối

echo "🔍 Kiểm tra containers và ports..."

echo ""
echo "📊 Containers đang chạy:"
docker ps --format "table {{.Names}}\t{{.Image}}\t{{.Ports}}"

echo ""
echo "🔌 Kiểm tra SQL Server (sqlpreview):"
if docker ps | grep -q sqlpreview; then
    echo "✅ Container sqlpreview đang chạy"
    echo "   Port mapping: $(docker port sqlpreview 2>/dev/null | grep 1433 || echo '1433:1433')"
    
    # Test kết nối
    echo "   Testing connection..."
    docker exec sqlpreview /opt/mssql-tools/bin/sqlcmd \
        -S localhost -U sa -P "StrongP@ssw0rd!" -C -Q "SELECT @@VERSION" 2>&1 | head -1
else
    echo "❌ Container sqlpreview không chạy"
fi

echo ""
echo "🐰 Kiểm tra RabbitMQ (fpt-tracking-rabbitmq):"
if docker ps | grep -q fpt-tracking-rabbitmq; then
    echo "✅ Container fpt-tracking-rabbitmq đang chạy"
    echo "   Port mapping: $(docker port fpt-tracking-rabbitmq 2>/dev/null | grep 5672 || echo '5672:5672')"
    echo "   Management UI: $(docker port fpt-tracking-rabbitmq 2>/dev/null | grep 15672 || echo '15672:15672')"
else
    echo "❌ Container fpt-tracking-rabbitmq không chạy"
fi

echo ""
echo "🚀 Kiểm tra Backend (fpt-tracking-backend):"
if docker ps | grep -q fpt-tracking-backend; then
    echo "✅ Container fpt-tracking-backend đang chạy"
    echo "   Port mapping: $(docker port fpt-tracking-backend 2>/dev/null | grep 5000 || echo '5000:5000')"
else
    echo "ℹ️ Container fpt-tracking-backend chưa chạy"
fi

echo ""
echo "🌐 Kiểm tra network connectivity từ backend:"
if docker ps | grep -q fpt-tracking-backend; then
    echo "   Testing host.docker.internal..."
    docker exec fpt-tracking-backend ping -c 1 host.docker.internal 2>&1 | head -2 || echo "   ⚠️ host.docker.internal không accessible"
fi

echo ""
echo "✅ Hoàn tất kiểm tra!"

