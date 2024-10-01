using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using Infrastructure.Data;
using api.Controllers;
using Microsoft.EntityFrameworkCore;

namespace UnitTests
{
    public class CandidatesControllerTests
    {
        private readonly Mock<IVoteAppContext> _mockContext;
        private readonly CandidatesController _controller;

        public CandidatesControllerTests()
        {
            _mockContext = new Mock<IVoteAppContext>();
            _controller = new CandidatesController(_mockContext.Object);
        }

        [Fact]
        public async Task GotVote_ReturnsNotFound_WhenCandidateDoesNotExsist()
        {
            //Arrange
            _mockContext.Setup(c => c.Candidates.FindAsync(It.IsAny<int>())).ReturnsAsync(null as Candidate);

            //Act
            var result = await _controller.GotVote(1);

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GotVote_ReturnsOkAndUpdatesCandidate()
        {
            //Arrange
            var candidate = new Candidate { Id = 1, Name = "Luke Skywalker", VoteCount = 0 };
            _mockContext.Setup(c => c.Candidates.FindAsync(It.IsAny<int>())).ReturnsAsync(candidate);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            //Act
            var result = await _controller.GotVote(1);

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var updatedCandidate = okResult.Value as Candidate;
            updatedCandidate.VoteCount.Should().Be(candidate.VoteCount);
        }

        [Fact]
        public async Task GotVote_ReturnsServerError_WhenSaveFails()
        {
            //Arrange
            var candidate = new Candidate { Id = 1, Name = "Luke Skywalker", VoteCount = 0 };
            _mockContext.Setup(c => c.Candidates.FindAsync(It.IsAny<int>())).ReturnsAsync(candidate);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ThrowsAsync(It.IsAny<Exception>());

            // Act
            var result = await _controller.GotVote(1);

            //Assert
            result.Result.Should().BeOfType<ObjectResult>();
            (result.Result as ObjectResult).StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task AddNewCandidate_ReturnsCreatedCandidate()
        {
            //Arrange
            var candidate = new Candidate { Name = "Luke Skywalker" };
            _mockContext.Setup(c => c.Candidates.Add(It.IsAny<Candidate>()));
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            //Act
            var result = await _controller.AddNewCandidate(candidate);

            //Assert
            var actionResult = result.Result as CreatedAtActionResult;
            actionResult.Should().NotBeNull();
            actionResult.Value.Should().BeEquivalentTo(candidate);
            actionResult.ActionName.Should().Be(nameof(_controller.GetCandidate));
            actionResult.RouteValues["id"].Should().Be(candidate.Id);
        }

        [Fact]
        public async Task AddNewCandidate_ReturnsConfict_WhenCandidateExsists()
        {
            // Arrange
            var existingCandidate = new Candidate { Id = 1, Name = "Luke Skywalker" };
            var newCandidate = new Candidate { Id = 1, Name = "Darth Vader" };

            _mockContext.Setup(c => c.Candidates.FindAsync(existingCandidate.Id)).ReturnsAsync(existingCandidate);

            //Act
            var result = await _controller.AddNewCandidate(newCandidate);

            //Assert
            result.Result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task AddNewCandidate_ThrowsException_WhenSaveFails()
        {
            //Arrange
            var candidate = new Candidate { Name = "Luke Skywalker" };
            _mockContext.Setup(c => c.Candidates.Add(It.IsAny<Candidate>()));
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ThrowsAsync(new Exception());

            //Act
            var result = await _controller.AddNewCandidate(candidate);

            //Assert
            result.Result.Should().BeOfType<ObjectResult>();
            (result.Result as ObjectResult).StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task GetCandidate_ReturnsCandidate()
        {
            // Arrange
            var candidate = new Candidate { Id = 1, Name = "Luke Skywalker" };

            _mockContext.Setup(c => c.Candidates.FindAsync(candidate.Id)).ReturnsAsync(candidate);

            //Act
            var result = await _controller.GetCandidate(candidate.Id);

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var returnedCandidate = okResult.Value as Candidate;
            returnedCandidate.Should().BeEquivalentTo(candidate);
        }

        [Fact]
        public async Task GetCandidate_ReturnsNotFound_WhenCandidateNotInDatabase()
        {
            // Arrange
            var candidate = new Voter { Id = 1, Name = "Luke Skywalker" };

            _mockContext.Setup(c => c.Candidates.FindAsync(candidate.Id)).ReturnsAsync(null as Candidate);

            //Act
            var result = await _controller.GetCandidate(candidate.Id);

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
            (result.Result as NotFoundResult).StatusCode.Should().Be(404);
        }
    }
}