# Hướng dẫn sử dụng SQL Server đã build riêng

## Tổng quan
Hệ thống sử dụng SQL Server container đã được build riêng (`sqlpreview`) thay vì tạo trong docker-compose.

## Yêu cầu
- Container SQL Server `sqlpreview` đang chạy trên port 1433
- Container phải accessible từ Docker network

## Kiểm tra SQL Server

```bash
# Kiểm tra container có đang chạy không
docker ps | grep sqlpreview

# Xem logs SQL Server
docker logs sqlpreview

# Kiểm tra kết nối
docker exec sqlpreview /opt/mssql-tools/bin/sqlcmd \
    -S localhost -U sa -P "YourStrong@Password123" -C -Q "SELECT @@VERSION"
```

## Cấu hình Connection String

### Từ Backend Container
Backend container sử dụng `host.docker.internal` để kết nối đến SQL Server trên host:
```
Server=host.docker.internal,1433;Database=FPTTrackingSystem;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;MultipleActiveResultSets=True
```

### Nếu `host.docker.internal` không hoạt động (Linux)

**Cách 1: Sử dụng IP của host**
```bash
# Lấy IP của host
ip addr show docker0 | grep inet

# Hoặc
hostname -I | awk '{print $1}'
```

Cập nhật connection string trong `docker-compose.yml`:
```yaml
ConnectionStrings__DefaultConnection=Server=172.17.0.1,1433;Database=FPTTrackingSystem;...
```

**Cách 2: Thêm container vào cùng network**
```bash
# Kiểm tra network của sqlpreview
docker inspect sqlpreview | grep NetworkMode

# Thêm backend vào cùng network
docker network connect <network_name> fpt-tracking-backend

# Cập nhật connection string để dùng container name
ConnectionStrings__DefaultConnection=Server=sqlpreview,1433;Database=FPTTrackingSystem;...
```

## Khởi tạo Database

```bash
# Chạy script init database
chmod +x init-database.sh
./init-database.sh

# Script sẽ tự động:
# 1. Kiểm tra container sqlpreview
# 2. Chờ SQL Server sẵn sàng
# 3. Tạo database từ file FptTrackingSystem_FiNAL.sql
```

## Troubleshooting

### Backend không kết nối được SQL Server

1. **Kiểm tra container sqlpreview có chạy không:**
```bash
docker ps | grep sqlpreview
```

2. **Kiểm tra network:**
```bash
# Xem network của sqlpreview
docker inspect sqlpreview | grep -A 10 Networks

# Xem network của backend
docker inspect fpt-tracking-backend | grep -A 10 Networks
```

3. **Test kết nối từ backend container:**
```bash
# Vào trong backend container
docker exec -it fpt-tracking-backend /bin/bash

# Test kết nối (nếu có sqlcmd trong backend)
# Hoặc test từ host
docker exec sqlpreview /opt/mssql-tools/bin/sqlcmd \
    -S localhost -U sa -P "YourStrong@Password123" -C -Q "SELECT 1"
```

4. **Nếu dùng Linux, thử thêm vào docker-compose.yml:**
```yaml
extra_hosts:
  - "host.docker.internal:host-gateway"
```

### Container sqlpreview không có trong cùng network

Nếu `sqlpreview` không trong cùng Docker network với backend, có 2 cách:

**Cách 1: Kết nối sqlpreview vào network của backend**
```bash
docker network connect fpttrackingsystem_fpt-tracking-network sqlpreview
```

**Cách 2: Sử dụng IP của host**
```bash
# Lấy IP của host
ip route show default | awk '/default/ {print $3}'

# Cập nhật connection string với IP này
```

## Cập nhật Password

Nếu container `sqlpreview` dùng password khác:

1. **Cập nhật docker-compose.yml:**
```yaml
ConnectionStrings__DefaultConnection=Server=host.docker.internal,1433;Database=FPTTrackingSystem;User Id=sa;Password=YOUR_PASSWORD;...
```

2. **Cập nhật appsettings.Production.json:**
```json
"DefaultConnection": "Server=host.docker.internal,1433;Database=FPTTrackingSystem;User Id=sa;Password=YOUR_PASSWORD;..."
```

3. **Cập nhật init-database.sh:**
Thay `YourStrong@Password123` bằng password thực tế của container `sqlpreview`.

