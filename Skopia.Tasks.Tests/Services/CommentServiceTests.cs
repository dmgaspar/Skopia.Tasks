using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Exceptions;
using Skopia.Tasks.Application.Services;
using Skopia.Tasks.Domain.Entities;
using Skopia.Tasks.Infrastructure.Persistence;
using Xunit;

namespace Skopia.Tasks.Tests.Services
{
    public class CommentServiceTests
    {
        // ---------------------------------------------------------
        // Criar comentário + registrar histórico
        // ---------------------------------------------------------
        [Fact]
        public async Task Deve_Criar_Comentario_E_Registrar_Historico()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();

            var task = new TaskItem
            {
                Title = "T1",
                Description = "D1",
                Status = Domain.Enums.TaskStatus.Pendente
            };

            ctx.Tasks.Add(task);
            await ctx.SaveChangesAsync();

            var service = new CommentService(ctx);

            // Act
            var result = await service.CreateAsync(task.Id, "Primeiro comentário");

            // Assert
            result.Should().NotBeNull();
            result!.Text.Should().Be("Primeiro comentário");

            ctx.TaskComments.Count().Should().Be(1);

            ctx.TaskHistories.Count().Should().Be(1);

            var history = ctx.TaskHistories.First();

            history.FieldName.Should().Be("Comentário");
            history.OldValue.Should().Be("");
            history.NewValue.Should().Be("Primeiro comentário");
        }

        // ---------------------------------------------------------
        // Tarefa inexistente → NotFoundException
        // ---------------------------------------------------------
        [Fact]
        public async Task Deve_Lancar_NotFound_Se_Tarefa_Nao_Existe()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();
            var service = new CommentService(ctx);

            var act = async () => await service.CreateAsync(999, "Teste");

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Tarefa não encontrada.");
        }

        // ---------------------------------------------------------
        // Atualizar comentário com sucesso
        // ---------------------------------------------------------
        [Fact]
        public async Task Deve_Atualizar_Comentario_Com_Sucesso()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();

            var comment = new TaskComment
            {
                TaskItemId = 1,
                Text = "Original"
            };

            ctx.TaskComments.Add(comment);
            await ctx.SaveChangesAsync();

            var service = new CommentService(ctx);

            // Act
            var result = await service.UpdateAsync(comment.Id, "Alterado");

            result.Should().NotBeNull();
            result!.Text.Should().Be("Alterado");
        }

        // ---------------------------------------------------------
        // Atualizar comentário inexistente → null
        // ---------------------------------------------------------
        [Fact]
        public async Task Deve_Retornar_Null_Ao_Atualizar_Comentario_Inexistente()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();
            var service = new CommentService(ctx);

            var result = await service.UpdateAsync(999, "Novo texto");

            result.Should().BeNull();
        }

        // ---------------------------------------------------------
        // Deletar comentário com sucesso
        // ---------------------------------------------------------
        [Fact]
        public async Task Deve_Deletar_Comentario_Com_Sucesso()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();

            var comment = new TaskComment
            {
                TaskItemId = 1,
                Text = "Teste"
            };

            ctx.TaskComments.Add(comment);
            await ctx.SaveChangesAsync();

            var service = new CommentService(ctx);

            // Act
            var result = await service.DeleteAsync(comment.Id);

            result.Should().BeTrue();
            ctx.TaskComments.Count().Should().Be(0);
        }

        // ---------------------------------------------------------
        // Deletar comentário inexistente → false
        // ---------------------------------------------------------
        [Fact]
        public async Task Deve_Retornar_False_Ao_Deletar_Comentario_Inexistente()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();
            var service = new CommentService(ctx);

            var result = await service.DeleteAsync(999);

            result.Should().BeFalse();
        }
    }
}
