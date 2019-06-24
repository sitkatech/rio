FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

COPY ./Rio.Models ./Rio.Models
COPY ./Rio.EFModels ./Rio.EFModels
COPY ./Rio.API ./Rio.API

RUN dotnet publish ./Rio.API/Rio.API.csproj -c Release -o /app

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS final
WORKDIR /app
COPY --from=build /app /app
RUN ls -la
EXPOSE 80
ENTRYPOINT ["dotnet", "Rio.API.dll"]