FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY ["QuizWebApp.Frontend/QuizWebApp.Frontend.csproj", "QuizWebApp.Frontend/"]
COPY ["QuizWebApp.Shared/QuizWebApp.Shared.csproj", "QuizWebApp.Shared/"]

RUN dotnet restore "QuizWebApp.Frontend/QuizWebApp.Frontend.csproj"

COPY . .
WORKDIR "/src/QuizWebApp.Frontend"
RUN dotnet publish "QuizWebApp.Frontend.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY QuizWebApp.Frontend/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
