using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Skopia.Tasks.Controllers;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Interfaces;

namespace Skopia.Tasks.Tests.Controllers
{
    public class ProjectControllerTests
    {
        private readonly Mock<IProjectService> _mockService;
        private readonly ProjectController _controller;

        public ProjectControllerTests()
        {
            _mockService = new Mock<IProjectService>();
            _controller = new ProjectController(_mockService.Object);
        }

        // --------------------------------------------------------------
        // GET ALL
        // --------------------------------------------------------------
        [Fact]
        public async Task GetAllAsync_Deve_Retornar_Lista_Vazia_Quando_Nao_Existir()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<ProjectDto>());

            // Act
            var response = await _controller.GetAllAsync() as OkObjectResult;

            // Assert
            response.Should().NotBeNull();
            var data = response.Value as IEnumerable<ProjectDto>;
            data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_Deve_Retornar_Lista_De_Projetos()
        {
            // Arrange
            var items = new List<ProjectDto>
            {
                new ProjectDto { Id = 1, Name = "Teste", Description = "Desc" }
            };

            _mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(items);

            // Act
            var response = await _controller.GetAllAsync() as OkObjectResult;

            // Assert
            response.Should().NotBeNull();
            var data = response.Value as IEnumerable<ProjectDto>;
            data.Should().HaveCount(1);
        }

        // --------------------------------------------------------------
        // GET BY ID
        // --------------------------------------------------------------
        [Fact]
        public async Task GetByIdAsync_Deve_Retornar_NotFound_Quando_Projeto_Nao_Existir()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync((ProjectDto?)null);

            // Act
            var response = await _controller.GetByIdAsync(1);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetByIdAsync_Deve_Retornar_Projeto_Quando_Existir()
        {
            // Arrange
            var dto = new ProjectDto { Id = 1, Name = "Teste", Description = "Desc" };

            _mockService.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(dto);

            // Act
            var response = await _controller.GetByIdAsync(1) as OkObjectResult;

            // Assert
            response.Should().NotBeNull();
            var data = response.Value as ProjectDto;
            data.Id.Should().Be(1);
        }

        // --------------------------------------------------------------
        // CREATE
        // --------------------------------------------------------------
        [Fact]
        public async Task CreateAsync_Deve_Retornar_Created_Com_Location_Correto()
        {
            // Arrange
            var input = new ProjectCreateDto { Name = "P1", Description = "D1" };

            var created = new ProjectDto
            {
                Id = 10,
                Name = "P1",
                Description = "D1"
            };

            _mockService.Setup(s => s.CreateAsync(input))
                .ReturnsAsync(created);

            // Act
            var response = await _controller.CreateAsync(input) as CreatedResult;

            // Assert
            response.Should().NotBeNull();
            response.Location.Should().Be("api/Project/10");
            response.Value.Should().Be(created);
        }

        // --------------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------------
        [Fact]
        public async Task UpdateAsync_Deve_Retornar_NotFound_Quando_Projeto_Nao_Existir()
        {
            // Arrange
            var updateDto = new ProjectUpdateDto { Name = "Novo", Description = "Desc" };

            _mockService.Setup(s => s.UpdateAsync(5, updateDto))
                .ReturnsAsync((ProjectDto?)null);

            // Act
            var response = await _controller.UpdateAsync(5, updateDto);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateAsync_Deve_Retornar_Projeto_Atualizado()
        {
            // Arrange
            var updateDto = new ProjectUpdateDto { Name = "Novo", Description = "Desc" };

            var updated = new ProjectDto
            {
                Id = 5,
                Name = "Novo",
                Description = "Desc"
            };

            _mockService.Setup(s => s.UpdateAsync(5, updateDto))
                .ReturnsAsync(updated);

            // Act
            var response = await _controller.UpdateAsync(5, updateDto) as OkObjectResult;

            // Assert
            response.Should().NotBeNull();
            response.Value.Should().Be(updated);
        }

        // --------------------------------------------------------------
        // DELETE
        // --------------------------------------------------------------
        [Fact]
        public async Task DeleteAsync_Deve_Retornar_BadRequest_Quando_Servico_Falhar()
        {
            // Arrange
            var deleteResult = new DeleteResultDto
            {
                Success = false,
                Message = "Erro"
            };

            _mockService.Setup(s => s.DeleteAsync(3))
                .ReturnsAsync(deleteResult);

            // Act
            var response = await _controller.DeleteAsync(3) as BadRequestObjectResult;

            // Assert
            response.Should().NotBeNull();
            response.Value.Should().Be("Erro");
        }

        [Fact]
        public async Task DeleteAsync_Deve_Retornar_NoContent_Quando_Sucesso()
        {
            // Arrange
            var deleteResult = new DeleteResultDto
            {
                Success = true,
                Message = ""
            };

            _mockService.Setup(s => s.DeleteAsync(3))
                .ReturnsAsync(deleteResult);

            // Act
            var response = await _controller.DeleteAsync(3);

            // Assert
            response.Should().BeOfType<NoContentResult>();
        }
    }
}
