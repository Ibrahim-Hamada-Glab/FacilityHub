using FacilityHub.Core.Entities;

namespace FacilityHub.Core.Contracts;

public interface IUnitOfWork
{
     public IGenericRepository<LoginActivity>  LoginActivityRepository { get; }
     public Task<T> ExecuteInTransaction<T>(Func<Task<T>> action, CancellationToken cancellationToken) where T : class;
     public Task ExecuteInTransaction(Func<Task>  action, CancellationToken cancellationToken);
     public Task SaveChangesAsync(CancellationToken cancellationToken);
}