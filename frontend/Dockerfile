# --- Build stage: compile the React app into static assets ---
FROM node:24-alpine AS build
WORKDIR /app

# Install dependencies first so Docker caches this layer between code changes.
COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build

# --- Serve stage: nginx serves the SPA and proxies /api to the backend ---
FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
