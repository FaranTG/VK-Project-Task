FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY ["QuizWebApp.Api/QuizWebApp.Api.csproj", "QuizWebApp.Api/"]
COPY ["QuizWebApp.Core/QuizWebApp.Core.csproj", "QuizWebApp.Core/"]
COPY ["QuizWebApp.Infrastructure/QuizWebApp.Infrastructure.csproj", "QuizWebApp.Infrastructure/"]
COPY ["QuizWebApp.Shared/QuizWebApp.Shared.csproj", "QuizWebApp.Shared/"]

RUN dotnet restore "QuizWebApp.Api/QuizWebApp.Api.csproj"

COPY . .
WORKDIR "/src/QuizWebApp.Api"
RUN dotnet build "QuizWebApp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuizWebApp.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuizWebApp.Api.dll"]
