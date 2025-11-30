FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["project1/project1.csproj", "project1/"]
RUN dotnet restore "project1/project1.csproj"
COPY . .
WORKDIR "/src/project1"
RUN dotnet publish "project1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "project1.dll"]
