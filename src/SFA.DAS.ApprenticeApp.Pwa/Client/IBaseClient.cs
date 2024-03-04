namespace SFA.DAS.ApprenticeApp.Pwa.Client
{
    public interface IBaseClient<T> : IDisposable
    {
        Task<List<T>> GetAll();

        Task<T> Get(int? id);

        Task Create(T item);

        Task Update(int? id, T item);

        Task Delete(int id);
    }
}
