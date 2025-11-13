using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Services;
using Skopia.Tasks.Domain.Entities;
using Skopia.Tasks.Domain.Enums;
using Skopia.Tasks.Infrastructure.Persistence;
using Xunit;

namespace Skopia.Tasks.Tests.Services
{
    public class ProjectServiceTests
    {
        // --------------------------------------------------------------
        // GET ALL
        // --------------------------------------------------------------
        [Fact]
        public async Task GetAllAsync_Deve_Retornar_Vazio_Quando_Nao_Houver_Projetos()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();
            var service = new ProjectService(ctx);

            var result = await service.GetAllAsync();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_Deve_Retornar_Todos_Projetos()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();
            ctx.Projects.Add(new Project { Name = "P1" });
            ctx.Projects.Add(new Project { Name = "P2" });
            await ctx.SaveChangesAsync();

            var service = new ProjectService(ctx);

            var result = await service.GetAllAsync();

            result.Should().HaveCount(2);
        }

        // --------------------------------------------------------------
        // GET BY ID
        // --------------------------------------------------------------
        [Fact]
        public async Task GetByIdAsync_Deve_Retornar_Null_Quando_Nao_Existir()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();
            var service = new ProjectService(ctx);

            var result = await service.GetByIdAsync(99);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_Deve_Retornar_Projeto_Quando_Existir()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();

            ctx.Projects.Add(new Project { Id = 1, Name = "Teste" });
            await ctx.SaveChangesAsync();

            var service = new ProjectService(ctx);

            var result = await service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Teste");
        }

        // --------------------------------------------------------------
        // CREATE
        // --------------------------------------------------------------
        [Fact]
        public async Task CreateAsync_Deve_Criar_Projeto_Com_Sucesso()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();
            var service = new ProjectService(ctx);

            var dto = new ProjectCreateDto
            {
                Name = "Projeto A",
                Description = "Desc A"
            };

            var result = await service.CreateAsync(dto);

            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Projeto A");

            ctx.Projects.Count().Should().Be(1);
        }

        // --------------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------------
        [Fact]
        public async Task UpdateAsync_Deve_Retornar_Null_Se_Projeto_Nao_Existir()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();
            var service = new ProjectService(ctx);

            var result = await service.UpdateAsync(999, new ProjectUpdateDto());

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Deve_Atualizar_Projeto()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();

            ctx.Projects.Add(new Project { Id = 1, Name = "Old", Description = "Old" });
            await ctx.SaveChangesAsync();

            var service = new ProjectService(ctx);

            var dto = new ProjectUpdateDto { Name = "New", Description = "New Desc" };

            var result = await service.UpdateAsync(1, dto);

            result.Should().NotBeNull();
            result!.Name.Should().Be("New");
            result.Description.Should().Be("New Desc");
        }

        // --------------------------------------------------------------
        // DELETE
        // --------------------------------------------------------------
        [Fact]
        public async Task DeleteAsync_Deve_Retornar_False_Se_Projeto_Nao_Existir()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();
            var service = new ProjectService(ctx);

            var result = await service.DeleteAsync(123);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("não encontrado");
        }

        [Fact]
        public async Task DeleteAsync_Nao_Deve_Excluir_Se_Tiver_Tarefas_Pendentes()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();

            ctx.Projects.Add(new Project
            {
                Id = 1,
                Name = "Teste",
                Tasks = new List<TaskItem>
                {
                    new TaskItem { Title = "T1", Status = Domain.Enums.TaskStatus.Pendente }
                }
            });

            await ctx.SaveChangesAsync();

            var service = new ProjectService(ctx);

            var result = await service.DeleteAsync(1);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("tarefas pendentes");
        }

        [Fact]
        public async Task DeleteAsync_Deve_Excluir_Projeto_Sem_Tarefas_Pendentes()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();

            ctx.Projects.Add(new Project
            {
                Id = 1,
                Name = "Teste",
                Tasks = new List<TaskItem>
                {
                    new TaskItem { Title = "T1", Status = Domain.Enums.TaskStatus.Concluida }
                }
            });

            await ctx.SaveChangesAsync();

            var service = new ProjectService(ctx);

            var result = await service.DeleteAsync(1);

            result.Success.Should().BeTrue();
            ctx.Projects.Count().Should().Be(0);
        }
    }
}
