# Hướng dẫn Deploy Backend lên GCP VM

## Yêu cầu
- Docker và Docker Compose đã được cài đặt trên GCP VM
- Port 5000 đã được mở trong firewall của GCP

## Các bước deploy

### 1. Build và chạy với Docker Compose (Khuyến nghị)
```bash
# Di chuyển vào thư mục dự án
cd FPTTrackingSystem

# Build và chạy containers
docker-compose up -d --build

# Xem logs
docker-compose logs -f backend
```

### 2. Build và chạy với Docker thủ công
```bash
# Build image
docker build -t fpt-tracking-backend:latest .

# Chạy container
docker run -d \
  --name fpt-tracking-backend \
  -p 5000:5000 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ASPNETCORE_URLS=http://+:5000 \
  -v $(pwd)/wwwroot:/app/wwwroot \
  --restart unless-stopped \
  fpt-tracking-backend:latest

# Xem logs
docker logs -f fpt-tracking-backend
```

### 3. Cấu hình Firewall trên GCP
```bash
# Mở port 5000
gcloud compute firewall-rules create allow-backend-port-5000 \
  --allow tcp:5000 \
  --source-ranges 0.0.0.0/0 \
  --description "Allow backend on port 5000"
```

### 4. Kiểm tra ứng dụng
- API: http://YOUR_GCP_VM_IP:5000
- Swagger: http://YOUR_GCP_VM_IP:5000/swagger

## Lưu ý
- File `appsettings.Production.json` đã được cấu hình với connection string và các settings cần thiết
- SQL Server sẽ chạy trong container với:
  - Username: `sa`
  - Password: `YourStrong@Password123` (nên thay đổi trong production)
  - Port: `1433`
- RabbitMQ sẽ chạy trong container riêng nếu sử dụng docker-compose
- Thư mục `wwwroot` sẽ được mount để lưu trữ uploads
- Container sẽ tự động restart nếu bị crash
- Database sẽ được tạo tự động từ file `FptTrackingSystem_FiNAL.sql` khi chạy script deploy

## Cấu hình Database

### Khởi tạo Database từ SQL Script
Hệ thống sẽ **tự động chạy file `FptTrackingSystem_FiNAL.sql`** để tạo database và schema khi deploy:
- Script `init-database.sh` sẽ tự động chuẩn bị file SQL cho Docker (sửa đường dẫn Windows)
- Database sẽ được tạo tự động từ SQL script khi chạy `./deploy.sh` hoặc `./init-database.sh`
- Nếu database đã tồn tại, script sẽ bỏ qua việc tạo mới

**Chuẩn bị file SQL thủ công (tùy chọn):**
```bash
chmod +x scripts/prepare-sql.sh
./scripts/prepare-sql.sh
```
File `FptTrackingSystem_FiNAL_Docker.sql` sẽ được tạo với các đường dẫn đã được sửa cho Docker.

### Thay đổi mật khẩu SQL Server
Nếu muốn thay đổi mật khẩu SQL Server, cập nhật trong:
1. `docker-compose.yml` - biến môi trường `MSSQL_SA_PASSWORD`
2. `docker-compose.yml` - connection string trong backend service
3. `appsettings.Production.json` - connection string

### Kết nối từ bên ngoài
Để kết nối SQL Server từ máy tính khác:
- Host: `YOUR_GCP_VM_IP`
- Port: `1433`
- Username: `sa`
- Password: `YourStrong@Password123`
- Database: `FPTTrackingSystem`

### Backup và Restore Database
```bash
# Backup
docker exec fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -Q "BACKUP DATABASE FPTTrackingSystem TO DISK='/var/opt/mssql/backup/FPTTrackingSystem.bak'"

# Restore
docker exec fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -Q "RESTORE DATABASE FPTTrackingSystem FROM DISK='/var/opt/mssql/backup/FPTTrackingSystem.bak' WITH REPLACE"
```

## Troubleshooting
```bash
# Xem logs của container
docker logs fpt-tracking-backend

# Vào trong container để debug
docker exec -it fpt-tracking-backend /bin/bash

# Dừng và xóa container
docker stop fpt-tracking-backend
docker rm fpt-tracking-backend

# Xóa image
docker rmi fpt-tracking-backend:latest
```

