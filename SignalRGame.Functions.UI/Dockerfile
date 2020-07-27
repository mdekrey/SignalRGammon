FROM node:12 AS builder

WORKDIR /src
ADD SignalRGame.Functions.UI/package*.json ./
RUN npm ci
ADD SignalRGame.Functions.UI/ ./
ENV REACT_APP_API_BASE=https://dekrey-net-games.azurewebsites.net/api
RUN npm run build

FROM steebchen/nginx-spa:stable
COPY --from=builder /src/build /app
