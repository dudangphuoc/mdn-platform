using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using AuthorizationModule.Entities;
using Environment = AuthorizationModule.Entities.Environment;


namespace AuthorizationModule.DataTransferObject.EnvironmentModule
{
    public class PagedResultRequestBase : PagedResultRequestDto
    {
        [MaxLength(EntitySettingBase.MaxNameLength)]
        public string? Name { get; set; }

        [MaxLength(EntitySettingBase.MaxDescriptionLength)]
        public string? Description { get; set; }
    }

    public class PagedApplicationResultRequestDto : PagedResultRequestBase
    {

    }

    [AutoMap(typeof(Environment))]
    public class PagedEnvironmentResultRequestDto : PagedResultRequestBase
    {
        public string SecretEncode { get; set; }
    }

    public class PagedAppSettingResultRequestDto : PagedResultRequestBase
    {

    }

}