# STSTAGE 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Biến này đại diện cho TÊN THƯ MỤC (dùng để COPY và trỏ đường dẫn)
ARG PROJECT_DIR=ShopOnlineApi
# Biến này đại diện cho TÊN FILE .CSPROJ (không bao gồm đuôi .csproj) và tên DLL đầu ra
ARG PROJECT_NAME=ShopOnline.Api

# 1. Copy file solution và tất cả .csproj sang để cache layer
COPY ShopOnlineApi.sln ./
COPY ShopOnlineApi/ShopOnline.Api.csproj ./ShopOnlineApi/
COPY ShopOnline.Common/ShopOnline.Common.csproj ./ShopOnline.Common/
COPY ShopOnline.IdentityServer/ShopOnline.IdentityServer.csproj ./ShopOnline.IdentityServer/
COPY ShopOnline.Api.Tests/ShopOnline.Api.Tests.csproj ./ShopOnline.Api.Tests/

# Restore dependencies
RUN dotnet restore ShopOnlineApi.sln

# 2. Copy source code thực tế sang
COPY ShopOnlineApi/ ./ShopOnlineApi/
COPY ShopOnline.Common/ ./ShopOnline.Common/
COPY ShopOnline.IdentityServer/ ./ShopOnline.IdentityServer/

# 3. Build chính xác: Thư mục (ShopOnlineApi) / File (ShopOnline.Api.csproj)
RUN dotnet publish ${PROJECT_DIR}/${PROJECT_NAME}.csproj -c Release -o out


# STAGE 2: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Khai báo lại để pass giá trị PROJECT_NAME sang Stage 2 làm tên file .dll
ARG PROJECT_NAME
ENV DLL_NAME=${PROJECT_NAME}.dll

# Copy output từ Stage 1
COPY --from=build-env /app/out .

EXPOSE 8080

# Thực thi ứng dụng (Sẽ chạy: dotnet ShopOnline.Api.dll)
ENTRYPOINT ["sh", "-c", "exec dotnet $DLL_NAME"]