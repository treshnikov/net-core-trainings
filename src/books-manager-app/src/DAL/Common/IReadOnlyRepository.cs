using System.Linq;

namespace DAL.Common
{
    public interface IReadOnlyRepository<T>
    {
        IQueryable<T> Query();
    }
}