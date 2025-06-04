namespace AddressValidation.Demo.Infrastructure.Repositories;

using Abstractions;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

public sealed class CountryRepository(IDbContextFactory<GeoContext> contextFactory, ILogger<CountryRepository> logger)
    : SqlRepository<CountryModel, GeoContext>(contextFactory, logger), ICountryRepository;
