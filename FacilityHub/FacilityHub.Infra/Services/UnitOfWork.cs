 
using FacilityHub.Core.Contracts;
using FacilityHub.Core.Entities;
using FacilityHub.Infra.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FacilityHub.Infra.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _appDbContext;
    private readonly ILogger<UnitOfWork> _logger;
 
    public UnitOfWork(AppDbContext appDbContext, ILogger<UnitOfWork> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
        LoginActivityRepository = new GenericRepository<LoginActivity>(_appDbContext);
        RefreshTokenRepository = new GenericRepository<RefreshToken>(_appDbContext);
    }

    public IGenericRepository<LoginActivity> LoginActivityRepository { get; } 
    public IGenericRepository<RefreshToken> RefreshTokenRepository { get; }
    public async Task<T> ExecuteInTransaction<T>(Func<Task<T>> action , CancellationToken cancellationToken) where T : class
    {
        if (_appDbContext.Database.CurrentTransaction != null)
      {
          _logger.LogInformation("Re-entrant call detected — joining existing transaction {TransactionId}",
              _appDbContext.Database.CurrentTransaction.TransactionId);
          return await action();
      }

        var strategy = _appDbContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            var transaction = await _appDbContext.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogInformation($"Beginning transaction {transaction.TransactionId}");
            try
            {
                var res = await action();
                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation($"Committed transaction {transaction.TransactionId}");
                return res;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogInformation($"Rollbacked transaction {transaction.TransactionId}");
                _logger.LogError(e, "An error occured during transaction");
                throw;
            }


        });
    }
    
    public async Task ExecuteInTransaction (Func<Task > action , CancellationToken cancellationToken)
    {
        var strategy = _appDbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            var transaction = await _appDbContext.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogInformation($"Beginning transaction {transaction.TransactionId}");
            try
            {
                 await action();
                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation($"Committed transaction {transaction.TransactionId}");
          
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogInformation($"Rollbacked transaction {transaction.TransactionId}");
                _logger.LogError(e, "An error occured during transaction");
                throw;
            }


        });
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>await _appDbContext.SaveChangesAsync(cancellationToken);
     
}