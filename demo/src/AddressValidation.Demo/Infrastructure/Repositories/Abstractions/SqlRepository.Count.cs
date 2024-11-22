namespace AddressValidation.Demo.Infrastructure.Repositories.Abstractions;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

public abstract partial class SqlRepository<TEntity, TContext>
	where TEntity : class, new()
	where TContext : DbContext
{
	public async ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
	{
		TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
		await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

		return await context.Set<TEntity>().CountAsync(cancellationToken).ConfigureAwait(false);
	}

	public ValueTask<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(predicate);
		return CountInternalAsync(predicate, cancellationToken);
	}

	public async ValueTask<long> LongCountAsync(CancellationToken cancellationToken = default)
	{
		TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
		await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

		return await context.Set<TEntity>().LongCountAsync(cancellationToken).ConfigureAwait(false);
	}

	public ValueTask<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(predicate);
		return LongCountInternalAsync(predicate, cancellationToken);
	}

	private async ValueTask<int> CountInternalAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
	{
		TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
		await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

		return await context.Set<TEntity>().CountAsync(predicate, cancellationToken).ConfigureAwait(false);
	}

	private async ValueTask<long> LongCountInternalAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
	{
		TContext context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
		await using ConfiguredAsyncDisposable _ = context.ConfigureAwait(false);

		return await context.Set<TEntity>().LongCountAsync(predicate, cancellationToken).ConfigureAwait(false);
	}
}
