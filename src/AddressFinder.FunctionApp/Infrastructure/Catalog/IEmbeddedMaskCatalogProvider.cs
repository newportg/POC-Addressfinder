using AddressFinder.FunctionApp.Domain.Models;

namespace AddressFinder.FunctionApp.Infrastructure.Catalog;

public interface IEmbeddedMaskCatalogProvider
{
    MaskResolutionResult Resolve(string countryCode);
    IReadOnlyDictionary<string, CountryMaskDefinition> GetAll();
}
