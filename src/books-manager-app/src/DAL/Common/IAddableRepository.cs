namespace DAL.Common
{
    public interface IAddableRepository<T> : IReadOnlyRepository<T>
    {
        void Add(T entity);
    }
}