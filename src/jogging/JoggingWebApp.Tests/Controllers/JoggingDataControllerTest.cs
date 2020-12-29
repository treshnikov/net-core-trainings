using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.DAL;
using BusinessLogic.Managers;
using BusinessLogic.Models;
using Domain;
using JoggingWebApp.Controllers;
using JoggingWebApp.Models;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace JoggingWebApp.Tests
{
    public class JoggingDataControllerTest
    {
        private JoggingDataController _controller;

        [SetUp]
        public void SetUp()
        {
            var list = new List<JoggingData>();
            var joggingDataRepository = new Mock<IRepository<JoggingData>>();
            joggingDataRepository
                .Setup(m => m.Add(It.IsAny<JoggingData>()))
                .Callback<JoggingData>(d => list.Add(d));
            joggingDataRepository.Setup(m => m.Query())
                .Returns(() => list.AsQueryable());
            
            var userManager = new Mock<IUserManager>();
            userManager.Setup(m => m.Get(It.IsAny<long>()))
                .Returns<long>(id => new User
                {
                    Id = id
                });

            var weatherProvider = new Mock<IWeatherProvider>();
            
            _controller = new JoggingDataController(new JoggingDataManager(
                joggingDataRepository.Object,
                userManager.Object,
                weatherProvider.Object));
        }

        [Test]
        public void Get_WithWrongParameters_ShouldReturnBadRequest()
        {
            // arrange

            // act
            var result = _controller.Get(id: 0, userId: 1);

            // assert
            result.ShouldBeBadRequestResult();
        }
        
        [Test]
        public void Get_WithWrongSkipAndTakeParameters_ShouldBeBadRequest()
        {
            // arrange

            // act
            var result = _controller.Get(1, 0, 0, null);

            // assert
            result.ShouldBeBadRequestResult();
        }

        [Test]
        public async Task Get_WithAddCall_ShouldReturnOneRecord()
        {
            // arrange
            var model = new JoggingDataUpdateModel
            {
                Distance = 5,
                Date = new DateTime(2011, 5, 1, 10, 0 ,0),
            };
            
            // act
            var addResult = await _controller.Add(1, model);
            var getResult = _controller.Get(1);

            // assert
            addResult.ShouldBeCreatedResult();
            getResult.ShouldBeOkResult();

            var value = getResult.GetOkObjectValue<Paging<JoggingDataGetModel>>();
            var data = value.Data.ToArray();
            data.Length.ShouldBe(1);

            var item = data.Single();
            item.ShouldSatisfyAllConditions(
                () => item.Distance.ShouldBe(model.Distance.Value),
                () => item.Date.ShouldBe(model.Date.Value));
        }
        
        [Test]
        public async Task Get_WithoutTakeParam_ShouldReturnTenRecords()
        {
            // arrange
            for (var i = 1; i <= 11; i++)
            {
                var model = new JoggingDataUpdateModel
                {
                    Distance = i,
                    Date = new DateTime(2011, 5, i),
                };
            
                var addResult = await _controller.Add(1, model);
                addResult.ShouldBeCreatedResult();
            }
            
            // act
            var getResult = _controller.Get(1);

            // assert
            getResult.ShouldBeOkResult();

            var value = getResult.GetOkObjectValue<Paging<JoggingDataGetModel>>();
            var data = value.Data.ToArray();
            data.Length.ShouldBe(10);

            data.Select(j => j.Distance).ShouldBe(Enumerable.Range(1, 10));
        }
    }
}