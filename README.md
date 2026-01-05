# BancoChu – Manual para Executar o Projeto

Este documento descreve **passo a passo** como executar o projeto **BancoChu** localmente utilizando **Docker Compose**, bem como **criar as tabelas no PostgreSQL** e entender rapidamente os **endpoints disponíveis**.

---

## 📌 Pré-requisitos

Antes de começar, certifique-se de ter instalado na máquina:

- **Docker** (>= 24)
- **Docker Compose** (>= v2)
- **.NET SDK** compatível com o projeto (ex: .NET 8 / 9 / 10, conforme definido no `.csproj`)

Verifique a instalação:

```bash
docker --version
docker compose version
dotnet --version
```

---

## 📁 Estrutura do Projeto (resumo)

```
apiBank/
│
├─ docker-compose.yml
├─ README.md
├─ src/
│  └─ BancoChu.Api
│     └─ Program.cs
│
└─ ...
```

> ⚠️ O arquivo `docker-compose.yml` está localizado **na raiz do projeto `apiBank`**.

---

## 🚀 Passo 1 – Subir a infraestrutura com Docker Compose

Na raiz do projeto (`apiBank`), execute:

```bash
docker compose up -d
```

Isso irá subir os seguintes serviços:

| Serviço   | Descrição | Porta |
|---------|----------|------|
| PostgreSQL | Banco de dados relacional | 5432 |
| Redis | Cache (ex.: dias úteis) | 6379 |

Verifique se os containers estão rodando:

```bash
docker ps
```

Você deve ver:

- `bank_postgres`
- `redis-cache`

---

## 🐘 Passo 2 – Acessar o PostgreSQL

### Credenciais configuradas no Docker Compose

- **Host:** localhost
- **Porta:** 5432
- **Database:** bank_db
- **Usuário:** bank_user
- **Senha:** bank_password

### Acessar via terminal

```bash
docker exec -it bank_postgres psql -U bank_user -d bank_db
```

---

## 🧩 Passo 3 – Habilitar extensão necessária

Antes de criar as tabelas, execute:

```sql
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
```

Essa extensão é necessária para o uso da função `gen_random_uuid()`.

---

## 🗄️ Passo 4 – Criar as tabelas

Execute os scripts abaixo **na ordem**.

### 1️⃣ Tabela `users`

```sql
CREATE TABLE users (
    id UUID PRIMARY KEY,
    email VARCHAR(150) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    status INT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);
```

---

### 2️⃣ Tabela `bank_accounts`

```sql
CREATE TABLE bank_accounts (
    id UUID PRIMARY KEY,
    account_number VARCHAR(20) NOT NULL UNIQUE,
    agency VARCHAR(10) NOT NULL,
    user_id UUID NOT NULL,
    balance NUMERIC(18,2) NOT NULL DEFAULT 0,
    status INT NOT NULL,
    type INT NOT NULL,
    created_at TIMESTAMP NOT NULL,

    CONSTRAINT fk_bank_accounts_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
);
```


### 3️⃣ Tabela `bank_transfers`

```sql
CREATE TABLE bank_transfers (
    transfer_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    origin_account_id UUID NOT NULL,
    destination_account_id UUID NOT NULL,
    amount NUMERIC(15,2) NOT NULL,
    transfer_date TIMESTAMP NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    status VARCHAR(20) NOT NULL,

    CONSTRAINT fk_transfer_origin_account
        FOREIGN KEY (origin_account_id)
        REFERENCES bank_accounts(id),

    CONSTRAINT fk_transfer_destination_account
        FOREIGN KEY (destination_account_id)
        REFERENCES bank_accounts(id)
);
```

---

## ▶️ Passo 5 – Executar a API

A partir da pasta do projeto da API:

```bash
dotnet restore
dotnet run
```

A API ficará disponível em:

```
https://localhost:5001
http://localhost:5000
```

(As portas podem variar conforme configuração do projeto.)

---

## 🔐 Autenticação

Todos os endpoints do `AccountsController` estão protegidos por:

```csharp
[Authorize]
```

➡️ É necessário obter um **token JWT válido** para acessar as rotas.

---

## 📚 Endpoints Disponíveis

### 👤 UsersController

Base route: `api/v{version:apiVersion}/users`

#### 🔹 Criar Usuário

**POST** `api/v{version}/users`

- Cria um novo usuário no sistema
- Endpoint público (`[AllowAnonymous]`)

**Entrada:**
- Corpo da requisição com `CreateUserRequestDto`

**Comportamento:**
- Chama `IUsersApplication.CreateAsync(CreateUserRequestDto)`
- Persiste o usuário no banco de dados

**Autorização:**
- Não requer autenticação

**Respostas:**
- `200 OK` – Usuário criado com sucesso
- `500 Internal Server Error` – Erro interno ao criar o usuário

---

## 📚 Endpoints Disponíveis – AccountsController

### 🔹 Consultar Saldo da Conta

**GET** `api/v{version}/accounts/{accountId}/balance`

- Consulta o saldo atual de uma conta bancária
- O usuário deve estar autenticado
- O saldo só pode ser consultado pelo dono da conta
- O userId é obtido automaticamente a partir do token JWT

**Respostas:**
- `200 OK`
- `400 Bad Request`
- `500 Internal Server Error`

### 🔹 Criar Conta

**POST** `api/v{version}/accounts`

- Cria uma nova conta bancária
- Valida se o usuário já possui conta do mesmo tipo

**Respostas:**
- `201 Created`
- `400 Bad Request`
- `500 Internal Server Error`

---

### 🔹 Transferência

**POST** `api/v{version}/accounts/{accountId}/transfer`

- Executa transferência entre contas
- Apenas em dias úteis
- Apenas transferencia do usuario logado
- Usa transação manual (commit/rollback)

**Respostas:**
- `201 Created`
- `400 Bad Request`
- `500 Internal Server Error`

---

### 🔹 Extrato

**GET** `api/v{version}/accounts/{accountId}/statement?startDate=&endDate=`

- Retorna extrato no período
- Débitos retornam como valores negativos
- Apenas extrato do usuario logado

**Respostas:**
- `200 OK`
- `400 Bad Request`
- `500 Internal Server Error`

---

## 🧹 Encerrar os containers

```bash
docker compose down
```

Para remover também os volumes:

```bash
docker compose down -v
```

---

## ✅ Conclusão

Seguindo este manual, é possível:

- Subir toda a infraestrutura via Docker
- Criar o banco e tabelas manualmente
- Executar a API localmente
- Testar os endpoints protegidos

Este documento deve ser utilizado como **guia oficial de execução do projeto**.
