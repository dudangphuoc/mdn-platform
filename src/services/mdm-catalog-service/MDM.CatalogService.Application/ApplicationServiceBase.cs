using Abp.Application.Services;
using MDM.Shared.AuthorizationModule;

namespace MDM.CatalogService.Application;

public abstract class ApplicationServiceBase : ApplicationService
{
    protected ApplicationServiceBase()
    {
        LocalizationSourceName = GlobalConsts.LocalizationSourceName;
    }
}