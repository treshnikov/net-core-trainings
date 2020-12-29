FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

COPY *.sln .

# copy everything else and build app
COPY WebServer/. ./WebServer/
COPY BusinessLogic/. ./BusinessLogic/
COPY DAL/. ./DAL/
COPY Domain/. ./Domain/

#RUN dotnet restore

WORKDIR /app/WebServer
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/WebServer/out ./
EXPOSE 5000
EXPOSE 80
ENTRYPOINT ["dotnet", "WebServer.dll"]
