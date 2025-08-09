using Auth.Application.Services.Contracts;

namespace Auth.Infrastructure
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dataContext;

        public UnitOfWork(AppDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task BeginAsync()
        {
            await _dataContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _dataContext.SaveChangesAsync();
            _dataContext.Database.CommitTransaction();
        }

        public async Task CommitPartialAsync()
        {
            await _dataContext.SaveChangesAsync();
        }

        public async Task CompleteAsync()
        {
            await _dataContext.SaveChangesAsync();
        }

        public void Rollback()
        {
            _dataContext.Database.RollbackTransaction();
        }
    }
}
