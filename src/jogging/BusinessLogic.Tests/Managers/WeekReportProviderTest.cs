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
    [TestFixture]
    public class WeekReportProviderTest
    {
        [Test]
        public void GetReport_WithAllParameters_ShouldReturnData()
        {
            // arrange
            var list = new List<JoggingData>
            {
                new JoggingData
                {
                    User = new User
                    {
                        Id = 1
                    },
                    Date = new DateTime(2019, 11, 16),
                    Distance = 40,
                    Time = 5
                },
                new JoggingData
                {
                    User = new User
                    {
                        Id = 1
                    },
                    Date = new DateTime(2019, 11, 17),
                    Distance = 10,
                    Time = 5
                },
                new JoggingData
                {
                    User = new User
                    {
                        Id = 1
                    },
                    Date = new DateTime(2019, 11, 20),
                    Distance = 20,
                    Time = 10
                }
            };
            
            var mock = new Mock<IRepository<JoggingData>>();
            mock.Setup(m => m.Query())
                .Returns(() => new EnumerableQuery<JoggingData>(list));
            var provider = new WeekReportProvider(mock.Object);

            // act
            var data = provider.GetReport(1, new DateTime(2019, 11, 11), new DateTime(2019, 11, 25))
                .ToArray();

            // assert
            data.Length.ShouldBe(2);
            data.ShouldSatisfyAllConditions(
                () => data[0].Distance.ShouldBe(50),
                () => data[0].StartOfWeek.ShouldBe(new DateTime(2019, 11, 11)),
                () => data[0].AverageSpeed.ShouldBe(5),
                
                () => data[1].Distance.ShouldBe(20),
                () => data[1].StartOfWeek.ShouldBe(new DateTime(2019, 11, 18)),
                () => data[1].AverageSpeed.ShouldBe(2));
        }
    }
}