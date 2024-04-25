namespace VehicleTaxonomy.Aws.Domain.Tests;

[CollectionDefinition(nameof(DbDependentFixtureCollection))]
public class DbDependentFixtureCollection : ICollectionFixture<DbDependentFixture>
{
}
