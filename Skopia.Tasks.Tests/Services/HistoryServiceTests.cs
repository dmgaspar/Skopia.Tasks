using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Skopia.Tasks.Application.Services;
using Skopia.Tasks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Infrastructure.Persistence;

namespace Skopia.Tasks.Tests.Services
{
    public class HistoryServiceTests
    {

        // --------------------------------------------------------------
        // GET ALL
        // --------------------------------------------------------------
        [Fact]
        public async Task GetAllAsync_Deve_Retornar_Todos_Historicos()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();;

            ctx.TaskHistories.AddRange(
                new TaskHistory { TaskItemId = 1, FieldName = "Title", OldValue = "A", NewValue = "B", ChangedAt = DateTime.UtcNow },
                new TaskHistory { TaskItemId = 2, FieldName = "Status", OldValue = "X", NewValue = "Y", ChangedAt = DateTime.UtcNow }
            );
            await ctx.SaveChangesAsync();

            var service = new HistoryService(ctx);

            var result = await service.GetAllAsync();

            result.Should().HaveCount(2);
        }

        // --------------------------------------------------------------
        // GET BY TASK
        // --------------------------------------------------------------
        [Fact]
        public async Task GetByTaskAsync_Deve_Retornar_Apenas_Da_Tarefa_Especificada()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();;

            ctx.TaskHistories.AddRange(
                new TaskHistory { TaskItemId = 10, FieldName = "Title", OldValue = "A", NewValue = "B", ChangedAt = DateTime.UtcNow },
                new TaskHistory { TaskItemId = 99, FieldName = "Status", OldValue = "X", NewValue = "Y", ChangedAt = DateTime.UtcNow }
            );
            await ctx.SaveChangesAsync();

            var service = new HistoryService(ctx);

            var result = await service.GetByTaskAsync(10);

            result.Should().HaveCount(1);
            result[0].TaskItemId.Should().Be(10);
        }

        // --------------------------------------------------------------
        // GET BY TASK - NOT FOUND
        // --------------------------------------------------------------
        [Fact]
        public async Task GetByTaskAsync_Deve_Retornar_Vazio_Quando_Nao_Houver_Historico()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();;

            var service = new HistoryService(ctx);

            var result = await service.GetByTaskAsync(123);

            result.Should().BeEmpty();
        }
    }
}
