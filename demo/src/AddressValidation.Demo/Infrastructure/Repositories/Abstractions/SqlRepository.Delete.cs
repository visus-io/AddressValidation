namespace AddressValidation.Demo.Infrastructure.Repositories.Abstractions;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

public abstract partial class SqlRepository<TEntity, TContext>
    where TEntity : class, new()
    where TContext : DbContext
{
    public ValueTask<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return !HasPrimaryKey
                   ? ValueTask.FromResult(false)
                   : DeleteInternalAsync(entity, cancellationToken);
    }

    public ValueTask<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return DeleteInternalAsync(predicate, cancellationToken);
    }

    private async ValueTask<bool> DeleteInternalAsync(TEntity entity, CancellationToken cancellationToken)
    {
        TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

        context.Set<TEntity>().Remove(entity);

        return await SaveChangesAsync(context, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<bool> DeleteInternalAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

        return await context.Set<TEntity>().Where(predicate).ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false) >= 0;
    }
}
