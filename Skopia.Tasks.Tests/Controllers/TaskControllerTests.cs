using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Exceptions;
using Skopia.Tasks.Application.Interfaces;
using Skopia.Tasks.Controllers;
using Xunit;

namespace Skopia.Tasks.Tests.Controllers
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;
        private readonly Mock<IProjectService> _projectServiceMock;

        public TaskControllerTests()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _projectServiceMock = new Mock<IProjectService>();
        }

        private TaskController CreateController()
        {
            return new TaskController(_taskServiceMock.Object, _projectServiceMock.Object);
        }

        // --------------------------------------------------------------
        // GET BY PROJECT
        // --------------------------------------------------------------
        [Fact]
        public async Task GetByProject_Deve_Retornar_NotFound_Quando_Projeto_Nao_Existir()
        {
            // Arrange
            _taskServiceMock
                .Setup(s => s.GetByProjectIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new NotFoundException("Projeto não encontrado"));

            var controller = CreateController();

            // Act
            var response = await controller.GetByProject(1);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetByProject_Deve_Retornar_Lista_Quando_Existir()
        {
            // Arrange
            var lista = new List<TaskDto>
            {
                new TaskDto { Id = 1, Title = "T1" },
                new TaskDto { Id = 2, Title = "T2" }
            };

            _taskServiceMock
                .Setup(s => s.GetByProjectIdAsync(10))
                .ReturnsAsync(lista);

            var controller = CreateController();

            // Act
            var result = await controller.GetByProject(10) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            var data = result!.Value as IEnumerable<TaskDto>;
            data!.Count().Should().Be(2);
        }

        // --------------------------------------------------------------
        // CREATE
        // --------------------------------------------------------------
        [Fact]
        public async Task Create_Deve_Retornar_NotFound_Quando_Projeto_Nao_Existir()
        {
            // Arrange
            _taskServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<int>(), It.IsAny<TaskCreateDto>()))
                .ThrowsAsync(new NotFoundException("Projeto não encontrado"));

            var controller = CreateController();
            var dto = new TaskCreateDto { Title = "X" };

            // Act
            var response = await controller.Create(99, dto);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Create_Deve_Retornar_BadRequest_Quando_Dados_Invalidos()
        {
            // Arrange
            _taskServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<int>(), It.IsAny<TaskCreateDto>()))
                .ThrowsAsync(new BusinessException("Erro"));

            var controller = CreateController();
            var dto = new TaskCreateDto { Title = "X" };

            // Act
            var response = await controller.Create(10, dto);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Create_Deve_Retornar_Created_Quando_Sucesso()
        {
            // Arrange
            var retorno = new TaskDto { Id = 1, Title = "OK" };
            _taskServiceMock
                .Setup(s => s.CreateAsync(1, It.IsAny<TaskCreateDto>()))
                .ReturnsAsync(retorno);

            var controller = CreateController();

            // Act
            var response = await controller.Create(1, new TaskCreateDto { Title = "OK" });

            // Assert
            response.Should().BeOfType<CreatedAtActionResult>();
        }

        // --------------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------------
        [Fact]
        public async Task Update_Deve_Retornar_NotFound_Quando_Nao_Existir()
        {
            // Arrange
            _taskServiceMock
                .Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TaskUpdateDto>()))
                .ThrowsAsync(new NotFoundException("Não existe"));

            var controller = CreateController();

            // Act
            var response = await controller.UpdateAsync(1, 2, new TaskUpdateDto());

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Update_Deve_Retornar_BadRequest_Quando_Invalido()
        {
            // Arrange
            _taskServiceMock
                .Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TaskUpdateDto>()))
                .ThrowsAsync(new BusinessException("Inválido"));

            var controller = CreateController();

            // Act
            var response = await controller.UpdateAsync(1, 2, new TaskUpdateDto());

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_Deve_Retornar_Ok_Quando_Sucesso()
        {
            // Arrange
            var updated = new TaskDto { Id = 1, Title = "OK" };

            _taskServiceMock
                .Setup(s => s.UpdateAsync(1, 1, It.IsAny<TaskUpdateDto>()))
                .ReturnsAsync(updated);

            var controller = CreateController();

            // Act
            var response = await controller.UpdateAsync(1, 1, new TaskUpdateDto());

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        // --------------------------------------------------------------
        // DELETE
        // --------------------------------------------------------------
        [Fact]
        public async Task Delete_Deve_Retornar_NotFound_Quando_Nao_Existir()
        {
            // Arrange
            _taskServiceMock
                .Setup(s => s.DeleteAsync(It.IsAny<int>()))
                .ThrowsAsync(new NotFoundException("Não existe"));

            var controller = CreateController();

            // Act
            var response = await controller.Delete(9);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Delete_Deve_Retornar_NoContent_Quando_Sucesso()
        {
            // Arrange
            _taskServiceMock
                .Setup(s => s.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            var controller = CreateController();

            // Act
            var response = await controller.Delete(1);

            // Assert
            response.Should().BeOfType<NoContentResult>();
        }
    }
}
