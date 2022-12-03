#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Collections.WebApi/Collections.WebApi.csproj", "src/Collections.WebApi/"]
COPY ["src/Collections.Application/Collections.Application.csproj", "src/Collections.Application/"]
COPY ["src/Collections.Domain/Collections.Domain.csproj", "src/Collections.Domain/"]
COPY ["src/Collections.Infrastructure/Collections.Infrastructure.csproj", "src/Collections.Infrastructure/"]
RUN dotnet restore "src/Collections.WebApi/Collections.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Collections.WebApi"
RUN dotnet build "Collections.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Collections.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Collections.WebApi.dll"]