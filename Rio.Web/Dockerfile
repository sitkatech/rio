FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

ENV NODE_VERSION 14.17.6
ENV NODE_DOWNLOAD_SHA 19e376214450e93e58687198070b4ab46e42357032ec65f23a7e35b0e86ad6e2

RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-linux-x64.tar.gz" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs

    
COPY Rio.Web/                        ./Rio.Web

RUN dotnet restore Rio.Web/Rio.Web.csproj

RUN dotnet build ./Rio.Web/Rio.Web.csproj -c Release -o /app
WORKDIR /src/Rio.Web/

RUN npm cache clean --force
RUN npm uninstall   -g angular-cli
RUN npm install     -g @angular/cli@12.2.13
RUN npm install

ARG env
RUN npm run build-${env}

FROM build AS publish
RUN dotnet publish Rio.Web.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Rio.Web.dll"]
