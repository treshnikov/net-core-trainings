using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DAL;
using BusinessLogic.Models;
using Domain;

namespace BusinessLogic.Managers
{
    public class WeekReportProvider : IWeekReportProvider
    {
        private readonly IRepository<JoggingData> _joggingDataRepository;

        public WeekReportProvider(IRepository<JoggingData> joggingDataRepository)
        {
            _joggingDataRepository = joggingDataRepository;
        }
        
        public IReadOnlyCollection<WeekReport> GetReport(long userId, DateTime start, DateTime end, int? skip = null, int? take = null)
        {
            var result = _joggingDataRepository.Query()
                .Where(d => d.User.Id == userId && d.Date >= start && d.Date < end)
                .AsEnumerable() // TODO : Try to remove it
                .GroupBy(d => d.Date.AddDays(-1 * (7 + d.Date.DayOfWeek - DayOfWeek.Monday) % 7))
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    StartOfWeek = g.Key,
                    TotalDistance = g.Sum(d => d.Distance),
                    TotalTime = g.Sum(d => d.Time)
                })
                .Select(a => new WeekReport
                {
                    StartOfWeek = a.StartOfWeek,
                    Distance = a.TotalDistance,
                    AverageSpeed = a.TotalTime != 0 ? (double) a.TotalDistance / a.TotalTime : 0
                });

            if (skip.HasValue)
            {
                result = result.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                result = result.Take(take.Value);
            }

            return result.ToArray();
        }
    }
}