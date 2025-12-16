#!/bin/bash

# Script để khởi tạo database và chạy SQL script
# Sử dụng: ./init-database.sh

echo "🔧 Đang khởi tạo database..."

# Đợi SQL Server sẵn sàng
echo "⏳ Đợi SQL Server khởi động..."
max_attempts=60
attempt=0

until docker exec fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -Q "SELECT 1" &> /dev/null
do
    attempt=$((attempt + 1))
    if [ $attempt -ge $max_attempts ]; then
        echo "❌ SQL Server không khởi động được sau $max_attempts lần thử"
        exit 1
    fi
    echo "Đang đợi SQL Server... ($attempt/$max_attempts)"
    sleep 2
done

echo "✅ SQL Server đã sẵn sàng!"

# Kiểm tra xem database đã tồn tại chưa
DB_EXISTS=$(docker exec fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P "YourStrong@Password123" \
    -d master \
    -Q "SELECT COUNT(*) FROM sys.databases WHERE name = 'FPTTrackingSystem'" \
    -h -1 -W 2>/dev/null | tr -d ' ')

if [ "$DB_EXISTS" = "1" ]; then
    echo "ℹ️ Database FPTTrackingSystem đã tồn tại, bỏ qua việc tạo mới"
else
    echo "📦 Đang tạo database từ SQL script..."
    
    # Chuẩn bị file SQL cho Docker nếu chưa có
    if [ ! -f "FptTrackingSystem_FiNAL_Docker.sql" ]; then
        echo "🔧 Đang chuẩn bị file SQL cho Docker..."
        if [ -f "scripts/prepare-sql.sh" ]; then
            chmod +x scripts/prepare-sql.sh
            ./scripts/prepare-sql.sh
        else
            echo "⚠️ Không tìm thấy script prepare-sql.sh, sử dụng file gốc..."
        }
    fi
    
    # Xác định file SQL để sử dụng
    SQL_FILE="FptTrackingSystem_FiNAL.sql"
    if [ -f "FptTrackingSystem_FiNAL_Docker.sql" ]; then
        SQL_FILE="FptTrackingSystem_FiNAL_Docker.sql"
        echo "✅ Sử dụng file SQL đã được chuẩn bị cho Docker"
    else
        echo "⚠️ Sử dụng file SQL gốc (có thể có vấn đề với đường dẫn Windows)"
    fi
    
    if [ ! -f "$SQL_FILE" ]; then
        echo "❌ Không tìm thấy file SQL: $SQL_FILE"
        exit 1
    fi
    
    echo "📄 Đang copy file SQL vào container..."
    docker cp "$SQL_FILE" fpt-tracking-sqlserver:/tmp/init.sql
    
    # Chạy SQL script
    echo "🚀 Đang chạy SQL script để tạo database và schema..."
    docker exec fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd \
        -S localhost \
        -U sa \
        -P "YourStrong@Password123" \
        -i /tmp/init.sql \
        -l 30 \
        || {
        echo "⚠️ Có lỗi khi chạy SQL script, thử phương pháp thay thế..."
        
        # Phương pháp thay thế: Tạo database trước, sau đó chạy phần còn lại
        echo "📦 Tạo database trước..."
        docker exec fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd \
            -S localhost \
            -U sa \
            -P "YourStrong@Password123" \
            -d master \
            -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FPTTrackingSystem') CREATE DATABASE [FPTTrackingSystem]"
        
        # Chạy phần còn lại của script (bỏ qua phần CREATE DATABASE)
        echo "🔄 Đang chạy schema và tables..."
        docker exec fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd \
            -S localhost \
            -U sa \
            -P "YourStrong@Password123" \
            -d FPTTrackingSystem \
            -i /tmp/init.sql \
            -l 30
    }
    
    echo "✅ Database đã được tạo từ SQL script!"
fi

echo "✅ Hoàn tất khởi tạo database!"

