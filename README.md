# Check-in Project

Plataforma fullstack para registro de check-in e check-out de funcionarios, com painel de gestor, listagem paginada e integracao opcional com RabbitMQ para publicar eventos de jornada.

## Stack
- Backend: ASP.NET Core 8, Entity Framework Core (PostgreSQL), RabbitMQ.Client, Swagger
- Frontend: React 19 + TypeScript + Vite, Axios
- Infra: PostgreSQL, RabbitMQ, Docker Compose

## Estrutura de pastas
- `checkin-project-backend/`: API REST (`/auth`, `/work`, `/dev/seed`), contexto EF, repositorios e publishers.
- `checkin-project-frontend/`: SPA React com login, dashboard do funcionario e dashboard do gestor.
- `docker-compose.yml`: sobe Postgres, RabbitMQ, backend e frontend para ambiente local completo.

## Funcionalidades
- Login com dois perfis de teste (gestor e funcionario).
- Check-in e check-out por funcionario; calculo de horas trabalhadas.
- Listagem filtrada por nome e data, com paginacao para gestores.
- Publicacao de eventos de trabalho em fila RabbitMQ (com fallback no-op caso o broker esteja indisponivel).
- Swagger UI em `/swagger` e health check em `/health`.

## Como rodar com Docker (recomendado)
1) Pre-requisitos: Docker e Docker Compose.
2) Suba tudo: `docker-compose up --build`
3) Enderecos:
   - Frontend: http://localhost:5173
   - Backend: http://localhost:5204 (Swagger em `/swagger`)
4) Popular dados de teste (cria gestor e funcionario):
   - `curl -X POST http://localhost:5204/dev/seed`
5) Credenciais de teste:
   - gestor@empresa.com / 123456
   - funcionario@empresa.com / 123456

## Como rodar localmente (sem Docker)
### Backend
1) Pre-requisitos: .NET 8, PostgreSQL rodando, opcional RabbitMQ.
2) Configure variaveis de ambiente ou `appsettings.*`:
   - `ConnectionStrings__Default`: ex. `Host=localhost;Port=5432;Database=checkin_db;Username=postgres;Password=senha`
   - `RabbitMq__HostName`, `RabbitMq__UserName`, `RabbitMq__Password`, `RabbitMq__QueueName`
3) Restaurar e executar:
   - `cd checkin-project-backend`
   - `dotnet restore`
   - `dotnet run`
4) (Opcional) Semear base para testes: `POST /dev/seed`
5) Observacao: se o RabbitMQ nao estiver acessivel, o backend registra aviso e usa `NoopWorkEventPublisher`, permitindo seguir sem eventos.

### Frontend
1) Pre-requisitos: Node 18+ e npm.
2) Configurar `.env`:
   - `VITE_API_BASE_URL=http://localhost:5204` (ou URL publicada)
3) Instalar e rodar:
   - `cd checkin-project-frontend`
   - `npm install`
   - `npm run dev` (ou `npm run build` e `npm run preview`)

## API (resumo)
- `POST /auth/login` — body: `{ email, password }` — devolve `employeeId`, `name`, `role`.
- `POST /work/checkin` — body: `{ employeeId }` — cria registro aberto.
- `POST /work/checkout` — body: `{ employeeId }` — fecha o ultimo registro aberto do dia.
- `GET /work/list` — query: `name`, `date` (YYYY-MM-DD), `page`, `pageSize` — lista registros com paginacao.
- `POST /dev/seed` — cria gestor e funcionario de teste (somente para ambiente de desenvolvimento).
- `GET /health` — status simples.

## CORS e origens permitidas
Configurado em `Program.cs` para aceitar:
- `http://localhost:5173`
- `http://127.0.0.1:5173`
- `https://checkin-project-ashy.vercel.app`
Adicione novas origens conforme necessario.

## Scripts uteis
- Backend: `dotnet run` para desenvolvimento.
- Frontend: `npm run dev`, `npm run build`, `npm run preview`, `npm run lint`.

## Observacoes importantes
- Autenticacao usa credenciais fixas de teste; o e-mail precisa existir na tabela `employees` (use `/dev/seed`).
- Banco: tabelas `employees` e `work_records` criadas automaticamente (`EnsureCreated`).
- Eventos: se RabbitMQ estiver configurado, os eventos sao publicados na fila definida em `RabbitMq__QueueName`.
