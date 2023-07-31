using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorizationModule.Entities
{
    [Table("AppSettings")]
    public class AppSetting : EntitySettingBase, IAutoUpgradeVersion
    {
        public byte[] Values { get; set; }

        public long EnvironmentId { get; set; }

        [ForeignKey(nameof(EnvironmentId))]
        public Environment Environment { get; set; }

        [MaxLength(128)]
        public string Version { get; set; }

        override public void SetNormalizedNames()
        {
            NormalizedName = Name.ToUpperInvariant();
            NormalizedDescription = Description.ToUpperInvariant();
        }
    }
}

