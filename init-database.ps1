# Script để khởi tạo database và chạy SQL script (PowerShell)
# Sử dụng: .\init-database.ps1

Write-Host "🔧 Đang khởi tạo database..." -ForegroundColor Green

# SQL Server container name (đã được build riêng)
$sqlContainer = "sqlpreview"

# Đợi SQL Server sẵn sàng
Write-Host "⏳ Đợi SQL Server ($sqlContainer) khởi động..." -ForegroundColor Yellow
$maxAttempts = 60
$attempt = 0
$ready = $false

while (-not $ready -and $attempt -lt $maxAttempts) {
    try {
        $result = docker exec $sqlContainer /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "StrongP@ssw0rd!" -C -Q "SELECT 1" 2>&1
        if ($LASTEXITCODE -eq 0) {
            $ready = $true
        }
    } catch {
        # Continue waiting
    }
    
    if (-not $ready) {
        Write-Host "Đang đợi SQL Server... ($attempt/$maxAttempts)" -ForegroundColor Yellow
        Start-Sleep -Seconds 2
        $attempt++
    }
}

if (-not $ready) {
    Write-Host "❌ SQL Server ($sqlContainer) không khởi động được sau $maxAttempts lần thử" -ForegroundColor Red
    Write-Host "💡 Kiểm tra container: docker ps | grep $sqlContainer" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ SQL Server đã sẵn sàng!" -ForegroundColor Green

# Kiểm tra xem database đã tồn tại chưa
Write-Host "🔍 Kiểm tra database..." -ForegroundColor Yellow
$dbExists = docker exec $sqlContainer /opt/mssql-tools/bin/sqlcmd `
    -S localhost `
    -U sa `
    -P "StrongP@ssw0rd!" `
    -C `
    -d master `
    -Q "SELECT COUNT(*) FROM sys.databases WHERE name = 'FPTTrackingSystem'" `
    -h -1 -W 2>$null

$dbExists = $dbExists.Trim()

if ($dbExists -eq "1") {
    Write-Host "ℹ️ Database FPTTrackingSystem đã tồn tại, bỏ qua việc tạo mới" -ForegroundColor Cyan
} else {
    Write-Host "📦 Đang tạo database từ SQL script..." -ForegroundColor Yellow
    
    # Chuẩn bị file SQL cho Docker nếu chưa có
    if (-not (Test-Path "FptTrackingSystem_FiNAL_Docker.sql")) {
        Write-Host "🔧 Đang chuẩn bị file SQL cho Docker..." -ForegroundColor Yellow
        if (Test-Path "scripts/prepare-sql.sh") {
            bash scripts/prepare-sql.sh
        } elseif (Test-Path "scripts/fix-sql-for-docker.ps1") {
            & "scripts/fix-sql-for-docker.ps1"
        } else {
            Write-Host "⚠️ Không tìm thấy script prepare, sử dụng file gốc..." -ForegroundColor Yellow
        }
    }
    
    # Xác định file SQL để sử dụng
    $sqlFile = "FptTrackingSystem_FiNAL.sql"
    if (Test-Path "FptTrackingSystem_FiNAL_Docker.sql") {
        $sqlFile = "FptTrackingSystem_FiNAL_Docker.sql"
        Write-Host "✅ Sử dụng file SQL đã được chuẩn bị cho Docker" -ForegroundColor Green
    } else {
        Write-Host "⚠️ Sử dụng file SQL gốc (có thể có vấn đề với đường dẫn Windows)" -ForegroundColor Yellow
    }
    
    if (-not (Test-Path $sqlFile)) {
        Write-Host "❌ Không tìm thấy file SQL: $sqlFile" -ForegroundColor Red
        exit 1
    }
    
    # Copy file SQL vào container
    Write-Host "📄 Đang copy file SQL vào container..." -ForegroundColor Yellow
    docker cp $sqlFile "${sqlContainer}:/tmp/init.sql"
    
    # Chạy SQL script
    Write-Host "🚀 Đang chạy SQL script để tạo database và schema..." -ForegroundColor Yellow
    docker exec $sqlContainer /opt/mssql-tools/bin/sqlcmd `
        -S localhost `
        -U sa `
        -P "StrongP@ssw0rd!" `
        -C `
        -i /tmp/init.sql `
        -l 30
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "⚠️ Có lỗi khi chạy SQL script, thử phương pháp thay thế..." -ForegroundColor Yellow
        
        # Phương pháp thay thế: Tạo database trước
        Write-Host "📦 Tạo database trước..." -ForegroundColor Yellow
        docker exec $sqlContainer /opt/mssql-tools/bin/sqlcmd `
            -S localhost `
            -U sa `
            -P "StrongP@ssw0rd!" `
            -C `
            -d master `
            -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FPTTrackingSystem') CREATE DATABASE [FPTTrackingSystem]"
        
        # Chạy phần còn lại của script
        Write-Host "🔄 Đang chạy schema và tables..." -ForegroundColor Yellow
        docker exec $sqlContainer /opt/mssql-tools/bin/sqlcmd `
            -S localhost `
            -U sa `
            -P "StrongP@ssw0rd!" `
            -C `
            -d FPTTrackingSystem `
            -i /tmp/init.sql `
            -l 30
    }
    
    Write-Host "✅ Database đã được tạo từ SQL script!" -ForegroundColor Green
}

Write-Host "✅ Hoàn tất khởi tạo database!" -ForegroundColor Green

