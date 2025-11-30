# Imagem de runtime (o que vai rodar em produção)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Porta "padrão" que a aplicação vai escutar dentro do container
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Imagem de build (SDK, compila o projeto)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia a solution e os csproj pra restaurar dependências
COPY *.sln ./
COPY Domain/Domain.csproj Domain/
COPY QuestionsApi/QuestionsApi.csproj QuestionsApi/

# Restaura os pacotes da API (e da Domain via reference)
RUN dotnet restore QuestionsApi/QuestionsApi.csproj

# Copia o restante do código
COPY . .

# Entra na pasta da API e faz o publish em Release
WORKDIR /src/QuestionsApi
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Imagem final: só o runtime + app publicado
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuestionsApi.dll"]
docker build -t questions-api .
