namespace Auth.Application.Services.Contracts
{
    public interface IUnitOfWork
    {
        Task BeginAsync();
        Task CommitPartialAsync();
        Task CommitAsync();
        void Rollback();
        Task CompleteAsync();
    }
}
