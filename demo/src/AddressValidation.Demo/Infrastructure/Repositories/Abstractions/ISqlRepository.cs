namespace AddressValidation.Demo.Infrastructure.Repositories.Abstractions;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

public interface ISqlRepository<TEntity>
    where TEntity : class, new()
{
    ValueTask<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    ValueTask<bool> AnyAsync(CancellationToken cancellationToken = default);

    ValueTask<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    ValueTask<int> CountAsync(CancellationToken cancellationToken = default);

    ValueTask<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    ValueTask<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    ValueTask<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    ValueTask<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<TResult>> ListAsync<TResult>(Expression<Func<TEntity, TResult>> keySelector, CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<TResult>> ListAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> keySelector, CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    ValueTask<long> LongCountAsync(CancellationToken cancellationToken = default);

    ValueTask<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    ValueTask<TEntity> MaxAsync(CancellationToken cancellationToken = default);

    ValueTask<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> predicate, CancellationToken cancellationToken = default);

    ValueTask<TEntity> MinAsync(CancellationToken cancellationToken = default);

    ValueTask<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> predicate, CancellationToken cancellationToken = default);

    ValueTask<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    ValueTask<bool> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> statements, CancellationToken cancellationToken = default);
}
