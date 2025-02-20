FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./BlazorDemo.csproj", "./"]
RUN dotnet restore "./BlazorDemo.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet publish "./BlazorDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "./BlazorDemo.dll"]
