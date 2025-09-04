
using System.Text.Json;
using Application.CommandHandlers.Start;
using Domain.Commands.Start;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace Tests.Application.CommandHandlers;

public class StartCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRadisRepository> _mockRadisRepository;
    private readonly StartCommandHandler _handler;
    
    public StartCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRadisRepository = new Mock<IRadisRepository>();
        _handler = new StartCommandHandler(_mockUserRepository.Object, _mockRadisRepository.Object);
    }
    
    [Fact]
    public async Task Handle_UserExists_ReturnsAlreadyRegisteredMessage()
    {
        // Arrange
        var existingUser = new User { IdTelegram = 123, Name = "John Doe" };
        _mockUserRepository.Setup(r => r.FindByIdTelegram(123)).ReturnsAsync(existingUser);
    
        var command = new StartCommand { UserId = 123 };
    
        // Act
        var result = await _handler.Handle(command);
    
        // Assert
        Assert.Equal($"Вы уже зарегистрированы!\nВаше имя: {existingUser.Name}", result);
        _mockRadisRepository.Verify(r => r.StringGet(It.IsAny<string>()), Times.Never);
    }
        
}