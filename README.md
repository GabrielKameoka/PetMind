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

Nota Pedagógica: Este projeto não deve ser usado em produção. Ele é um exercício intencionalmente "imperfeito" para fins educacionais.
