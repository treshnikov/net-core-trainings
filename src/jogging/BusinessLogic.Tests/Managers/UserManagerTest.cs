using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Common;
using BusinessLogic.DAL;
using BusinessLogic.Exceptions;
using BusinessLogic.Managers;
using Domain;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace BusinessLogic.Tests
{
    [TestFixture]
    public class UserManagerTest
    {
        private Mock<IRepository<User>> _userRepository;
        private List<User> _dataSource;
        private UserManager _manager;
        private long _currentUserId;

        [SetUp]
        public void Setup()
        {
            var userRole = new UserRole
            {
                Id = 1,
                Name = "User"
            };

            var managerRole = new UserRole
            {
                Id = 2,
                Name = "Manager"
            };

            var adminRole = new UserRole
            {
                Id = 3,
                Name = "Administrator",
                IsSuperUser = true
            };
            
            _dataSource = new List<User>
            {
                new User
                {
                    Id = 1,
                    Email = "a@b.com",
                    Role = userRole
                },
                new User
                {
                    Id = 2,
                    Email = "b@b.com",
                    Role = managerRole
                },
                new User
                {
                    Id = 3,
                    Email = "c@b.com",
                    Role = adminRole
                }
            };
            
            _userRepository = new Mock<IRepository<User>>();
            _userRepository
                .Setup(m => m.Query())
                .Returns(() => new EnumerableQuery<User>(_dataSource));
             
            var userRoleRepository = new Mock<IReadOnlyRepository<UserRole>>();
            userRoleRepository.Setup(m => m.Query())
                .Returns(() => new EnumerableQuery<UserRole>(new List<UserRole>
                {
                    userRole,
                    managerRole,
                    adminRole
                }));
            
            var serverSettingsManager = new Mock<IServerSettingsManager>();

            var userPrincipal = new Mock<IUserPrincipal>();
            userPrincipal.Setup(m => m.UserId)
                .Returns(() => _currentUserId);
            
            _manager = new UserManager(
                _userRepository.Object, 
                userRoleRepository.Object,
                userPrincipal.Object,
                serverSettingsManager.Object);
        }
        
        [Test]
        public void Get_FilterAndSkip_ShouldReturnTwoItems()
        {
            // arrange
            // act
            var paging = _manager.Get("email ne 'c@b.com'", 10, 1);

            // assert
            paging.ShouldSatisfyAllConditions(
                () => paging.TotalCount.ShouldBe(2),
                () => paging.StartAt.ShouldBe(1));
            
            var data = paging.Data.ToArray();
            data.Length.ShouldBe(1);
            data[0].Id.ShouldBe(2);
        }

        [Test]
        public void UpdateRole_PromoteUserToManager_ShouldSucceed()
        {
            // arrange
            _currentUserId = 2;
            
            // act
            _manager.UpdateRole(1, 2);

            // assert
            var newManager = _manager.Get(1);
            newManager.Role.Id.ShouldBe(2);
        }
        
        [Test]
        public void UpdateRole_PromoteUserToAdmin_ShouldFail()
        {
            // arrange
            _currentUserId = 2;
            
            // act
            Should.Throw<BadRequestException>(() => _manager.UpdateRole(1, 3));

            // assert
        }
        
        [Test]
        public void UpdateRole_DemoteAdminToUser_ShouldFail()
        {
            // arrange
            _currentUserId = 2;
            
            // act
            Should.Throw<BadRequestException>(() => _manager.UpdateRole(3, 1));

            // assert
        }
        
        [Test]
        public void UpdateRole_PromoteUserToAdminByAdmin_ShouldSucceed()
        {
            // arrange
            _currentUserId = 3;
            
            // act
            _manager.UpdateRole(1, 3);

            // assert
            var newManager = _manager.Get(1);
            newManager.Role.Id.ShouldBe(3);
        }
    }
}