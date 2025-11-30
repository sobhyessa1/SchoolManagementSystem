# Environment Variables Configuration Guide

## Overview

For production deployment, sensitive configuration values should be stored in environment variables rather than in `appsettings.json` files. This guide explains how to configure the School Management API using environment variables.

## Required Environment Variables

### Database Configuration

```bash
# SQL Server connection details
DB_SERVER=your-sql-server.database.windows.net
DB_NAME=SchoolManagementDB
DB_USER=sqladmin
DB_PASSWORD=YourSecurePassword123!
```

### JWT Authentication

```bash
# JWT secret key (must be at least 32 characters)
JWT_SECRET_KEY=YourVeryLongAndSecureSecretKeyForJWTTokenGeneration123456789
JWT_ISSUER=SchoolManagementAPI
JWT_AUDIENCE=SchoolManagementUsers
```

### Optional Configuration

```bash
# Redis cache (optional)
REDIS_CONNECTION_STRING=your-redis-server:6379,password=yourredispassword

# SMTP for email notifications (optional)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your-email@gmail.com
SMTP_PASSWORD=your-app-password
SMTP_FROM=noreply@schoolmanagement.com
```

## Setting Environment Variables

### Windows (PowerShell)

```powershell
# Set for current session
$env:DB_SERVER="your-server.database.windows.net"
$env:DB_NAME="SchoolManagementDB"
$env:DB_USER="sqladmin"
$env:DB_PASSWORD="YourPassword123!"
$env:JWT_SECRET_KEY="YourVeryLongSecretKey123456789"
$env:JWT_ISSUER="SchoolManagementAPI"
$env:JWT_AUDIENCE="SchoolManagementUsers"

# Set permanently (system-wide)
[System.Environment]::SetEnvironmentVariable("DB_SERVER", "your-server", "Machine")
```

### Linux/macOS (Bash)

```bash
# Set for current session
export DB_SERVER="your-server.database.windows.net"
export DB_NAME="SchoolManagementDB"
export DB_USER="sqladmin"
export DB_PASSWORD="YourPassword123!"
export JWT_SECRET_KEY="YourVeryLongSecretKey123456789"
export JWT_ISSUER="SchoolManagementAPI"
export JWT_AUDIENCE="SchoolManagementUsers"

# Add to ~/.bashrc or ~/.zshrc for persistence
echo 'export DB_SERVER="your-server"' >> ~/.bashrc
```

### Docker Compose

```yaml
version: '3.8'
services:
  api:
    image: school-management-api
    environment:
      - DB_SERVER=sqlserver
      - DB_NAME=SchoolManagementDB
      - DB_USER=sa
      - DB_PASSWORD=YourStrong@Passw0rd
      - JWT_SECRET_KEY=YourVeryLongSecretKey123456789
      - JWT_ISSUER=SchoolManagementAPI
      - JWT_AUDIENCE=SchoolManagementUsers
    ports:
      - "5000:80"
```

### Azure App Service

```bash
# Using Azure CLI
az webapp config appsettings set --name your-app-name --resource-group your-rg \
  --settings \
    DB_SERVER="your-server.database.windows.net" \
    DB_NAME="SchoolManagementDB" \
    DB_USER="sqladmin" \
    DB_PASSWORD="YourPassword123!" \
    JWT_SECRET_KEY="YourVeryLongSecretKey123456789" \
    JWT_ISSUER="SchoolManagementAPI" \
    JWT_AUDIENCE="SchoolManagementUsers"
```

### .NET User Secrets (Development Only)

```bash
# Initialize user secrets
cd project1
dotnet user-secrets init

# Set secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=SchoolDB;Trusted_Connection=True;"
dotnet user-secrets set "Jwt:Key" "YourDevelopmentSecretKey123456789"
dotnet user-secrets set "Jwt:Issuer" "SchoolManagementAPI"
dotnet user-secrets set "Jwt:Audience" "SchoolManagementUsers"
```

## Configuration Priority

.NET Core loads configuration in this order (later sources override earlier ones):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User Secrets (Development only)
4. Environment Variables
5. Command-line arguments

## Security Best Practices

### ✅ DO:
- Use environment variables for all secrets in production
- Use strong, randomly generated JWT keys (32+ characters)
- Use different secrets for each environment (dev, staging, prod)
- Rotate secrets regularly
- Use Azure Key Vault or AWS Secrets Manager for production
- Enable HTTPS in production

### ❌ DON'T:
- Commit secrets to source control
- Use the same secrets across environments
- Use weak or short JWT keys
- Share production secrets in team chats or emails
- Store secrets in plain text files

## Generating Secure JWT Keys

### PowerShell
```powershell
# Generate a random 64-character key
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

### Linux/macOS
```bash
# Generate a random 64-character key
openssl rand -base64 64 | tr -d '\n'
```

### Online (use with caution)
```
https://randomkeygen.com/
```

## Verifying Configuration

After setting environment variables, verify they're loaded correctly:

```bash
# Run the application
dotnet run --project project1

# Check logs for configuration errors
# The app will fail to start if required variables are missing
```

## Production Deployment Checklist

- [ ] All secrets moved to environment variables
- [ ] JWT key is strong and unique (64+ characters)
- [ ] Database connection string uses secure credentials
- [ ] HTTPS is enforced
- [ ] `appsettings.Production.json` uses variable placeholders
- [ ] Secrets are stored in secure vault (Azure Key Vault, AWS Secrets Manager)
- [ ] Different secrets for each environment
- [ ] Secrets are not in source control
- [ ] Team members have access to production secrets via secure channels only

## Troubleshooting

### "Jwt:Key missing" error
- Ensure `JWT_SECRET_KEY` environment variable is set
- Check variable name matches exactly (case-sensitive on Linux)
- Verify the key is at least 16 characters long

### Database connection fails
- Verify `DB_SERVER`, `DB_NAME`, `DB_USER`, `DB_PASSWORD` are all set
- Check firewall rules allow connection to database server
- Ensure connection string format is correct

### Variables not loading
- Restart the application after setting environment variables
- Check environment variable scope (user vs system vs process)
- Verify variable names match the configuration structure

## Example: Complete Production Setup

```bash
# 1. Set all required environment variables
export DB_SERVER="prod-sql.database.windows.net"
export DB_NAME="SchoolManagementDB"
export DB_USER="sqladmin"
export DB_PASSWORD="SuperSecure@Password123!"
export JWT_SECRET_KEY="$(openssl rand -base64 64 | tr -d '\n')"
export JWT_ISSUER="SchoolManagementAPI"
export JWT_AUDIENCE="SchoolManagementUsers"

# 2. Build the application
dotnet publish -c Release -o ./publish

# 3. Run the application
cd publish
dotnet project1.dll

# 4. Verify it's running
curl https://localhost:5001/health
```

## Support

For issues with environment configuration:
1. Check application logs in `logs/` directory
2. Verify all required variables are set: `printenv | grep -E "(DB_|JWT_)"`
3. Review `appsettings.Production.json` for correct placeholder syntax
4. Consult deployment platform documentation (Azure, AWS, Docker, etc.)
