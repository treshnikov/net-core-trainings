namespace BusinessLogic.DAL
{
    public interface IRepository<T> : IReadOnlyRepository<T>
    {
        void Add(T entity);
        void Update<TInt>(TInt id, T entity);
        void Delete(long id);
    }
}