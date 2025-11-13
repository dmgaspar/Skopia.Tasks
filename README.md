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

A camada Infrastructure contém o AppDbContext, as migrations e as implementações de acesso a dados.

## Execução com Docker

Para executar o projeto via Docker e Docker Compose, os arquivos Dockerfile e docker-compose.yml devem estar na raiz da solução.
