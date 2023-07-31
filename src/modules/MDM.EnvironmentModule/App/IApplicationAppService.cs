using Abp.Application.Services;
using AuthorizationModule.DataTransferObject.EnvironmentModule;

namespace AuthorizationModule.App
{
    public interface IApplicationAppService : IAsyncCrudAppService<ApplicationDto, long, PagedApplicationResultRequestDto, CreateApplicationDto, ApplicationDto>
    {
    }
}
