# Build Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS Base
COPY bin/Release/netcoreapp3.1/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "aspnetapp.dll"]