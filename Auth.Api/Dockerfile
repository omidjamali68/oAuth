## syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# --- مرحله اول: کپی csproj و restore با تنظیمات NuGet
# تنظیم متغیر محیطی برای غیرفعال کردن fallback packages
ENV NUGET_FALLBACK_PACKAGES=""

COPY Auth.Api/Auth.Api.csproj Auth.Api/
COPY Auth.Application/Auth.Application.csproj Auth.Application/
COPY Auth.Domain/Auth.Domain.csproj Auth.Domain/
COPY Auth.Infrastructure/Auth.Infrastructure.csproj Auth.Infrastructure/

# restore با متغیر محیطی که ست کردیم
RUN dotnet restore Auth.Api/Auth.Api.csproj

# --- مرحله دوم: کپی کل سورس و publish
COPY . .

# publish با disable fallback folder و confirm کردن disable
RUN dotnet publish Auth.Api/Auth.Api.csproj -c Release -o /app/publish --no-restore
# --- مرحله نهایی: runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# کپی نتیجه‌ی publish از image build
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Auth.Api.dll"]
