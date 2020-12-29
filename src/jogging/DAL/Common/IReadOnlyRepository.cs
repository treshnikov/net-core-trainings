using System.Linq;

namespace BusinessLogic.DAL
{
    public interface IReadOnlyRepository<T>
    {
        IQueryable<T> Query();
        T Find<TInt>(TInt id);
    }
}