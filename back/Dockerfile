FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln .
COPY ["Messenger/Messenger.csproj", "Messenger/"]

RUN dotnet restore "Messenger/Messenger.csproj"

COPY . .

RUN dotnet publish -c release -o out "Messenger/Messenger.csproj"
RUN rm -rf ~/.nuget/packages/*

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app
COPY --from=build /src/out ./
EXPOSE 5000

ENTRYPOINT ["dotnet", "Messenger.dll"]