FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SistemaAcademia/SistemaAcademia.csproj", "SistemaAcademia/"]
RUN dotnet restore "SistemaAcademia/SistemaAcademia.csproj"
COPY . .
RUN dotnet publish "SistemaAcademia/SistemaAcademia.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SistemaAcademia.dll"]
