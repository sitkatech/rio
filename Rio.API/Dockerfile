FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /src

COPY ./Rio.Models ./Rio.Models
COPY ./Rio.EFModels ./Rio.EFModels
COPY ./Rio.API ./Rio.API

RUN dotnet publish ./Rio.API/Rio.API.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build-env /app /app
EXPOSE 80

# install GDAL
RUN apt-get update && apt-get install -y gdal-bin && apt-get install -y gnupg2

ENTRYPOINT ["dotnet", "Rio.API.dll"]