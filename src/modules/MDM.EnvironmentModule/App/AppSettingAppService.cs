using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using AuthorizationModule.Entities;
using Microsoft.EntityFrameworkCore;
using AuthorizationModule.DataTransferObject.EnvironmentModule;
using MDMLibrary.Crypto;
using Environment = AuthorizationModule.Entities.Environment;


namespace AuthorizationModule.App
{

    [AbpAuthorize]
    public class AppSettingAppService :
        AsyncCrudAppService<AppSetting, AppSettingDto, long, PagedAppSettingResultRequestDto,
            CreateAppSettingDto, AppSettingDto>, IAppSettingAppService
    {
        public AppSettingAppService(IRepository<AppSetting, long> repository) : base(repository)
        {

        }

        protected override IQueryable<AppSetting> CreateFilteredQuery(PagedAppSettingResultRequestDto input)
        {
            return base.CreateFilteredQuery(input).Include(x => x.Environment)
                .ThenInclude(x => x.Application)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Name), x => x.Name.Contains(input.Name))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Description), x => x.Description.Contains(input.Description));
        }

        public override async Task<AppSettingDto> CreateAsync(CreateAppSettingDto input)
        {
            if (string.IsNullOrEmpty(input.Description))
            {
                input.Description = input.Name;
            }
            var appSetting = ObjectMapper.Map<AppSetting>(input);
            appSetting.SetNormalizedNames();


            using (var repository = IocManager.Instance.ResolveAsDisposable<IRepository<Environment, long>>())
            {
                var environment = await repository.Object.FirstOrDefaultAsync(x => x.Id == input.EnvironmentId);
                if (environment == null)
                {
                    throw new UserFriendlyException("Environment not found");
                }
                var crypto = new CryptoAes(environment.Secret);
                appSetting.Values = crypto.EncryptFromString(input.JsonValues);
            }

            int? currentTenantId = UnitOfWorkManager.Current.GetTenantId();
            if (currentTenantId.HasValue && !appSetting.TenantId.HasValue)
            {
                appSetting.TenantId = currentTenantId.Value;
            }

            await Repository.InsertOrUpdateAndGetIdAsync(appSetting);

            return MapToEntityDto(appSetting);
        }

        public async override Task<AppSettingDto> GetAsync(EntityDto<long> input)
        {
            var entity = await GetEntityByIdAsync(input.Id);
            var result = MapToEntityDto(entity);
            result.JsonValues = await MapToValue(entity.Values, entity.Environment.Secret);

            return result;
        }

        protected override async Task<AppSetting> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding().Include(x => x.Environment).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(AppSetting), id);
            }

            return user;
        }

        public override Task<PagedResultDto<AppSettingDto>> GetAllAsync(PagedAppSettingResultRequestDto input)
        {
            return base.GetAllAsync(input);
        }

        public override async Task<AppSettingDto> UpdateAsync(AppSettingDto input)
        {
            var entity = await GetEntityByIdAsync(input.Id);

            entity.SetNormalizedNames();
            var result = MapToEntityDto(entity);
            using (var repository = IocManager.Instance.ResolveAsDisposable<IRepository<Environment, long>>())
            {
                var environment = await repository.Object.FirstOrDefaultAsync(x => x.Id == input.EnvironmentId);
                if (environment == null)
                {
                    throw new UserFriendlyException("Environment not found");
                }
                var crypto = new CryptoAes(environment.Secret);
                entity.Values = crypto.EncryptFromString(input.JsonValues);
            }
            //var crypto = new CryptoAes(entity.Environment.Secret);
            //entity.Values = crypto.EncryptFromString(input.JsonValues);
            MapToEntity(input, entity);
            Repository.Update(entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            result.JsonValues = input.JsonValues;
            return result;
        }




        private async Task<string> MapToValue(byte[] jsonValues, long environmentId)
        {
            using (var repository = IocManager.Instance.ResolveAsDisposable<IRepository<Environment, long>>())
            {
                var environment = await repository.Object.FirstOrDefaultAsync(x => x.Id == environmentId);
                if (environment == null)
                {
                    throw new UserFriendlyException("Environment not found");
                }
                var crypto = new CryptoAes(environment.Secret);
                return crypto.DecryptToString(jsonValues);
            }
        }

        private async Task<string> MapToValue(byte[] jsonValues, string secret)
        {
            var crypto = new CryptoAes(secret);
            return crypto.DecryptToString(jsonValues);
        }

    }
}
