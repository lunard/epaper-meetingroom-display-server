#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
COPY ["NOITechParkDoorSignage/NOITechParkDoorSignage.csproj", "NOITechParkDoorSignage/"]
RUN dotnet restore "./NOITechParkDoorSignage/./NOITechParkDoorSignage.csproj"
COPY . .
WORKDIR "/NOITechParkDoorSignage"
RUN dotnet build "./NOITechParkDoorSignage.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NOITechParkDoorSignage.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ARG ASPNETCORE_URLS
ARG ASPNETCORE_ENVIRONMENT
ARG LOG_PATH
ARG LOG_LEVEL
ARG AZURE_TENANT_ID
ARG AZURE_CLIENT_ID
ARG AZURE_CLIENT_SECRET

ENV ASPNETCORE_URLS=$ASPNETCORE_URLS
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
ENV LOG_PATH=$LOG_PATH
ENV LOG_LEVEL=$LOG_LEVEL
ENV AZURE_TENANT_ID=$AZURE_TENANT_ID
ENV AZURE_CLIENT_ID=$AZURE_CLIENT_ID
ENV AZURE_CLIENT_SECRET=$AZURE_CLIENT_SECRET


ENTRYPOINT ["dotnet", "NOITechParkDoorSignage.dll"]