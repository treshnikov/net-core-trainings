using System;
using System.Collections.Generic;
using BusinessLogic.Models;
using Domain;

namespace BusinessLogic.Managers
{
    public interface IWeekReportProvider
    {
        IReadOnlyCollection<WeekReport> GetReport(long userId, DateTime start, DateTime end, int? skip = null, int? take = null);
    }
}