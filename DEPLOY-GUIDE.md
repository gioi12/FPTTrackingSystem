# Hướng dẫn Deploy Backend với Database lên GCP VM

## Tổng quan
Hệ thống bao gồm:
- **Backend API** (.NET 8.0) - Port 5000
- **SQL Server 2022** - Port 1433
- **RabbitMQ** - Ports 5672, 15672

## Yêu cầu
- Docker và Docker Compose đã được cài đặt
- Port 5000, 1433 đã được mở trong GCP Firewall

## Các bước Deploy

### 1. Clone/Upload code lên GCP VM
```bash
# Nếu dùng Git
git clone <your-repo-url>
cd FPTTrackingSystem

# Hoặc upload code qua SCP/SFTP
```

### 2. Deploy tự động (Khuyến nghị)
```bash
# Linux/Mac
chmod +x deploy.sh
./deploy.sh

# Windows (PowerShell)
.\deploy.ps1
```

### 3. Deploy thủ công
```bash
# Build và chạy containers
docker-compose up -d --build

# Đợi SQL Server khởi động (khoảng 15-30 giây)
sleep 20

# Khởi tạo database
chmod +x init-database.sh
./init-database.sh

# Hoặc trên Windows
.\init-database.ps1
```

### 4. Kiểm tra trạng thái
```bash
# Xem logs
docker-compose logs -f backend

# Kiểm tra containers
docker-compose ps

# Kiểm tra database
docker exec -it fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Password123" \
  -Q "SELECT name FROM sys.databases"
```

## Truy cập Services

- **Backend API**: `http://YOUR_GCP_VM_IP:5000`
- **Swagger UI**: `http://YOUR_GCP_VM_IP:5000/swagger`
- **RabbitMQ Management**: `http://YOUR_GCP_VM_IP:15672` (guest/guest)
- **SQL Server**: `YOUR_GCP_VM_IP:1433`

## Cấu hình Database

### Thông tin đăng nhập mặc định
- **Server**: `sqlserver` (trong Docker network) hoặc `YOUR_GCP_VM_IP` (từ bên ngoài)
- **Port**: `1433`
- **Username**: `sa`
- **Password**: `YourStrong@Password123` ⚠️ **Nên thay đổi trong production!**
- **Database**: `FPTTrackingSystem`

### Thay đổi mật khẩu SQL Server

1. **Cập nhật docker-compose.yml**:
```yaml
sqlserver:
  environment:
    - MSSQL_SA_PASSWORD=YourNewStrongPassword123
```

2. **Cập nhật connection string trong docker-compose.yml**:
```yaml
backend:
  environment:
    - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=FPTTrackingSystem;User Id=sa;Password=YourNewStrongPassword123;TrustServerCertificate=True;MultipleActiveResultSets=True
```

3. **Cập nhật appsettings.Production.json**:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=sqlserver;Database=FPTTrackingSystem;User Id=sa;Password=YourNewStrongPassword123;TrustServerCertificate=True;MultipleActiveResultSets=True"
}
```

4. **Rebuild và restart**:
```bash
docker-compose down
docker-compose up -d --build
```

## Kết nối Database từ bên ngoài

### Sử dụng SQL Server Management Studio (SSMS)
- **Server name**: `YOUR_GCP_VM_IP,1433`
- **Authentication**: SQL Server Authentication
- **Login**: `sa`
- **Password**: `YourStrong@Password123`

### Sử dụng Azure Data Studio hoặc VS Code
- Connection string: `Server=YOUR_GCP_VM_IP,1433;Database=FPTTrackingSystem;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True`

## Backup và Restore Database

### Backup
```bash
docker exec fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Password123" \
  -Q "BACKUP DATABASE FPTTrackingSystem TO DISK='/var/opt/mssql/backup/FPTTrackingSystem_$(date +%Y%m%d_%H%M%S).bak'"
```

### Restore
```bash
# Copy file backup vào container
docker cp backup.bak fpt-tracking-sqlserver:/var/opt/mssql/backup/

# Restore
docker exec fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "YourStrong@Password123" \
  -Q "RESTORE DATABASE FPTTrackingSystem FROM DISK='/var/opt/mssql/backup/backup.bak' WITH REPLACE"
```

## Troubleshooting

### SQL Server container không healthy
```bash
# Kiểm tra logs SQL Server
docker-compose logs sqlserver

# Kiểm tra health status
docker inspect --format='{{.State.Health.Status}}' fpt-tracking-sqlserver

# Xem chi tiết healthcheck
docker inspect --format='{{json .State.Health}}' fpt-tracking-sqlserver | python3 -m json.tool

# Thử kết nối trực tiếp
docker exec fpt-tracking-sqlserver /opt/mssql-tools/bin/sqlcmd \
    -S localhost -U sa -P "YourStrong@Password123" -C -Q "SELECT 1"

# Nếu vẫn lỗi, chạy script fix
chmod +x fix-sqlserver.sh
./fix-sqlserver.sh
```

### Backend không kết nối được database
```bash
# Kiểm tra SQL Server có chạy không
docker-compose ps sqlserver

# Kiểm tra logs SQL Server
docker-compose logs sqlserver

# Kiểm tra connection string
docker exec fpt-tracking-backend printenv | grep ConnectionStrings

# Kiểm tra network
docker network inspect fpttrackingsystem_fpt-tracking-network
```

### Database chưa được tạo
```bash
# Chạy lại script init database
./init-database.sh
```

### Port đã được sử dụng
```bash
# Kiểm tra port nào đang sử dụng
sudo netstat -tulpn | grep :5000
sudo netstat -tulpn | grep :1433

# Thay đổi port trong docker-compose.yml nếu cần
```

### Xóa và khởi động lại
```bash
# Dừng và xóa containers (giữ lại data volumes)
docker-compose down

# Xóa tất cả bao gồm volumes (⚠️ Mất dữ liệu!)
docker-compose down -v

# Khởi động lại
docker-compose up -d --build
```

## Cấu hình Firewall GCP

```bash
# Mở port 5000 (Backend)
gcloud compute firewall-rules create allow-backend-5000 \
  --allow tcp:5000 \
  --source-ranges 0.0.0.0/0 \
  --description "Allow backend API on port 5000"

# Mở port 1433 (SQL Server) - Chỉ nên mở cho IP cụ thể trong production!
gcloud compute firewall-rules create allow-sqlserver-1433 \
  --allow tcp:1433 \
  --source-ranges YOUR_IP_ADDRESS/32 \
  --description "Allow SQL Server on port 1433"
```

## Monitoring

### Xem logs real-time
```bash
# Backend logs
docker-compose logs -f backend

# SQL Server logs
docker-compose logs -f sqlserver

# Tất cả logs
docker-compose logs -f
```

### Kiểm tra tài nguyên
```bash
# Sử dụng CPU, Memory
docker stats

# Disk usage
docker system df
```

## Production Checklist

- [ ] Đã thay đổi mật khẩu SQL Server mặc định
- [ ] Đã cấu hình firewall chỉ cho phép IP cần thiết
- [ ] Đã backup database
- [ ] Đã test kết nối từ bên ngoài
- [ ] Đã cấu hình SSL/TLS cho production (nếu cần)
- [ ] Đã setup monitoring và logging
- [ ] Đã cấu hình auto-restart cho containers

