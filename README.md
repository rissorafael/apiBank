# BancoChu - Estrutura do Banco de Dados

Este projeto possui as seguintes tabelas principais:

---

## 1. Tabela `users`

Armazena os usuários do sistema.

| Coluna      | Tipo      | Obrigatório | Descrição                            |
|------------|----------|------------|--------------------------------------|
| id         | UUID     | Sim        | Identificador único do usuário       |
| email      | VARCHAR(150) | Sim    | E-mail do usuário, único             |
| password   | VARCHAR(255) | Sim    | Senha do usuário                     |
| status     | INT      | Sim        | Status do usuário (1=Ativo, 2=Bloqueado) |
| created_at | TIMESTAMP | Sim       | Data de criação (padrão NOW())       |

**SQL de criação:**

```sql
CREATE TABLE users (
    id UUID PRIMARY KEY,
    email VARCHAR(150) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    status INT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);```

## 2. Tabela bank_accounts

| Coluna         | Tipo          | Obrigatório | Descrição                              |
| -------------- | ------------- | ----------- | -------------------------------------- |
| id             | UUID          | Sim         | Identificador único da conta           |
| account_number | VARCHAR(20)   | Sim         | Número da conta (único)                |
| agency         | VARCHAR(10)   | Sim         | Agência da conta                       |
| user_id        | UUID          | Sim         | Identificador do usuário dono da conta |
| balance        | NUMERIC(18,2) | Sim         | Saldo da conta                         |
| status         | INT           | Sim         | Status da conta                        |
| type           | INT           | Sim         | Tipo da conta                          |
| created_at     | TIMESTAMP     | Sim         | Data de criação da conta               |

**SQL de criação:**

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
);```

## 3. Tabela bank_transfers

| Coluna                 | Tipo          | Obrigatório | Descrição                            |
| ---------------------- | ------------- | ----------- | ------------------------------------ |
| transfer_id            | UUID          | Sim         | Identificador único da transferência |
| origin_account_id      | UUID          | Sim         | Conta de origem da transferência     |
| destination_account_id | UUID          | Sim         | Conta de destino da transferência    |
| amount                 | NUMERIC(15,2) | Sim         | Valor da transferência               |
| transfer_date          | TIMESTAMP     | Sim         | Data e hora da transferência         |
| created_at             | TIMESTAMP     | Sim         | Data de criação do registro          |
| status                 | VARCHAR(20)   | Sim         | Status da transferência              |

**SQL de criação:**

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

--Antes de criar as tabelas, execute:
--CREATE EXTENSION IF NOT EXISTS "pgcrypto";
--Essa extensão é necessária para o uso da função:
--gen_random_uuid()

```
