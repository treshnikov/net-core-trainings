using System.Collections.Generic;
using System.Linq;
using BusinessLogic;
using BusinessLogic.Common;
using BusinessLogic.DAL;
using BusinessLogic.Exceptions;
using BusinessLogic.Managers;
using BusinessLogic.Models;
using Domain;
using JoggingWebApp.Controllers;
using JoggingWebApp.Models;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace JoggingWebApp.Tests
{
    [TestFixture]
    public class UsersControllerTest
    {
        private UsersController _controller;

        [SetUp]
        public void SetUp()
        {
            var list = new List<User>();
            var userRepository = new Mock<IRepository<User>>();
            userRepository
                .Setup(m => m.Add(It.IsAny<User>()))
                .Callback<User>(d => list.Add(d));
            userRepository.Setup(m => m.Query())
                .Returns(() => list.AsQueryable());

            var options = new Mock<IOptions<AppSettings>>();
            options.Setup(m => m.Value)
                .Returns(() => new AppSettings
                {
                    JwtSecretKey = "test"
                });
            
            var userRoleRepository = new Mock<IReadOnlyRepository<UserRole>>();
            userRoleRepository.Setup(m => m.Query())
                .Returns(() => new EnumerableQuery<UserRole>(new List<UserRole>
                {
                    new UserRole
                    {
                        Id = 1
                    }
                }));
            
            var serverSettingsManager = new Mock<IServerSettingsManager>();
            serverSettingsManager.Setup(m => m.GetServerSettings())
                .Returns(() => new MainServerSettings
                {
                    DefaultUserRole = "1"
                });
            
            _controller = new UsersController(new UserManager(userRepository.Object, userRoleRepository.Object, null, serverSettingsManager.Object), options.Object);
        }
        
        [Test]
        public void Get_WithAddCall_ShouldReturnOneRecord()
        {
            // arrange
            var model = new UpdateUserModel
            {
                UserName = "Asd",
                Email = "a@b.com",
                Password = "12345678"
            };
            
            // act
            var addResult = _controller.Add(model);
            var getResult = _controller.Get();

            // assert
            addResult.ShouldBeCreatedResult();
            getResult.ShouldBeOkResult();

            var value = getResult.GetOkObjectValue<Paging<GetUserModel>>();
            var data = value.Data.ToArray();
            data.Length.ShouldBe(1);

            var item = data.Single();
            item.ShouldSatisfyAllConditions(
                () => item.UserName.ShouldBe("Asd"),
                () => item.Email.ShouldBe("a@b.com"));
        }

        [Test]
        public void Register_WithShortPassword_ShouldThrowException()
        {
            // arrange
            var model = new UpdateUserModel
            {
                Email = "asd@zxc.com",
                Password = "1234567",
                UserName = "Asd"
            };

            // act
            Should.Throw<BadRequestException>(() => _controller.Register(model));

            // assert
        }
    }
}