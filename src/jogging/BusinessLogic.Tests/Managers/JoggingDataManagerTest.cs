using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DAL;
using BusinessLogic.Managers;
using Domain;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace BusinessLogic.Tests
{
    public class JoggingDataManagerTest
    {
        private List<JoggingData> _dataSource;
        private Mock<IRepository<JoggingData>> _joggingDataRepository;
        private JoggingDataManager _manager;

        [SetUp]
        public void Setup()
        {
            var user = new User
            {
                Id = 1
            };
            
            _dataSource = new List<JoggingData>
            {
                new JoggingData
                {
                    Id = 1,
                    Distance = 5,
                    Date = new DateTime(2011, 1, 4),
                    User = user
                },
                new JoggingData
                {
                    Id = 2,
                    Distance = 10,
                    Date = new DateTime(2011, 1, 3),
                    User = user
                },
                new JoggingData
                {
                    Id = 3,
                    Distance = 15,
                    Date = new DateTime(2011, 1, 2),
                    User = user
                },
                new JoggingData
                {
                    Id = 4,
                    Distance = 20,
                    Date = new DateTime(2011, 1, 1),
                    User = user
                },
                new JoggingData
                {
                    Id = 5,
                    Distance = 100,
                    Date = new DateTime(2011, 1, 1),
                    User = new User
                    {
                        Id = 2
                    }
                }
            };
            
             _joggingDataRepository = new Mock<IRepository<JoggingData>>();
             _joggingDataRepository
                 .Setup(m => m.Query())
                 .Returns(() => new EnumerableQuery<JoggingData>(_dataSource));
             
             var userManager = new Mock<IUserManager>();
             var weatherProvider = new Mock<IWeatherProvider>();
             
             _manager = new JoggingDataManager(
                 _joggingDataRepository.Object, 
                 userManager.Object,
                 weatherProvider.Object);
        }

        [Test]
        public void Get_WithEmptyParameters_ShouldReturnAllItemsOrderedByDate()
        {
            // arrange
            // act
            var paging = _manager.Get(1, null, 10, null);

            // assert
            paging.ShouldSatisfyAllConditions(
                () => paging.TotalCount.ShouldBe(4),
                () => paging.StartAt.ShouldBe(0));
            
            var data = paging.Data.ToArray();
            data.Length.ShouldBe(4);
            data.ShouldSatisfyAllConditions(
                () => data[0].Id.ShouldBe(4),
                () => data[1].Id.ShouldBe(3),
                () => data[2].Id.ShouldBe(2),
                () => data[3].Id.ShouldBe(1));
        }
        
        [Test]
        public void Get_FirstThreeItems_ShouldReturnThreeItemsOrderedByDate()
        {
            // arrange
            // act
            var paging = _manager.Get(1, null, 3, 0);

            // assert
            paging.ShouldSatisfyAllConditions(
                () => paging.TotalCount.ShouldBe(4),
                () => paging.StartAt.ShouldBe(0));
            
            var data = paging.Data.ToArray();
            data.Length.ShouldBe(3);
            data.ShouldSatisfyAllConditions(
                () => data[0].Id.ShouldBe(4),
                () => data[1].Id.ShouldBe(3),
                () => data[2].Id.ShouldBe(2));
        }

        [Test]
        public void Get_SkipTwoItems_ShouldReturnTwoItems()
        {
            // arrange
            // act
            var paging = _manager.Get(1, null, 10, 2);

            // assert
            paging.ShouldSatisfyAllConditions(
                () => paging.TotalCount.ShouldBe(4),
                () => paging.StartAt.ShouldBe(2));
            
            var data = paging.Data.ToArray();
            data.Length.ShouldBe(2);
            data.ShouldSatisfyAllConditions(
                () => data[0].Id.ShouldBe(2),
                () => data[1].Id.ShouldBe(1));
        }
        
        [Test]
        public void Get_FilterTwoItems_ShouldReturnTwoItems()
        {
            // arrange
            // act
            var paging = _manager.Get(1, "distance gt 10", 10, 0);

            // assert
            paging.ShouldSatisfyAllConditions(
                () => paging.TotalCount.ShouldBe(2),
                () => paging.StartAt.ShouldBe(0));
            
            var data = paging.Data.ToArray();
            data.Length.ShouldBe(2);
            data.ShouldSatisfyAllConditions(
                () => data[0].Id.ShouldBe(4),
                () => data[1].Id.ShouldBe(3));
        }
        
        [Test]
        public void Get_FilterAndSkip_ShouldReturnTwoItems()
        {
            // arrange
            // act
            var paging = _manager.Get(1, "distance gt 5", 10, 1);

            // assert
            paging.ShouldSatisfyAllConditions(
                () => paging.TotalCount.ShouldBe(3),
                () => paging.StartAt.ShouldBe(1));
            
            var data = paging.Data.ToArray();
            data.Length.ShouldBe(2);
            data.ShouldSatisfyAllConditions(
                () => data[0].Id.ShouldBe(3),
                () => data[1].Id.ShouldBe(2));
        }
        
        [Test]
        public void Get_FilterAndSkipAndTake_ShouldReturnOneItems()
        {
            // arrange
            // act
            var paging = _manager.Get(1, "distance gt 5", 1, 1);

            // assert
            paging.ShouldSatisfyAllConditions(
                () => paging.TotalCount.ShouldBe(3),
                () => paging.StartAt.ShouldBe(1));
            
            var data = paging.Data.ToArray();
            data.Length.ShouldBe(1);
            data.ShouldSatisfyAllConditions(
                () => data[0].Id.ShouldBe(3));
        }
    }
}