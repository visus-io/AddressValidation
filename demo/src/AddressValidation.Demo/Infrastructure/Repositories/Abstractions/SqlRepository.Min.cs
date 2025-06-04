namespace AddressValidation.Demo.Infrastructure.Repositories.Abstractions;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

public abstract partial class SqlRepository<TEntity, TContext>
    where TEntity : class, new()
    where TContext : DbContext
{
    public async ValueTask<TEntity> MinAsync(CancellationToken cancellationToken = default)
    {
        TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

        return await context.Set<TEntity>().MinAsync(cancellationToken).ConfigureAwait(false);
    }

    public ValueTask<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return MinInternalAsync(predicate, cancellationToken);
    }

    private async ValueTask<TResult> MinInternalAsync<TResult>(Expression<Func<TEntity, TResult>> predicate, CancellationToken cancellationToken)
    {
        TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

        return await context.Set<TEntity>().MinAsync(predicate, cancellationToken).ConfigureAwait(false);
    }
}
