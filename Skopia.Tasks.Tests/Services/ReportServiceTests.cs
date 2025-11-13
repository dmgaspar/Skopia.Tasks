using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Skopia.Tasks.Application.Services;
using Skopia.Tasks.Domain.Entities;
using Xunit;

namespace Skopia.Tasks.Tests.Services
{
    public class ReportServiceTests
    {
        // --------------------------------------------------------------
        // GET REPORT — SEM TAREFAS
        // --------------------------------------------------------------
        [Fact]
        public async Task GetPerformanceReportAsync_Deve_Retornar_Vazio_Quando_Nao_Houver_Tarefas()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();

            var service = new ReportService(ctx);

            var result = await service.GetPerformanceReportAsync();

            result.Should().BeEmpty();
        }

        // --------------------------------------------------------------
        // GET REPORT — SOMENTE TAREFAS CONCLUÍDAS SÃO CONTADAS
        // --------------------------------------------------------------
        [Fact]
        public async Task GetPerformanceReportAsync_Deve_Contar_Apenas_Tarefas_Concluidas()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();

            var now = DateTime.UtcNow;

            ctx.Tasks.AddRange(
                new TaskItem
                {
                    ProjectId = 1,
                    Title = "T1",
                    Description = "D1",
                    Status = Domain.Enums.TaskStatus.Concluida,
                    DueDate = now.AddDays(-1)
                },
                new TaskItem
                {
                    ProjectId = 1,
                    Title = "T2",
                    Description = "D2",
                    Status = Domain.Enums.TaskStatus.EmAndamento,
                    DueDate = now.AddDays(-2)
                }
            );

            await ctx.SaveChangesAsync();

            var service = new ReportService(ctx);

            var result = (await service.GetPerformanceReportAsync()).ToList();

            result.Should().HaveCount(1);
            result.First().CompletedTasks.Should().Be(1);
        }

        // --------------------------------------------------------------
        // AGRUPAMENTO POR PROJETO
        // --------------------------------------------------------------
        [Fact]
        public async Task GetPerformanceReportAsync_Deve_Agrupar_Por_Projeto()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();

            var now = DateTime.UtcNow;

            ctx.Tasks.AddRange(
                new TaskItem
                {
                    ProjectId = 10,
                    Title = "T1",
                    Description = "D1",
                    Status = Domain.Enums.TaskStatus.Concluida,
                    DueDate = now.AddDays(-1)
                },
                new TaskItem
                {
                    ProjectId = 10,
                    Title = "T2",
                    Description = "D2",
                    Status = Domain.Enums.TaskStatus.Concluida,
                    DueDate = now.AddDays(-2)
                },
                new TaskItem
                {
                    ProjectId = 20,
                    Title = "T3",
                    Description = "D3",
                    Status = Domain.Enums.TaskStatus.Concluida,
                    DueDate = now.AddDays(-1)
                }
            );

            await ctx.SaveChangesAsync();

            var service = new ReportService(ctx);

            var result = (await service.GetPerformanceReportAsync()).ToList();

            result.Should().HaveCount(2);

            result.Count(r => r.CompletedTasks == 2).Should().Be(1);

            result.Count(r => r.CompletedTasks == 1).Should().Be(1);
        }

        // --------------------------------------------------------------
        // ORDENACAO POR QUANTIDADE DE TAREFAS
        // --------------------------------------------------------------
        [Fact]
        public async Task GetPerformanceReportAsync_Deve_Ordenar_Por_Quantidade_De_Tarefas()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();

            var now = DateTime.UtcNow;

            ctx.Tasks.AddRange(
                new TaskItem
                {
                    ProjectId = 1,
                    Title = "A",
                    Description = "D",
                    Status = Domain.Enums.TaskStatus.Concluida,
                    DueDate = now.AddDays(-1)
                },
                new TaskItem
                {
                    ProjectId = 1,
                    Title = "B",
                    Description = "D",
                    Status = Domain.Enums.TaskStatus.Concluida,
                    DueDate = now.AddDays(-2)
                },
                new TaskItem
                {
                    ProjectId = 2,
                    Title = "C",
                    Description = "D",
                    Status = Domain.Enums.TaskStatus.Concluida,
                    DueDate = now.AddDays(-1)
                }
            );

            await ctx.SaveChangesAsync();

            var service = new ReportService(ctx);

            var result = (await service.GetPerformanceReportAsync()).ToList();

            result.First().CompletedTasks.Should().Be(2);

            result.Last().CompletedTasks.Should().Be(1);
        }

        // --------------------------------------------------------------
        // NÃO INCLUI TAREFAS FORA DO INTERVALO DE 30 DIAS
        // --------------------------------------------------------------
        [Fact]
        public async Task GetPerformanceReportAsync_Deve_Ignorar_Tarefas_Fora_Dos_30_Dias()
        {
            using var ctx = TestHelpers.CreateInMemoryContext();

            var now = DateTime.UtcNow;

            ctx.Tasks.AddRange(
                new TaskItem
                {
                    ProjectId = 1,
                    Title = "T1",
                    Description = "D1",
                    Status = Domain.Enums.TaskStatus.Concluida,
                    DueDate = now.AddDays(-10) // conta
                },
                new TaskItem
                {
                    ProjectId = 1,
                    Title = "T2",
                    Description = "D2",
                    Status = Domain.Enums.TaskStatus.Concluida,
                    DueDate = now.AddDays(-40) // NÃO conta
                }
            );

            await ctx.SaveChangesAsync();

            var service = new ReportService(ctx);

            var result = (await service.GetPerformanceReportAsync()).ToList();

            result.Should().HaveCount(1);
            result.First().CompletedTasks.Should().Be(1);
        }
    }
}
