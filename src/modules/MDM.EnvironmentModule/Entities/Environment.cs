using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorizationModule.Entities
{
    [AutoMapFrom(typeof(System.Environment))]
    public class Environment : EntitySettingBase
    {
        public long ApplicationId { get; set; }

        [MaxLength(512)]
        public string Code { get; set; }

        [ForeignKey(nameof(ApplicationId))]
        public Application Application { get; set; }

        public string Secret { get; set; }

        public string SecretEncode { get; set; }

        public ICollection<AppSetting> AppSettings { get; set; }

        override public void SetNormalizedNames()
        {
            NormalizedName = Name.ToUpperInvariant().Replace(" ", "-");
            NormalizedDescription = Description.ToUpperInvariant().Replace(" ", "-");
        }

    }
}
