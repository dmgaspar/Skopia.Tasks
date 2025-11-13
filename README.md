# Skopia.Tasks – API ASP.NET Core 8

Este projeto consiste em uma API para gerenciamento de **Projetos**, **Tarefas**, **Comentários** e **Histórico**.  
A solução segue uma estrutura em camadas (**Domain**, **Application**, **Infrastructure** e **API**), utilizando **Entity Framework Core** com **SQL Server**, migrations e testes unitários.  
Toda a execução foi preparada para rodar também via **Docker**.

---

## Arquitetura da Solução

```text
Skopia.Tasks.sln
│
├── Skopia.Tasks (API)
├── Skopia.Tasks.Application
├── Skopia.Tasks.Domain
└── Skopia.Tasks.Infrastructure
```

A camada Infrastructure contém o AppDbContext, as migrations e as implementações de acesso a dados.

---

## Execução com Docker

Para executar o projeto via Docker e Docker Compose, os arquivos Dockerfile e docker-compose.yml devem estar na raiz da solução.

### 1. Build da imagem da API

```text
docker build -t skopia-tasks-api .
```

Esse passo é opcional se você utilizar apenas o docker-compose.

---

### 2. Subir a API e o SQL Server

```text
docker-compose up -d --build
```

Após iniciar os containers:

API: http://localhost:5001  
Swagger: http://localhost:5001/swagger

---

### 3. Parar os containers

```text
docker-compose down
```

Para remover volumes (limpa o banco de dados):

```text
docker-compose down -v
```

---

### 4. Logs

Logs da API:

```text
docker logs -f skopia-tasks-api
```

Logs do SQL Server:

```text
docker logs -f skopia-sqlserver
```

---

## Migrations dentro do Docker

As migrations são aplicadas automaticamente durante a inicialização, pois o projeto executa:

```text
db.Database.Migrate();
```

Caso seja necessário criar novas migrations, utilize os comandos abaixo no ambiente local (fora do Docker):

```text
dotnet ef migrations add NomeDaMigration   --project Skopia.Tasks.Infrastructure   --startup-project Skopia.Tasks   --context AppDbContext
```

Após criar a migration, basta reconstruir e subir novamente:

```text
docker-compose up -d --build
```

---

## Fase 2 – Refinamento (Perguntas para o PO)

Abaixo estão algumas perguntas que auxiliam no refinamento do backlog e no planejamento das próximas entregas.

### Requisitos e roadmap

- Existe uma lista de funcionalidades planejadas para próximas versões (v2, v3, etc.)?
- Há integrações previstas com outros sistemas internos ou serviços externos?
- Quais funcionalidades são consideradas essenciais para o MVP e quais podem ser postergadas?

### Fluxo de uso

- Como o usuário final deve navegar entre projetos, tarefas, comentários e relatórios?
- Existem regras de SLA, prazos máximos ou notificações automáticas relacionadas às tarefas?
- Quais são os principais cenários de uso que precisam ser contemplados (fluxo principal e fluxos de exceção)?

### Permissões

- Quais perfis de usuário o sistema deve suportar (Admin, Gestor, Operador, Somente leitura)?
- Quais operações cada perfil pode criar, editar, excluir e visualizar?
- Existe necessidade de multi-tenant (várias empresas usando a mesma instância do sistema)?

### Segurança e auditoria

- Quais ações precisam ser auditadas (login, criação/edição/exclusão de dados, mudança de status, etc.)?
- Há exigências específicas de LGPD ou outras normas de compliance?
- Por quanto tempo os registros de auditoria precisam ser mantidos?

### Relatórios e performance

- Quais métricas são mais relevantes para o negócio (tarefas atrasadas, produtividade, tempo médio de conclusão, etc.)?
- Os relatórios devem ser em tempo real ou podem ser consolidados periodicamente (diário, semanal, mensal)?
- Existe necessidade de exportação (CSV, Excel, PDF) ou integração com ferramentas de BI (como Power BI)?

### Funcionalidades pendentes

- O sistema deve suportar anexos (documentos, imagens, evidências)?
- Haverá envio de notificações por e-mail, SMS ou push?
- Existe previsão de painéis/dashboards analíticos com visão consolidada?

---

# Fase 3 – Melhorias Propostas

Aqui estão sugestões claras e objetivas de melhorias com foco em arquitetura, boas práticas, testes, segurança e infraestrutura.

##  Arquitetura

- Migrar de controllers tradicionais para **Minimal APIs** (mais moderno e performático).
- Aplicar **CQRS** para separar consultas de comandos em cenários mais complexos.
- Introduzir **MediatR** para padronizar comunicação interna e reduzir acoplamento.
- Criar camadas independentes com foco em Domain-Driven Design (DDD).

##  Qualidade e Testes

- Aumentar cobertura de testes para acima de **85%** (incluindo testes de integração).
- Adicionar testes de carga e stress para endpoints críticos.
- Configurar pipelines automáticos com GitHub Actions ou Azure DevOps.

## ️ Segurança

- Implementar autenticação JWT.
- Adicionar controle de permissões baseado em roles/perfis.
- Habilitar rate limiting contra ataques de força bruta.
- Configurar HTTPS obrigatório.

## 📊 Observabilidade

- Inserir logs estruturados com Serilog.
- Criar dashboards com Grafana para monitoramento.
- Implementar rastreamento distribuído (OpenTelemetry).

## 🐳 DevOps e Infra

- Criar imagens Docker multi-stage para reduzir tamanho.
- Criar docker-compose para produção (com health checks + restart policies).
- Habilitar migrações automáticas Entity Framework no container.


## Dockerfile

```text
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Skopia.Tasks/Skopia.Tasks.csproj", "Skopia.Tasks/"]
COPY ["Skopia.Tasks.Application/Skopia.Tasks.Application.csproj", "Skopia.Tasks.Application/"]
COPY ["Skopia.Tasks.Domain/Skopia.Tasks.Domain.csproj", "Skopia.Tasks.Domain/"]
COPY ["Skopia.Tasks.Infrastructure/Skopia.Tasks.Infrastructure.csproj", "Skopia.Tasks.Infrastructure/"]

RUN dotnet restore "Skopia.Tasks/Skopia.Tasks.csproj"

COPY . .

WORKDIR "/src/Skopia.Tasks"
RUN dotnet build "Skopia.Tasks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Skopia.Tasks.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Skopia.Tasks.dll"]
```

---

## docker-compose.yml

```text
version: '3.9'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: skopia-sqlserver
    environment:
      - SA_PASSWORD=Your_password123
      - ACCEPT_EULA=Y
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - skopia-sqlserver-data:/var/opt/mssql

  api:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: skopia-tasks-api
    depends_on:
      - sqlserver
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://0.0.0.0:8080
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=TasksDb;User=sa;Password=Your_password123;TrustServerCertificate=true;
    ports:
      - "5001:8080"

volumes:
  skopia-sqlserver-data:
```
## Fase 2 – Refinamento (Perguntas para o PO)

Abaixo estão algumas perguntas que auxiliam no refinamento do backlog e no planejamento das próximas entregas.

### Requisitos e roadmap

- Existe uma lista de funcionalidades planejadas para próximas versões (v2, v3, etc.)?
- Há integrações previstas com outros sistemas internos ou serviços externos?
- Quais funcionalidades são consideradas essenciais para o MVP e quais podem ser postergadas?

### Fluxo de uso

- Como o usuário final deve navegar entre projetos, tarefas, comentários e relatórios?
- Existem regras de SLA, prazos máximos ou notificações automáticas relacionadas às tarefas?
- Quais são os principais cenários de uso que precisam ser contemplados (fluxo principal e fluxos de exceção)?

### Permissões

- Quais perfis de usuário o sistema deve suportar (Admin, Gestor, Operador, Somente leitura)?
- Quais operações cada perfil pode criar, editar, excluir e visualizar?
- Existe necessidade de multi-tenant (várias empresas usando a mesma instância do sistema)?

### Segurança e auditoria

- Quais ações precisam ser auditadas (login, criação/edição/exclusão de dados, mudança de status, etc.)?
- Há exigências específicas de LGPD ou outras normas de compliance?
- Por quanto tempo os registros de auditoria precisam ser mantidos?

### Relatórios e performance

- Quais métricas são mais relevantes para o negócio (tarefas atrasadas, produtividade, tempo médio de conclusão, etc.)?
- Os relatórios devem ser em tempo real ou podem ser consolidados periodicamente (diário, semanal, mensal)?
- Existe necessidade de exportação (CSV, Excel, PDF) ou integração com ferramentas de BI (como Power BI)?

### Funcionalidades pendentes

- O sistema deve suportar anexos (documentos, imagens, evidências)?
- Haverá envio de notificações por e-mail, SMS ou push?
- Existe previsão de painéis/dashboards analíticos com visão consolidada?


---

# Fase 3 – Melhorias Propostas

Aqui estão sugestões claras e objetivas de melhorias com foco em arquitetura, boas práticas, testes, segurança e infraestrutura.

---

##  Arquitetura

- Migrar de controllers tradicionais para **Minimal APIs** (mais moderno e performático).
- Aplicar **CQRS** para separar consultas de comandos em cenários mais complexos.
- Introduzir **MediatR** para padronizar comunicação interna e reduzir acoplamento.
- Criar camadas independentes com foco em Domain-Driven Design (DDD).

##  Qualidade e Testes

- Aumentar cobertura de testes para acima de **85%** (incluindo testes de integração).
- Adicionar testes de carga e stress para endpoints críticos.
- Configurar pipelines automáticos com GitHub Actions ou Azure DevOps.

## ️ Segurança

- Implementar autenticação JWT.
- Adicionar controle de permissões baseado em roles/perfis.
- Habilitar rate limiting contra ataques de força bruta.
- Configurar HTTPS obrigatório.

## Observabilidade

- Inserir logs estruturados com Serilog.
- Criar dashboards com Grafana para monitoramento.
- Implementar rastreamento distribuído (OpenTelemetry).

## DevOps e Infra

- Criar imagens Docker multi-stage para reduzir tamanho.
- Criar docker-compose para produção (com health checks + restart policies).
- Habilitar migrações automáticas Entity Framework no container.

---

## Contribuição

Sugestões de melhorias e correções podem ser enviadas via Pull Request ou registradas como Issues no repositório.
