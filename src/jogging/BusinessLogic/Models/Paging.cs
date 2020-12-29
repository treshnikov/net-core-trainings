using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Models
{
    public class Paging<T>
    {
        public int TotalCount { get; }
        public long StartAt { get; }
        public IEnumerable<T> Data { get; }

        public Paging(int totalCount, long startAt, IEnumerable<T> data)
        {
            TotalCount = totalCount;
            StartAt = startAt;
            Data = data;
        }

        public Paging<U> Select<U>(Func<T, U> func)
        {
            return new Paging<U>(TotalCount, StartAt, Data.Select(func));
        }
    }
}