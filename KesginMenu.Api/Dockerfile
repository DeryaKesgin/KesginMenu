FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY KesginMenu.Api.csproj ./
RUN dotnet restore

COPY . ./

RUN dotnet publish \
    KesginMenu.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "KesginMenu.Api.dll"]