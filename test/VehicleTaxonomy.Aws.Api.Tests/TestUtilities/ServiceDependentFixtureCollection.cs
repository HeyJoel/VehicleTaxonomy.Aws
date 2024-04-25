using Xunit;

namespace VehicleTaxonomy.Aws.Api.Tests;

[CollectionDefinition(nameof(ServiceDependentFixture))]
public class ServiceDependentFixtureCollection : ICollectionFixture<ServiceDependentFixture>
{
}
