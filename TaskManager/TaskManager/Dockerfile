FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 80

COPY TaskManager.sln ./TaskManager.sln
COPY BusinessLogicLayer/ ./BusinessLogicLayer/
COPY DataAccessLayer/ ./DataAccessLayer/
COPY TaskManager/ ./TaskManager/
COPY TaskManager.Tests/ ./TaskManager.Tests/

RUN dotnet restore ./TaskManager.sln

RUN dotnet publish ./TaskManager/TaskManager.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "TaskManager.dll"]