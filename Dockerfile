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

# 👇 AJUSTADO: pasta + nome do csproj exatos
COPY Questions.Domain/Questions.Domain.csproj Questions.Domain/
COPY Questions.API/Questions.API.csproj Questions.API/

# Restaura os pacotes da API (e da Domain via reference)
RUN dotnet restore Questions.API/Questions.API.csproj

# Copia o restante do código
COPY . .

# Entra na pasta da API e faz o publish em Release
WORKDIR /src/Questions.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Imagem final: só o runtime + app publicado
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# 👇 nome da DLL = nome do projeto (Questions.API)
ENTRYPOINT ["dotnet", "Questions.API.dll"]
