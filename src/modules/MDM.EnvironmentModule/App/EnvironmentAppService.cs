using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using MDMLibrary.Crypto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorizationModule.DataTransferObject.EnvironmentModule;
using Environment = AuthorizationModule.Entities.Environment;

namespace AuthorizationModule.App
{
    [AbpAuthorize]
    public class EnvironmentAppService :
        AsyncCrudAppService<Environment, EnvironmentDto, long, PagedEnvironmentResultRequestDto,
            CreateEnvironmentDto, EnvironmentDto>, IEnvironmentAppService
    {
        public EnvironmentAppService(IRepository<Environment, long> repository) : base(repository)
        {
        }

        protected override IQueryable<Environment> CreateFilteredQuery(PagedEnvironmentResultRequestDto input)
        {

            return Repository.GetAllIncluding()
                .Include(x => x.AppSettings)
                .Include(x => x.Application)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Name), x => x.Name.Contains(input.Name))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Description), x => x.Description.Contains(input.Description));
        }

        /// <summary>
        /// Get Values
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("~/connect/case/{code}")]
        [AllowAnonymous]
        public async Task<List<ApplicationSettingDto>> GetApplicationSetingsAsync(string code)
        {
            UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            List<ApplicationSettingDto> values = new List<ApplicationSettingDto>();
            //int? currentTenantId = UnitOfWorkManager.Current.GetTenantId();
            var entity = await Repository.GetAllIncluding(x => x.AppSettings)
                //.WhereIf(currentTenantId.HasValue, x => x.TenantId == currentTenantId)
                .Where(x => x.Code == code)
                .FirstOrDefaultAsync();

            if (entity != null && entity.AppSettings != null)
            {
                values.AddRange(entity.AppSettings.Select(x => new ApplicationSettingDto()
                {
                    Name = x.NormalizedName,
                    Version = x.Version,
                    Data = Convert.ToBase64String(x.Values)
                }));
            }

            return values;
        }

        public override async Task<EnvironmentDto> CreateAsync(CreateEnvironmentDto input)
        {
            if (string.IsNullOrEmpty(input.Description))
            {
                input.Description = input.Name;
            }
            var environment = ObjectMapper.Map<Environment>(input);
            var crypto = new CryptoAes();

            var secretPlainText = crypto.ExportKey();
            environment.Secret = secretPlainText;
            environment.SecretEncode = CryptoNetUtils.Base64Encode(secretPlainText);
            environment.SetNormalizedNames();
            environment.Code = string.Format("{0}-{1:N}", environment.NormalizedName, Guid.NewGuid()).ToUpperInvariant();
            int? currentTenantId = UnitOfWorkManager.Current.GetTenantId();
            if (currentTenantId.HasValue && !environment.TenantId.HasValue)
            {
                environment.TenantId = currentTenantId.Value;
            }

            await Repository.InsertOrUpdateAndGetIdAsync(environment);

            return MapToEntityDto(environment);
        }

        public override async Task<EnvironmentDto> UpdateAsync(EnvironmentDto input)
        {
            CheckUpdatePermission();

            var entity = await GetEntityByIdAsync(input.Id);

            MapToEntity(input, entity);

            if (string.IsNullOrEmpty(entity.SecretEncode))
            {
                entity.SecretEncode = CryptoNetUtils.Base64Encode(entity.Secret);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

    }
}
