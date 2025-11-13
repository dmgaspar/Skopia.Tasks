using FluentAssertions;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Exceptions;
using Skopia.Tasks.Application.Services;
using Skopia.Tasks.Domain.Entities;
using Xunit;

namespace Skopia.Tasks.Tests.Services;

public class TaskServiceTests
{
    // ------------------------------
    // GET BY PROJECT ID
    // ------------------------------

    [Fact]
    public async Task Deve_Retornar_Tarefas_Do_Projeto()
    {
        using var ctx = TestHelpers.CreateInMemoryContext();

        // Arrange
        var project = new Project { Name = "P1", Description = "Desc" };
        ctx.Projects.Add(project);
        ctx.Tasks.Add(new TaskItem { Title = "T1", ProjectId = project.Id });
        ctx.Tasks.Add(new TaskItem { Title = "T2", ProjectId = project.Id });
        await ctx.SaveChangesAsync();

        var service = new TaskService(ctx);

        // Act
        var result = await service.GetByProjectIdAsync(project.Id);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Deve_Lancar_NotFound_Se_Projeto_Nao_Existe()
    {
        using var ctx = TestHelpers.CreateInMemoryContext();
        var service = new TaskService(ctx);

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetByProjectIdAsync(999)
        );
    }

    [Fact]
    public async Task Deve_Criar_Tarefa_Com_Sucesso()
    {
        using var ctx = TestHelpers.CreateInMemoryContext();

        var project = new Project { Name = "P1", Description = "D" };
        ctx.Projects.Add(project);
        await ctx.SaveChangesAsync();

        var service = new TaskService(ctx);

        var dto = new TaskCreateDto
        {
            Title = "Task 1",
            Description = "Desc",
            Priority = "Alta",      
            Status = "Pendente",    
                                    
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var result = await service.CreateAsync(project.Id, dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("Task 1");
        result.Priority.Should().Be("Alta");
        result.Status.Should().Be("Pendente");
    }

    [Fact]
    public async Task CreateAsync_Deve_Lancar_NotFound_Se_Projeto_Inexistente()
    {
        using var ctx = TestHelpers.CreateInMemoryContext();
        var service = new TaskService(ctx);

        var dto = new TaskCreateDto
        {
            Title = "T1",
            Priority = "High",
            Status = "Open"
        };

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.CreateAsync(999, dto)
        );
    }

    [Fact]
    public async Task CreateAsync_Deve_Lancar_Excessao_Se_Tiver_Mais_De_20_Tarefas()
    {
        using var ctx = TestHelpers.CreateInMemoryContext();

        var project = new Project { Name = "P1" };
        ctx.Projects.Add(project);
        await ctx.SaveChangesAsync();

        // Cria 20 tarefas
        for (int i = 0; i < 20; i++)
            ctx.Tasks.Add(new TaskItem { ProjectId = project.Id, Title = $"T{i}" });

        await ctx.SaveChangesAsync();

        var service = new TaskService(ctx);

        var dto = new TaskCreateDto
        {
            Title = "Overflow",
            Priority = "High",
            Status = "Open"
        };

        await Assert.ThrowsAsync<BusinessException>(
            () => service.CreateAsync(project.Id, dto)
        );
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Para_Prioridade_Invalida()
    {
        using var ctx = TestHelpers.CreateInMemoryContext();

        var project = new Project { Name = "P1" };
        ctx.Projects.Add(project);
        await ctx.SaveChangesAsync();

        var service = new TaskService(ctx);

        var dto = new TaskCreateDto
        {
            Title = "Task",
            Priority = "INVALIDO",
            Status = "Open"
        };

        await Assert.ThrowsAsync<BusinessException>(
            () => service.CreateAsync(project.Id, dto)
        );
    }

    [Fact]
    public async Task Deve_Atualizar_Tarefa_E_Registrar_Historico()
    {
        using var ctx = TestHelpers.CreateInMemoryContext();

        var project = new Project { Name = "P1" };
        var task = new TaskItem
        {
            Title = "T1",
            Description = "D1",
            Status = Skopia.Tasks.Domain.Enums.TaskStatus.EmAndamento,
            DueDate = new DateTime(2025, 1, 1),
            Project = project
        };

        ctx.Projects.Add(project);
        ctx.Tasks.Add(task);
        await ctx.SaveChangesAsync();

        var service = new TaskService(ctx);

        var dto = new TaskUpdateDto
        {
            Title = "T2",
            Description = "D2",
            Status = "Concluida",
            DueDate = new DateTime(2025, 1, 1)
        };

        // Act
        var result = await service.UpdateAsync(project.Id, task.Id, dto);

        // Assert
        result.Title.Should().Be("T2");
        ctx.TaskHistories.Count().Should().Be(3); // Title, Description, Status
    }

    [Fact]
    public async Task UpdateAsync_Deve_Lancar_NotFound_Se_Projeto_Nao_Existe()
    {
        using var ctx = TestHelpers.CreateInMemoryContext();
        var service = new TaskService(ctx);

        var dto = new TaskUpdateDto
        {
            Title = "T1",
            Description = "D",
            Status = "EmAndamento"
        };

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateAsync(999, 1, dto)
        );
    }

    [Fact]
    public async Task Deve_Deletar_Tarefa()
    {
        using var ctx = TestHelpers.CreateInMemoryContext();

        var task = new TaskItem { Title = "T1" };
        ctx.Tasks.Add(task);
        await ctx.SaveChangesAsync();

        var service = new TaskService(ctx);

        await service.DeleteAsync(task.Id);

        ctx.Tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_Deve_Lancar_NotFound_Se_Id_Inexistente()
    {
        using var ctx = TestHelpers.CreateInMemoryContext();
        var service = new TaskService(ctx);

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(999));
    }



}
