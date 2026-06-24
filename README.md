# Task Management API

A cloud-deployed RESTful API for task management built with ASP.NET Core, Entity Framework Core, MySQL and Docker.

## Technologies

- C#
- ASP.NET Core 8
- Entity Framework Core
- MySQL
- Docker
- Docker Compose
- AWS EC2
- Swagger/OpenAPI
- Git & GitHub

## Features

- Create tasks
- Read tasks
- Update tasks
- Delete tasks
- Persistent MySQL storage
- Interactive Swagger documentation

## Deployment

The application is deployed on an AWS EC2 Ubuntu instance using Docker Compose.

## Architecture
```
Client

↓

ASP.NET Core API

↓

Entity Framework Core

↓

MySQL
```

## Project structure

```
TaskManagerApi/
├── Controllers/
│   └── TasksController.cs   # CRUD endpoints
├── Models/
│   └── TaskItem.cs          # Entity (tasks table)
├── Data/
│   └── AppDbContext.cs      # EF Core DbContext
├── DTOs/
│   └── TaskDtos.cs          # Request/response shapes
├── Program.cs               # Entry point / configuration
├── appsettings.json
├── TaskManagerApi.csproj
├── Dockerfile
├── docker-compose.yml
└── .gitignore / .dockerignore
```

## Endpoints

| Method | Path           | Description           |
|--------|----------------|------------------------|
| POST   | /tasks         | Create a new task      |
| GET    | /tasks         | List all tasks         |
| GET    | /tasks/{id}    | Get a single task      |
| PUT    | /tasks/{id}    | Update a task          |
| DELETE | /tasks/{id}    | Delete a task          |

`tasks` table: `id, title, description, status, created_at`

---

## 1. Run locally without Docker (requires .NET 8 SDK + MySQL)

```bash
dotnet restore

export DB_HOST=localhost
export DB_PORT=3306
export DB_USER=taskuser
export DB_PASSWORD=taskpassword
export DB_NAME=taskdb

dotnet run
```

Swagger UI: `http://localhost:8000/swagger`

## 2. Run locally with Docker Compose (easier — no local .NET/MySQL needed)

```bash
docker compose up --build -d
docker compose logs -f api
```

API: `http://localhost:8000` · Swagger: `http://localhost:8000/swagger`

### Test the endpoints with curl

```bash
# Create
curl -X POST http://localhost:8000/tasks \
  -H "Content-Type: application/json" \
  -d '{"title": "Buy bread", "description": "Local bakery", "status": "pending"}'

# Get all
curl http://localhost:8000/tasks

# Get one
curl http://localhost:8000/tasks/1

# Update
curl -X PUT http://localhost:8000/tasks/1 \
  -H "Content-Type: application/json" \
  -d '{"status": "done"}'

# Delete
curl -X DELETE http://localhost:8000/tasks/1
```

## 3. GitHub

```bash
git init
git add .
git commit -m "Initial commit: Task Management API (C#/.NET)"
git branch -M main
git remote add origin https://github.com/<USERNAME>/<REPO_NAME>.git
git push -u origin main
```

## 4. Deployment to AWS EC2

Same process regardless of language — Docker makes the platform language-agnostic:

1. **EC2 instance**: Ubuntu 22.04, security group with ports 22 & 8000 open
2. **SSH** into the instance: `ssh -i your-key.pem ubuntu@<EC2_PUBLIC_IP>`
3. **Install Docker**:
   ```bash
   sudo apt-get update -y
   sudo apt-get install -y docker.io docker-compose-plugin git
   sudo systemctl enable docker && sudo systemctl start docker
   sudo usermod -aG docker $USER
   ```
4. **Clone & run**:
   ```bash
   git clone https://github.com/<USERNAME>/<REPO_NAME>.git
   cd <REPO_NAME>
   docker compose up --build -d
   ```
5. **Verify**: `http://<EC2_PUBLIC_IP>:8000/swagger`

## Production notes (suggested next steps)
- Use a managed database (AWS RDS) instead of a MySQL container for production
- Store secrets (DB password) in AWS Secrets Manager instead of plain env vars
- Set up CI/CD with GitHub Actions for auto-deploy on every push to `main`
