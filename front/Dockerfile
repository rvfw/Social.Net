FROM node:24

WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
RUN npm install -g serve
COPY . .

ENV CHOKIDAR_USEPOLLING=true
ENV WDS_SOCKET_PORT=0

RUN npm run build

EXPOSE 3000
CMD ["serve", "-s", "build"]