using Abp.AutoMapper;
using MDM.CatalogService.Html.Authentication.External;

namespace MDM.CatalogService.Html.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
