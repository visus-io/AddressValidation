namespace AddressValidation.Demo.Infrastructure.Repositories.Abstractions;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

public abstract partial class SqlRepository<TEntity, TContext>
    where TEntity : class, new()
    where TContext : DbContext
{
    public async ValueTask<TEntity> MaxAsync(CancellationToken cancellationToken = default)
    {
        TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

        return await context.Set<TEntity>().MaxAsync(cancellationToken).ConfigureAwait(false);
    }

    public ValueTask<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return MaxInternalAsync(predicate, cancellationToken);
    }

    private async ValueTask<TResult> MaxInternalAsync<TResult>(Expression<Func<TEntity, TResult>> predicate, CancellationToken cancellationToken)
    {
        TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

        return await context.Set<TEntity>().MaxAsync(predicate, cancellationToken).ConfigureAwait(false);
    }
}
