# 📋 Sobre o Projeto
Esta é uma API RESTful monolítica desenvolvida como base para um futuro refatoramento em Arquitetura Limpa. O sistema gerencia operações de petshops, incluindo cadastro de pets, agendamentos, autenticação e dashboard.

### Nota Importante: Esta versão é intencionalmente NÃO seguindo Arquitetura Limpa. Ela serve como:
- Ponto de partida para aprendizado
- Baseline para refatoramento futuro
- Exemplo didático de evolução arquitetural

### Objetivos Pedagógicos
Este projeto foi criado para demonstrar:
- Evolução Gradual conforme a leitura do livro Arquitetura Limpa (Uncle Bob)
- Evolução Arquitetural: Como migrar de monolito para arquitetura limpa
- Refatoramento Passo a Passo: Identificar pontos de melhoria
- Boas Práticas Progressivas: Implementar melhorias gradualmente
- Code Smells: Exemplos comuns em aplicações monolíticas

### Problemas Conhecidos (Intencionais)
- Violação do SRP: Controllers com múltiplas responsabilidades
- Acoplamento Forte: Dependência direta do EF Core
- Ausência de Testabilidade: Difícil mockar dependências
- Lógica Espalhada: Regras de negócio em vários lugares
- Sem Abstrações: Implementações concretas em todo lugar

## Endpoints da API

### Autenticação (`/api/auth`)
| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| `POST` | `/login` | Login com email e senha | ❌ Público |
| `POST` | `/refresh` | Renovar tokens usando refresh token | ❌ Público |
| `POST` | `/revoke` | Revogar refresh token | ✅ JWT |
| `POST` | `/logout` | Logout e limpar tokens | ✅ JWT |
| `GET` | `/validate` | Validar token atual | ✅ JWT |
| `GET` | `/debug` | Debug de claims do token | ✅ JWT |

### Petshops (`/api/petshops`)
| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| `GET` | `/` | Listar todos petshops | ✅ JWT |
| `GET` | `/{id}` | Obter petshop por ID | ✅ JWT |
| `GET` | `/{id}/dashboard` | Dashboard com estatísticas | ✅ JWT |
| `POST` | `/` | Criar novo petshop | ❌ Público |
| `PUT` | `/{id}` | Atualizar petshop | ✅ JWT |
| `DELETE` | `/{id}` | Excluir petshop | ✅ JWT |

### Cachorros (`/api/cachorros`)
| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| `GET` | `/` | Listar todos os cachorros do petshop logado | ✅ JWT |
| `GET` | `/{id}` | Obter cachorro por ID | ✅ JWT |
| `GET` | `/meus` | Listar cachorros do petshop logado | ✅ JWT |
| `GET` | `/por-raca?raca={nome}` | Filtrar cachorros por raça | ✅ JWT |
| `GET` | `/por-porte?porte={tamanho}` | Filtrar cachorros por porte | ✅ JWT |
| `GET` | `/estatisticas` | Estatísticas dos cachorros | ✅ JWT |
| `POST` | `/` | Cadastrar novo cachorro | ✅ JWT |
| `PUT` | `/{id}` | Atualizar cachorro | ✅ JWT |
| `DELETE` | `/{id}` | Excluir cachorro | ✅ JWT |

### Horários (`/api/horarios`)
| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| `GET` | `/` | Listar horários do petshop logado (com paginação) | ✅ JWT |
| `GET` | `/{id}` | Obter horário por ID | ✅ JWT |
| `GET` | `/por-data?data={data}` | Buscar horários por data específica | ✅ JWT |
| `POST` | `/` | Agendar novo horário | ✅ JWT |
| `PUT` | `/{id}` | Atualizar horário | ✅ JWT |
| `DELETE` | `/{id}` | Cancelar/excluir horário | ✅ JWT |

Nota Pedagógica: Este projeto não deve ser usado em produção. Ele é um exercício intencionalmente "imperfeito" para fins educacionais.
