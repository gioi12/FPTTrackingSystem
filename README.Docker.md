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
- RabbitMQ sẽ chạy trong container riêng nếu sử dụng docker-compose
- Thư mục `wwwroot` sẽ được mount để lưu trữ uploads
- Container sẽ tự động restart nếu bị crash

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

