using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Services;
using Skopia.Tasks.Infrastructure.Persistence;
using Xunit;

namespace Skopia.Tasks.Tests.Services
{
    public class ProjectServiceTests
    {
        // Cria um DbContext em memória para cada teste
        private static AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"tasksdb_{System.Guid.NewGuid()}")
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task Deve_Criar_Projeto_Com_Sucesso()
        {
            // Arrange
            using var ctx = CreateInMemoryContext();
            var service = new ProjectService(ctx);

            var dto = new ProjectCreateDto
            {
                Name = "Projeto A",
                Description = "Descrição A"
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert - resultado da service
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Projeto A");
            result.Description.Should().Be("Descrição A");

            // Assert extra – conferir se foi salvo corretamente no banco em memória
            var projectInDb = await ctx.Projects.SingleAsync();
            projectInDb.Name.Should().Be(dto.Name);
            projectInDb.Description.Should().Be(dto.Description);
        }
    }
}
