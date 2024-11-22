namespace AddressValidation.Demo.Infrastructure.Repositories.Abstractions;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

public abstract partial class SqlRepository<TEntity, TContext>
	where TEntity : class, new()
	where TContext : DbContext
{
	public ValueTask<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(entity);

		return !HasPrimaryKey
				   ? ValueTask.FromResult(false)
				   : UpdateInternalAsync(entity, cancellationToken);
	}

	public ValueTask<bool> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> statements, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(predicate);
		return UpdateInternalAsync(predicate, statements, cancellationToken);
	}

	private async ValueTask<bool> UpdateInternalAsync(TEntity entity, CancellationToken cancellationToken)
	{
		TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
		await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

		context.Set<TEntity>().Update(entity);

		return await SaveChangesAsync(context, cancellationToken).ConfigureAwait(false);
	}

	private async ValueTask<bool> UpdateInternalAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> statements, CancellationToken cancellationToken)
	{
		TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
		await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

		return await context.Set<TEntity>().Where(predicate).ExecuteUpdateAsync(statements, cancellationToken).ConfigureAwait(false) >= 0;
	}
}
