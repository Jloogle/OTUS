using System.Text.Json;
using Application.CommandHandlers.Start;
using Domain.Commands.Start;
using Domain.Entities;
using Domain.Repositories;
using Moq;


namespace Tests.Application.CommandHandlers
{
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

        [Fact]
        public async Task Handle_UserNotFound_InitiatesNewRegistrationProcess()
        {
            // Arrange
            var command = new StartCommand { UserId = 123, Command = "/start", UserCommand = "/start" };
            _mockUserRepository.Setup(r => r.FindByIdTelegram(123)).ReturnsAsync((User)null);
            
            var tempUser = new User { State = (int)TrafficState.New };
            _mockRadisRepository.Setup(r => r.StringGet("Reg: 123")).Returns(JsonSerializer.Serialize(tempUser));

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.Equal("Добро пожаловать!\nНеобходимо пройти процедуру регистрации!\nВведите ваше имя:", result);
            _mockUserRepository.Verify(r => r.FindByIdTelegram(123), Times.Once);
            _mockRadisRepository.Verify(r => r.StringGet("Reg: 123"), Times.Once);
            _mockRadisRepository.Verify(r => r.StringSet("Reg: 123", It.IsAny<string>()), Times.Once);
        }
        

        [Fact]
        public async Task Handle_AgeState_SavesUserAgeAndAddsToRepository()
        {
            // Arrange
            var command = new StartCommand { UserId = 123, Command = "/start", UserCommand = "30" };
            _mockUserRepository.Setup(r => r.FindByIdTelegram(123)).ReturnsAsync((User)null);
            
            var tempUser = new User 
            { 
                State = (int)TrafficState.Age, 
                Name = "John Doe", 
                Email = "john@example.com", 
                PhoneNumber = "1234567890" 
            };
            _mockRadisRepository.Setup(r => r.StringGet("Reg: 123")).Returns(JsonSerializer.Serialize(tempUser));

            // Act
            var result = await _handler.Handle(command);

            // Assert

            _mockUserRepository.Verify(r => r.AddUser(It.Is<User>(u => 
                u.Name == "John Doe" && 
                u.Email == "john@example.com" && 
                u.PhoneNumber == "1234567890" && 
                u.Age == 30 &&
                u.State == (int)TrafficState.Finished &&
                u.IdTelegram == 123
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_ExceptionDuringRegistration_ReturnsErrorMessage()
        {
            // Arrange
            var command = new StartCommand { UserId = 123, Command = "/start", UserCommand = "invalid_age" };
            _mockUserRepository.Setup(r => r.FindByIdTelegram(123)).ReturnsAsync((User)null);
            
            var tempUser = new User 
            { 
                State = (int)TrafficState.Age, 
                Name = "John Doe", 
                Email = "john@example.com", 
                PhoneNumber = "1234567890" 
            };
            _mockRadisRepository.Setup(r => r.StringGet("Reg: 123")).Returns(JsonSerializer.Serialize(tempUser));
            _mockUserRepository.Setup(r => r.AddUser(It.IsAny<User>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command);

            // Assert
            _mockUserRepository.Verify(r => r.AddUser(It.IsAny<User>()), Times.Once);
        }
    }
}