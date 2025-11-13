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
    public class CommentControllerTests
    {
        private readonly Mock<ICommentService> _commentServiceMock;

        public CommentControllerTests()
        {
            _commentServiceMock = new Mock<ICommentService>();
        }

        private CommentController CreateController()
        {
            return new CommentController(_commentServiceMock.Object);
        }

        // --------------------------------------------------------------
        // CREATE
        // --------------------------------------------------------------
        [Fact]
        public async Task CreateAsync_Deve_Retornar_NotFound_Quando_Task_Nao_Existir()
        {
            // Arrange
            _commentServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((CommentDto?)null);

            var controller = CreateController();

            // Act
            var response = await controller.CreateAsync(999, "Texto");

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateAsync_Deve_Retornar_Ok_Quando_Criar_Com_Sucesso()
        {
            // Arrange
            var expected = new CommentDto { Id = 10, Text = "Criado" };

            _commentServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(expected);

            var controller = CreateController();

            // Act
            var response = await controller.CreateAsync(1, "Texto");

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            var ok = response as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(expected);
        }

        // --------------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------------
        [Fact]
        public async Task UpdateAsync_Deve_Retornar_NotFound_Quando_Comentario_Nao_Existir()
        {
            // Arrange
            _commentServiceMock
                .Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((CommentDto?)null);

            var controller = CreateController();

            // Act
            var response = await controller.UpdateAsync(999, "Texto atualizado");

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateAsync_Deve_Retornar_Ok_Quando_Atualizar_Com_Sucesso()
        {
            // Arrange
            var expected = new CommentDto { Id = 1, Text = "Atualizado" };

            _commentServiceMock
                .Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(expected);

            var controller = CreateController();

            // Act
            var response = await controller.UpdateAsync(1, "Texto atualizado");

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            var ok = response as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(expected);
        }

        // --------------------------------------------------------------
        // DELETE
        // --------------------------------------------------------------
        [Fact]
        public async Task DeleteAsync_Deve_Retornar_NotFound_Quando_Comentario_Nao_Existir()
        {
            // Arrange
            _commentServiceMock
                .Setup(s => s.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var controller = CreateController();

            // Act
            var response = await controller.DeleteAsync(1);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteAsync_Deve_Retornar_NoContent_Quando_Excluir_Com_Sucesso()
        {
            // Arrange
            _commentServiceMock
                .Setup(s => s.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var controller = CreateController();

            // Act
            var response = await controller.DeleteAsync(1);

            // Assert
            response.Should().BeOfType<NoContentResult>();
        }
    }
}
