namespace AddressValidation.Demo.Infrastructure.Repositories.Abstractions;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

public abstract partial class SqlRepository<TEntity, TContext>
    where TEntity : class, new()
    where TContext : DbContext
{
    public async ValueTask<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

        return await context.Set<TEntity>().ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public ValueTask<IReadOnlyList<TResult>> ListAsync<TResult>(Expression<Func<TEntity, TResult>> keySelector, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(nameof(keySelector));
        return ListInternalAsync(keySelector, cancellationToken);
    }

    public ValueTask<IReadOnlyList<TResult>> ListAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> keySelector, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(nameof(predicate));
        ArgumentNullException.ThrowIfNull(nameof(keySelector));
        return ListInternalAsync(predicate, keySelector, cancellationToken);
    }

    public ValueTask<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return ListInternalAsync(predicate, cancellationToken);
    }

    private async ValueTask<IReadOnlyList<TEntity>> ListInternalAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

        return await context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<IReadOnlyList<TResult>> ListInternalAsync<TResult>(Expression<Func<TEntity, TResult>> keySelector, CancellationToken cancellationToken)
    {
        TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

        return await context.Set<TEntity>().Select(keySelector).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<IReadOnlyList<TResult>> ListInternalAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> keySelector, CancellationToken cancellationToken)
    {
        TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

        return await context.Set<TEntity>()
                            .Where(predicate)
                            .Select(keySelector)
                            .ToListAsync(cancellationToken)
                            .ConfigureAwait(false);
    }
}
