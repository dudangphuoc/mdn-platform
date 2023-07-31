using Abp.Application.Services;
using AuthorizationModule.DataTransferObject.EnvironmentModule;

namespace AuthorizationModule.App
{
    public interface IEnvironmentAppService :
        IAsyncCrudAppService<EnvironmentDto, long, PagedEnvironmentResultRequestDto,
            CreateEnvironmentDto, EnvironmentDto>
    {
        Task<List<ApplicationSettingDto>> GetApplicationSetingsAsync(string code);
    }
}
