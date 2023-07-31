using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace AuthorizationModule.Entities
{
    public abstract class EntitySettingBase : FullAuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxNameLength = 256;

        public const int MaxDescriptionLength = 512;

        [Required]
        [MaxLength(MaxNameLength)]
        public virtual string Name { get; set; }

        [Required]
        [MaxLength(MaxDescriptionLength)]
        public string? Description { get; set; }


        [Required]
        [MaxLength(MaxNameLength)]
        public virtual string NormalizedName { get; set; }

        [Required]
        [MaxLength(MaxDescriptionLength)]
        public virtual string NormalizedDescription { get; set; }

        public int? TenantId { get; set; }

        public virtual void SetNormalizedNames()
        {
            NormalizedName = Name.ToUpperInvariant();
            NormalizedDescription = Description.ToUpperInvariant();
        }
    }
}
