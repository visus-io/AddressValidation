namespace AddressValidation.Demo.Infrastructure.Repositories;

using Abstractions;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

public sealed class StateRepository(IDbContextFactory<GeoContext> contextFactory, ILogger<SqlRepository<StateModel, GeoContext>> logger)
    : SqlRepository<StateModel, GeoContext>(contextFactory, logger), IStateRepository;
