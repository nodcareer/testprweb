FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "testpr.web.csproj" -c Release -o /published  /p:UseAppHost=false  

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
USER root
RUN apt-get update && apt-get install -y nano && rm -rf /var/lib/apt/lists/*
COPY --from=build /published .
ENTRYPOINT [ "dotnet", "testpr.web.dll" ]