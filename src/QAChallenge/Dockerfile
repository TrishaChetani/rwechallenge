FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/QAChallenge/QAChallenge.csproj", "src/QAChallenge/"]
COPY ["src/QAChallenge.RabbitMQ/QAChallenge.RabbitMQ.csproj", "src/QAChallenge.RabbitMQ/"]
RUN dotnet restore "src/QAChallenge/QAChallenge.csproj"
COPY . .
WORKDIR "/src/src/QAChallenge"
RUN dotnet build "QAChallenge.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QAChallenge.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QAChallenge.dll"]
