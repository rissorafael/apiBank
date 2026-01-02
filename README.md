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
);
