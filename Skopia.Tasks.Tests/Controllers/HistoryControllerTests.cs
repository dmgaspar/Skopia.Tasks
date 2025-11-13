using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Exceptions;
using Skopia.Tasks.Application.Interfaces;
using Skopia.Tasks.Controllers;
using Skopia.Tasks.Domain.Entities;
using Xunit;

namespace Skopia.Tasks.Tests.Controllers
{
    public class HistoryControllerTests
    {
        private readonly Mock<IHistoryService> _historyServiceMock;

        public HistoryControllerTests()
        {
            _historyServiceMock = new Mock<IHistoryService>();
        }

        private HistoryController CreateController()
        {
            return new HistoryController(_historyServiceMock.Object);
        }

        // =====================================================================
        // GET ALL
        // =====================================================================
        [Fact]
        public async Task GetAllAsync_Deve_Retornar_Ok_Com_Lista()
        {
            // Arrange
            var expected = new List<TaskHistory>
            {
                new TaskHistory { Id = 1, TaskItemId = 10, FieldName = "Title", OldValue = "A", NewValue = "B" },
                new TaskHistory { Id = 2, TaskItemId = 20, FieldName = "Status", OldValue = "Open", NewValue = "Done" }
            };

            _historyServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(expected);

            var controller = CreateController();

            // Act
            var response = await controller.GetAllAsync();

            // Assert
            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(expected);
        }

        // =====================================================================
        // GET BY TASK
        // =====================================================================
        [Fact]
        public async Task GetByTaskAsync_Deve_Retornar_NotFound_Quando_Tarefa_Nao_Existir()
        {
            // Arrange
            _historyServiceMock
                .Setup(s => s.GetByTaskAsync(It.IsAny<int>()))
                .ThrowsAsync(new NotFoundException("Histórico não encontrado"));

            var controller = CreateController();

            // Act
            var response = await controller.GetByTaskAsync(999);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetByTaskAsync_Deve_Retornar_Ok_Quando_Existir()
        {
            // Arrange
            var expected = new List<TaskHistory>
            {
                new TaskHistory { Id = 1, TaskItemId = 10, FieldName = "Title", OldValue = "X", NewValue = "Y" }
            };

            _historyServiceMock
                .Setup(s => s.GetByTaskAsync(It.IsAny<int>()))
                .ReturnsAsync(expected);

            var controller = CreateController();

            // Act
            var response = await controller.GetByTaskAsync(10);

            // Assert
            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(expected);
        }
    }
}
