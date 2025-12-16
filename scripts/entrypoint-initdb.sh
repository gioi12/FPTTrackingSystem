#!/bin/bash
# Entrypoint script để chạy SQL script khi SQL Server khởi động

# Chờ SQL Server sẵn sàng
echo "⏳ Đợi SQL Server khởi động..."
for i in {1..60}; do
    if /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "SELECT 1" &> /dev/null; then
        echo "✅ SQL Server đã sẵn sàng!"
        break
    fi
    sleep 2
done

# Kiểm tra xem database đã tồn tại chưa
DB_EXISTS=$(/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -d master -Q "SELECT COUNT(*) FROM sys.databases WHERE name = 'FPTTrackingSystem'" -h -1 -W 2>/dev/null | tr -d ' ')

if [ "$DB_EXISTS" != "1" ] && [ -f "/docker-entrypoint-initdb.d/init.sql" ]; then
    echo "📦 Đang tạo database từ SQL script..."
    
    # Chạy SQL script
    /opt/mssql-tools/bin/sqlcmd \
        -S localhost \
        -U sa \
        -P "$MSSQL_SA_PASSWORD" \
        -i /docker-entrypoint-initdb.d/init.sql \
        -l 30
    
    if [ $? -eq 0 ]; then
        echo "✅ Database đã được tạo từ SQL script!"
    else
        echo "⚠️ Có lỗi khi chạy SQL script, thử phương pháp thay thế..."
        
        # Tạo database trước
        /opt/mssql-tools/bin/sqlcmd \
            -S localhost \
            -U sa \
            -P "$MSSQL_SA_PASSWORD" \
            -d master \
            -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FPTTrackingSystem') CREATE DATABASE [FPTTrackingSystem]"
        
        # Chạy phần còn lại (bỏ qua phần CREATE DATABASE)
        /opt/mssql-tools/bin/sqlcmd \
            -S localhost \
            -U sa \
            -P "$MSSQL_SA_PASSWORD" \
            -d FPTTrackingSystem \
            -i /docker-entrypoint-initdb.d/init.sql \
            -l 30
    fi
else
    echo "ℹ️ Database đã tồn tại hoặc không có file init.sql"
fi

# Chạy SQL Server mặc định
exec /opt/mssql/bin/sqlservr

