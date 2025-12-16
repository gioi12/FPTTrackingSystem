#!/bin/bash

# Script để sửa file SQL cho Docker
# Loại bỏ đường dẫn Windows cụ thể trong CREATE DATABASE

INPUT_FILE="${1:-../FptTrackingSystem_FiNAL.sql}"
OUTPUT_FILE="${2:-../FptTrackingSystem_FiNAL_Docker.sql}"

echo "🔧 Đang sửa file SQL cho Docker..."

# Tạo file mới với các thay đổi
sed -E 's|FILENAME = N'\''C:\\Program Files\\Microsoft SQL Server\\MSSQL15\\.SQLEXPRESS\\MSSQL\\DATA\\FPTTrackingSystem\.mdf'\''|FILENAME = N'\''/var/opt/mssql/data/FPTTrackingSystem.mdf'\''|g' \
    -E 's|FILENAME = N'\''C:\\Program Files\\Microsoft SQL Server\\MSSQL15\\.SQLEXPRESS\\MSSQL\\DATA\\FPTTrackingSystem_log\.ldf'\''|FILENAME = N'\''/var/opt/mssql/data/FPTTrackingSystem_log.ldf'\''|g' \
    "$INPUT_FILE" > "$OUTPUT_FILE"

echo "✅ Đã tạo file SQL cho Docker: $OUTPUT_FILE"

