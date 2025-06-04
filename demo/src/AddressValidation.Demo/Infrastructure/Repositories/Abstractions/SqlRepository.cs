namespace AddressValidation.Demo.Infrastructure.Repositories.Abstractions;

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

public abstract partial class SqlRepository<TEntity, TContext> : ISqlRepository<TEntity>
    where TEntity : class, new()
    where TContext : DbContext
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    private static readonly Action<ILogger, Exception, string, Exception?> LogDbUpdateException =
        LoggerMessage.Define<Exception, string>(LogLevel.Error,
                                                new EventId(2203, nameof(SaveChangesAsync)),
                                                "{Message}: {@Exception}");

    private readonly Lazy<Dictionary<string, string?>> _columns;

    private readonly IDbContextFactory<TContext> _contextFactory;

    private readonly ILogger<SqlRepository<TEntity, TContext>> _logger;

    private readonly Lazy<IReadOnlyList<IProperty>> _primaryKeys;

    private readonly Lazy<string?> _schemaName;

    private readonly Lazy<string?> _tableName;

    protected SqlRepository(IDbContextFactory<TContext> contextFactory, ILogger<SqlRepository<TEntity, TContext>> logger)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // note: must be initialized first
        _schemaName = new Lazy<string?>(GetEntitySchemaName);
        _tableName = new Lazy<string?>(GetEntityTableName);

        _columns = new Lazy<Dictionary<string, string?>>(GetEntityColumns);
        _primaryKeys = new Lazy<IReadOnlyList<IProperty>>(GetEntityPrimaryKeys);
    }

    protected IReadOnlyDictionary<string, string?> Columns => _columns.Value;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    protected bool HasPrimaryKey => _primaryKeys.Value.Any();

    protected IReadOnlyList<IProperty> PrimaryKeys => _primaryKeys.Value;

    protected string? SchemaName => _schemaName.Value;

    protected string? TableName => _tableName.Value;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    protected async ValueTask<bool> SaveChangesAsync(TContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            bool result = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;

            // Note: intentional as we are working in a 'disconnected' state.
            context.ChangeTracker.Clear();

            return result;
        }
        catch ( DbUpdateException e )
        {
            LogDbUpdateException(_logger, e, e.Message, null);
        }

        return false;
    }

    private Dictionary<string, string?> GetEntityColumns()
    {
        if ( string.IsNullOrWhiteSpace(_tableName.Value) )
        {
            return new Dictionary<string, string?>();
        }

        StoreObjectIdentifier storeObjectIdentifier = StoreObjectIdentifier.Table(_tableName.Value, _schemaName.Value);

        using TContext context = _contextFactory.CreateDbContext();

        return context.Model.FindEntityType(typeof(TEntity))?.GetProperties()
                      .Where(w => string.IsNullOrWhiteSpace(w.GetComputedColumnSql(storeObjectIdentifier)) &&
                                  ( w.ValueGenerated & ValueGenerated.OnAddOrUpdate ) != ValueGenerated.OnAddOrUpdate)
                      .ToDictionary(d => d.Name, d => d.GetColumnName(storeObjectIdentifier)) ?? new Dictionary<string, string?>();
    }

    private IReadOnlyList<IProperty> GetEntityPrimaryKeys()
    {
        using TContext context = _contextFactory.CreateDbContext();
        return context.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties ?? new List<IProperty>().AsReadOnly();
    }

    private string? GetEntitySchemaName()
    {
        using TContext context = _contextFactory.CreateDbContext();
        return context.Model.FindEntityType(typeof(TEntity))?.GetSchema();
    }

    private string? GetEntityTableName()
    {
        using TContext context = _contextFactory.CreateDbContext();
        return context.Model.FindEntityType(typeof(TEntity))?.GetTableName();
    }
}
