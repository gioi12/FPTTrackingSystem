#!/bin/bash

# Script để chuẩn bị file SQL cho Docker
# Loại bỏ đường dẫn Windows và sửa các vấn đề tương thích

INPUT_FILE="FptTrackingSystem_FiNAL.sql"
OUTPUT_FILE="FptTrackingSystem_FiNAL_Docker.sql"

if [ ! -f "$INPUT_FILE" ]; then
    echo "❌ Không tìm thấy file: $INPUT_FILE"
    exit 1
fi

echo "🔧 Đang chuẩn bị file SQL cho Docker..."

# Tạo file mới với các thay đổi
cat "$INPUT_FILE" | \
    # Loại bỏ đường dẫn Windows cụ thể trong CREATE DATABASE
    sed -E 's|FILENAME = N'\''C:\\Program Files\\Microsoft SQL Server\\MSSQL15\\.SQLEXPRESS\\MSSQL\\DATA\\FPTTrackingSystem\.mdf'\''|FILENAME = N'\''/var/opt/mssql/data/FPTTrackingSystem.mdf'\''|g' | \
    sed -E 's|FILENAME = N'\''C:\\Program Files\\Microsoft SQL Server\\MSSQL15\\.SQLEXPRESS\\MSSQL\\DATA\\FPTTrackingSystem_log\.ldf'\''|FILENAME = N'\''/var/opt/mssql/data/FPTTrackingSystem_log.ldf'\''|g' | \
    # Loại bỏ AUTO_CLOSE ON (không tốt cho Docker)
    sed -E 's|ALTER DATABASE \[FPTTrackingSystem\] SET AUTO_CLOSE ON|ALTER DATABASE \[FPTTrackingSystem\] SET AUTO_CLOSE OFF|g' \
    > "$OUTPUT_FILE"

echo "✅ Đã tạo file SQL cho Docker: $OUTPUT_FILE"
echo "📝 File đã được sửa để tương thích với Docker container"

