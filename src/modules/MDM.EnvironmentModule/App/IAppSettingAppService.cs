using Abp.Application.Services;
using AuthorizationModule.DataTransferObject.EnvironmentModule;

namespace AuthorizationModule.App
{
    public interface IAppSettingAppService : IAsyncCrudAppService<AppSettingDto, long, PagedAppSettingResultRequestDto, CreateAppSettingDto, AppSettingDto>
    {
    }
}
