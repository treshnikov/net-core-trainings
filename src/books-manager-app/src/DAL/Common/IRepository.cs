namespace DAL.Common
{
    public interface IRepository<T> : IAddableRepository<T>
    {
        void Update(int id, T entity);
        void Delete(int id);
    }
}