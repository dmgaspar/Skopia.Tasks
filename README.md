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

## Fase 3 – Possíveis Melhorias e Evolução da Arquitetura

Abaixo estão algumas sugestões de melhorias e evoluções para o projeto, considerando um cenário de crescimento e maturidade da solução.

### Estrutura e arquitetura

- Evoluir para uma abordagem mais aderente a **Clean Architecture**, enfatizando a separação entre camadas.
- Refinar os casos de uso na camada `Application`, tornando as regras de negócio mais explícitas.
- Manter a camada de domínio (`Domain`) completamente independente da infraestrutura (DB, frameworks, etc.).

### Microsserviços

- Separar os contextos principais em serviços independentes, por exemplo:
  - Projects Service
  - Tasks Service
  - Reporting Service
- Cada serviço com seu próprio banco de dados (database per service).
- Comunicação assíncrona entre serviços utilizando eventos de domínio (ex.: `TaskCreated`, `TaskCompleted`).

### Observabilidade

- Adicionar logging estruturado (por exemplo, com Serilog).
- Configurar métricas e tracing distribuído com OpenTelemetry.
- Utilizar Prometheus + Grafana ou Application Insights para monitoramento e visualização.

### Mensageria

- Utilizar RabbitMQ ou Azure Service Bus para processamento assíncrono de tarefas e integração entre serviços.
- Exemplos de uso:
  - Envio de notificações
  - Atualização de índices de busca
  - Geração de relatórios em background

### Cache

- Utilizar Redis para cachear consultas mais pesadas (relatórios, dashboards, listagens de grande volume).
- Adotar o padrão Cache-Aside, com invalidação de cache durante operações de escrita.

### CI/CD

- Criar pipelines no GitHub Actions (ou ferramenta equivalente) para:
  - Build da solução
  - Execução dos testes unitários
  - Build e publicação das imagens Docker
  - Deploy automatizado para ambientes de desenvolvimento/homologação/produção

### Deploy em cloud

- Publicar a API em Azure App Service, Azure Container Apps, AWS ECS/EKS ou outra plataforma de containers.
- Utilizar um banco de dados gerenciado, como Azure SQL ou AWS RDS, para simplificar operações de infraestrutura.

### Padrões de projeto

- Aplicar **CQRS** para separar comandos (escrita) de consultas (leitura), especialmente em relatórios e painéis.
- Utilizar **Outbox Pattern** para garantir consistência entre transações no banco de dados e publicação de eventos.
- Empregar padrões como **Strategy** e **Factory** para encapsular regras de negócio específicas que podem variar conforme o tipo de projeto, prioridade ou configuração.

---

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

---

## Contribuição

Sugestões de melhorias e correções podem ser enviadas via Pull Request ou registradas como Issues no repositório.
