# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app

# Switch to root to install packages
USER root

# Install ffmpeg and libgdiplus in the base image
RUN apk add libgdiplus --update-cache --repository http://dl-3.alpinelinux.org/alpine/edge/testing/ --allow-untrusted && \
    apk update && apk add ffmpeg

# Switch back to the app user
USER app

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SGSMTube-Web/SGSMTube-Web.csproj", "SGSMTube-Web/"]
RUN dotnet restore "./SGSMTube-Web/SGSMTube-Web.csproj"
COPY . .
WORKDIR "/src/SGSMTube-Web"
RUN dotnet build "./SGSMTube-Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SGSMTube-Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SGSMTube-Web.dll"]
