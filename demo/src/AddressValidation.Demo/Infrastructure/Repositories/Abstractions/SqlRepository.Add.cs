namespace AddressValidation.Demo.Infrastructure.Repositories.Abstractions;

using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

public abstract partial class SqlRepository<TEntity, TContext>
    where TEntity : class, new()
    where TContext : DbContext
{
    public ValueTask<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return !HasPrimaryKey
                   ? ValueTask.FromResult(false)
                   : AddInternalAsync(entity, cancellationToken);
    }

    private async ValueTask<bool> AddInternalAsync(TEntity entity, CancellationToken cancellationToken)
    {
        TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

        context.Set<TEntity>().Add(entity);

        return await SaveChangesAsync(context, cancellationToken).ConfigureAwait(false);
    }
}
