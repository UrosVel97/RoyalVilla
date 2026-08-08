# Ubuntu VPS deployment

The production stack runs the MVC app, API, and PostgreSQL in separate containers. The MVC app is bound to the VPS loopback interface for a reverse proxy, the API is published on port `5000`, and PostgreSQL stays on a private Docker network.

## Prerequisites

Install Docker Engine and the Docker Compose plugin on the VPS, then clone this repository.

## Configure secrets

Create the deployment environment file:

```bash
cp .env.example .env
```

Generate independent values for `POSTGRES_PASSWORD` and `JWT_SECRET`:

```bash
openssl rand -hex 32
openssl rand -hex 32
```

Put those values in `.env`. Do not commit that file.

## Start the stack

```bash
docker compose -f compose.production.yaml up -d --build
docker compose -f compose.production.yaml ps
curl --fail http://127.0.0.1:8080/health
curl --fail http://127.0.0.1:5000/health
```

Scalar is available at `http://<vps-address>:5000/scalar`. API routes are available from the same public origin. Set `OPENAPI_ENABLED=false` to disable the documentation without disabling the API.

The API applies pending Entity Framework Core migrations when it starts. View service logs with:

```bash
docker compose -f compose.production.yaml logs -f web api postgres
```

## Publish through HTTPS

Keep port `8080` closed in the VPS firewall. Point Nginx or another TLS reverse proxy at `http://127.0.0.1:8080` and forward the original host and scheme. A minimal Nginx location is:

```nginx
location / {
    proxy_pass http://127.0.0.1:8080;
    proxy_http_version 1.1;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```

Port `5000` is published publicly for the API and Scalar. For a real deployment, proxy a dedicated API hostname to port `5000`, terminate TLS at the reverse proxy, and then block direct public access to port `5000` in the VPS firewall.

## Update or stop

```bash
git pull
docker compose -f compose.production.yaml up -d --build
docker image prune -f
```

```bash
docker compose -f compose.production.yaml down
```

Do not add `--volumes` to `down` unless the PostgreSQL data should be deleted.