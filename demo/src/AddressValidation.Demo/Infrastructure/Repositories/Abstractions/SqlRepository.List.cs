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
}
