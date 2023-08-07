using System.Collections.Generic;

namespace MDM.CatalogService.Html.Authentication.External
{
    public interface IExternalAuthConfiguration
    {
        List<ExternalLoginProviderInfo> Providers { get; }
    }
}
