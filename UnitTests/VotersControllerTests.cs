using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using Infrastructure.Data;
using api.Controllers;
using Microsoft.EntityFrameworkCore;

namespace UnitTests
{
    public class VotersControllerTests
    {
        private readonly Mock<IVoteAppContext> _mockContext;
        private readonly VotersController _controller;

        public VotersControllerTests()
        {
            _mockContext = new Mock<IVoteAppContext>();
            _controller = new VotersController(_mockContext.Object);
        }

        [Fact]
        public async Task HasVoted_ReturnsNotFound_WhenVoterDoesNotExsist()
        {
            //Arrange
            _mockContext.Setup(c => c.Voters.FindAsync(It.IsAny<int>())).ReturnsAsync(null as Voter);

            //Act
            var result = await _controller.HasVoted(1);

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task HasVoted_ReturnsOkAndUpdatesVoter()
        {
            //Arrange
            var voter = new Voter { Id = 1, Name = "Luke Skywalker", HasVoted = false };
            _mockContext.Setup(c => c.Voters.FindAsync(It.IsAny<int>())).ReturnsAsync(voter);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            //Act
            var result = await _controller.HasVoted(1);

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var updatedVoter = okResult.Value as Voter;
            updatedVoter.HasVoted.Should().BeTrue();
        }

        [Fact]
        public async Task HasVoted_ReturnsServerError_WhenSaveFails()
        {
            //Arrange
            var voter = new Voter { Id = 1, Name = "Luke Skywalker", HasVoted = false };
            _mockContext.Setup(c => c.Voters.FindAsync(It.IsAny<int>())).ReturnsAsync(voter);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ThrowsAsync(It.IsAny<Exception>());

            // Act
            var result = await _controller.HasVoted(1);

            //Assert
            result.Result.Should().BeOfType<ObjectResult>();
            (result.Result as ObjectResult).StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task AddNewVoter_ReturnsCreatedVoter()
        {
            //Arrange
            var voter = new Voter { Name = "Luke Skywalker" };
            _mockContext.Setup(c => c.Voters.Add(It.IsAny<Voter>()));
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            //Act
            var result = await _controller.AddNewVoter(voter);

            //Assert
            var actionResult = result.Result as CreatedAtActionResult;
            actionResult.Should().NotBeNull();
            actionResult.Value.Should().BeEquivalentTo(voter);
            actionResult.ActionName.Should().Be(nameof(_controller.GetVoter));
            actionResult.RouteValues["id"].Should().Be(voter.Id);
        }

        [Fact]
        public async Task AddNewVoter_ThrowsException_WhenSaveFails()
        {
            //Arrange
            var voter = new Voter { Name = "Luke Skywalker" };
            _mockContext.Setup(c => c.Voters.Add(It.IsAny<Voter>()));
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ThrowsAsync(new Exception());

            //Act
            var result = await _controller.AddNewVoter(voter);

            //Assert
            result.Result.Should().BeOfType<ObjectResult>();
            (result.Result as ObjectResult).StatusCode.Should().Be(500);
        }
    }
}