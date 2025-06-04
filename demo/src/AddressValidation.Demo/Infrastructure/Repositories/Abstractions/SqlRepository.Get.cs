namespace AddressValidation.Demo.Infrastructure.Repositories.Abstractions;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

public abstract partial class SqlRepository<TEntity, TContext>
    where TEntity : class, new()
    where TContext : DbContext
{
    public ValueTask<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return GetInternalAsync(predicate, cancellationToken);
    }

    private async ValueTask<TEntity?> GetInternalAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

        return await context.Set<TEntity>().Where(predicate).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }
}
