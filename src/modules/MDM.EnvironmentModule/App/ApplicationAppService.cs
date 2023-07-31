using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using AuthorizationModule.Entities;
using Microsoft.EntityFrameworkCore;
using AuthorizationModule.DataTransferObject.EnvironmentModule;

namespace AuthorizationModule.App
{
    [AbpAuthorize]
    public class ApplicationAppService :
        AsyncCrudAppService<Application, ApplicationDto, long, PagedApplicationResultRequestDto, CreateApplicationDto, ApplicationDto>, IApplicationAppService
    {
        public ApplicationAppService(IRepository<Application, long> repository) : base(repository)
        {
        }

        public override Task<PagedResultDto<ApplicationDto>> GetAllAsync(PagedApplicationResultRequestDto input)
        {
            return base.GetAllAsync(input);
        }
        protected override IQueryable<Application> CreateFilteredQuery(PagedApplicationResultRequestDto input)
        {
            return Repository.GetAllIncluding().Include(x => x.Environments)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Name), x => x.NormalizedName.Contains(input.Name.Normalize()));
        }

        public override async Task<ApplicationDto> CreateAsync(CreateApplicationDto input)
        {
            var application = ObjectMapper.Map<Application>(input);
            application.SetNormalizedNames();
            if (string.IsNullOrEmpty(input.Description))
            {
                input.Description = input.Name;
            }

            int? currentTenantId = UnitOfWorkManager.Current.GetTenantId();
            if (currentTenantId.HasValue && !application.TenantId.HasValue)
            {
                application.TenantId = currentTenantId.Value;
            }

            await Repository.InsertAndGetIdAsync(application);

            return MapToEntityDto(application);
        }

        public override async Task<ApplicationDto> UpdateAsync(ApplicationDto input)
        {
            var entity = await Repository.GetAsync(input.Id);

            MapToEntity(input, entity);
            entity.SetNormalizedNames();
            await CurrentUnitOfWork.SaveChangesAsync();
            return MapToEntityDto(entity);
        }

    }
}
