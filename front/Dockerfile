FROM node:18-alpine

WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci

COPY . .

ENV CHOKIDAR_USEPOLLING=true
ENV WDS_SOCKET_PORT=0

EXPOSE 3000
CMD ["npm", "start"]