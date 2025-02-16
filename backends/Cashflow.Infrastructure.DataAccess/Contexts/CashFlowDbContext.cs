using Cashflow.Core;
using Cashflow.Domain.Abstractions.DataAccess;
using Cashflow.Domain.Abstractions.EventHandling;
using Cashflow.Domain.Abstractions.RequestPipeline;
using Cashflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cashflow.Infrastructure.DataAccess.Contexts;

internal class CashflowDbContext : DbContext, ICashflowDbContext
{
    private readonly IAuthenticatedUser _authenticatedUser;
    private readonly IEventMediator _eventMediator;
    
    public DbSet<AccountType> AccountTypes { get; init; }
    public DbSet<AuditionEvent> AuditionEvents { get; init; }
    public DbSet<BankAccount> BankAccounts { get; init; }
    public DbSet<Category> Categories { get; init; }
    public DbSet<Recurrency> Recurrencies { get; init; }
    public DbSet<RecurrencyTime> RecurrenciesTime { get; init; }
    public DbSet<Role> Roles { get; init; }
    public DbSet<Transaction> Transactions { get; init; }
    public DbSet<TransactionMethod> TransactionMethods { get; init; }
    public DbSet<User> Users { get; init; }

    public CashflowDbContext(DbContextOptions options, IAuthenticatedUser authenticatedUser, IEventMediator eventMediator)
        : base(options)
    {
        _authenticatedUser = authenticatedUser;
        _eventMediator = eventMediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CashflowDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<IEntity<long>>().ToList();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.OwnerId = _authenticatedUser.Id;
                entry.Entity.Activate();
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedAt = DateTime.UtcNow;
                entry.Entity.LastModifiedBy = _authenticatedUser.Id;
            }
        }

        // Optimize operations on Save Changes.
        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var entry in entries)
        {
            if (entry.Entity is IEventSource eventSource)
            {
                await HandleTransactionEvents(eventSource);
            }
        }

        return result;
    }

    private async Task HandleTransactionEvents(IEventSource source)
    {
       foreach (var @event in source.Invoke())
       {
           
           var auditionEvent = new AuditionEvent
           {
               Event = @event.Description(),
               Private = @event.Private,
               OccuredAt = @event.OccuredAt,
               IpAddress = _authenticatedUser.IpAddress,
               UserAgent = _authenticatedUser.UserAgent
           };
           
           auditionEvent.BindToTrace(_authenticatedUser.TraceIdentifier);
           auditionEvent.BindToUserWithId(_authenticatedUser.Id);
           Add(auditionEvent);
           
           @event.BindToUserWithId(_authenticatedUser.Id);
           @event.BindToTrace(_authenticatedUser.TraceIdentifier);
           await _eventMediator.NotifyAsync(@event);
       }

       await base.SaveChangesAsync(default);
    }
}