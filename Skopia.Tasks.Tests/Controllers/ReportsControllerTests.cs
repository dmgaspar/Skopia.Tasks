using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Skopia.Tasks.Application.DTOs;
using Skopia.Tasks.Application.Interfaces;
using Skopia.Tasks.Controllers;
using Xunit;

namespace Skopia.Tasks.Tests.Controllers
{
    public class ReportsControllerTests
    {
        private readonly Mock<IReportService> _reportServiceMock;

        public ReportsControllerTests()
        {
            _reportServiceMock = new Mock<IReportService>();
        }

        private ReportsController CreateController()
        {
            return new ReportsController(_reportServiceMock.Object);
        }

        // ================================
        // GET REPORT OK
        // ================================
        [Fact]
        public async Task GetPerformanceReport_Deve_Retornar_Ok_Quando_Manager()
        {
            // Arrange
            var expected = new List<PerformanceReportDto>
            {
                new PerformanceReportDto
                {
                    UserId = 1,
                    CompletedTasks = 5,
                    AveragePerDay = 0.2
                }
            };

            _reportServiceMock
                .Setup(s => s.GetPerformanceReportAsync())
                .ReturnsAsync(expected);

            var controller = CreateController();

            // Act
            var response = await controller.GetPerformanceReport("manager");
            var ok = response as OkObjectResult;

            // Assert
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(expected);
        }
    }
}
