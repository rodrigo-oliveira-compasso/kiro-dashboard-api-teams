# Kiro Dashboard - API Teams

API de gestão de profissionais e squads. Responsável pelo cadastro, edição e organização dos profissionais em squads.

## Visão Geral

Gerencia os profissionais (usuários Kiro com AWS User ID) e squads (grupos/equipes). O vínculo entre profissional e squad é N:N. Os dados aqui cadastrados são usados pela API Dashboard para enriquecer os dados de uso com nomes e agrupamentos.

## Funcionalidades

- **CRUD de Profissionais** — Cadastro com nome, email e AWS User ID (GUID do Kiro)
- **CRUD de Squads** — Cadastro com nome e descrição
- **Associação N:N** — Adicionar/remover profissionais de squads
- **Paginação server-side** — Com busca e ordenação
- **Validação de duplicidade** — AWS User ID e email únicos para profissionais, nome único para squads

## Stack Tecnológica

- .NET 10 (ASP.NET Core)
- Entity Framework Core 9 + PostgreSQL (Npgsql)
- JWT Bearer Authentication
- Scalar (documentação OpenAPI)

## Pré-requisitos

- .NET SDK 10
- PostgreSQL rodando na porta 5433

## Como Rodar

```bash
dotnet run --urls "http://localhost:5074"
```

A API estará disponível em `http://localhost:5074`.  
Documentação Scalar: `http://localhost:5074/scalar`

## Configuração

### appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=kiro_dashboard;Username=postgres;Password=SUA_SENHA"
  },
  "Jwt": {
    "SecretKey": "SUA_CHAVE_SECRETA_MINIMO_32_CHARS",
    "Issuer": "KiroDashboard",
    "Audience": "KiroDashboardUsers"
  }
}
```

## Endpoints

### Profissionais

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/professionals` | Listar todos |
| GET | `/api/professionals/paginated` | Listar paginado |
| GET | `/api/professionals/{id}` | Buscar por ID |
| POST | `/api/professionals` | Criar |
| PUT | `/api/professionals/{id}` | Editar |
| DELETE | `/api/professionals/{id}` | Excluir |

### Squads

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/squads` | Listar todos |
| GET | `/api/squads/paginated` | Listar paginado |
| GET | `/api/squads/{id}` | Buscar por ID |
| POST | `/api/squads` | Criar |
| PUT | `/api/squads/{id}` | Editar |
| DELETE | `/api/squads/{id}` | Excluir |
| POST | `/api/squads/{squadId}/professionals/{professionalId}` | Adicionar profissional |
| DELETE | `/api/squads/{squadId}/professionals/{professionalId}` | Remover profissional |

Todos os endpoints requerem autenticação com role Admin.
