using Abp.AutoMapper;
using Abp.Runtime.Validation;
using AuthorizationModule.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using MDM.Common;

namespace AuthorizationModule.DataTransferObject.EnvironmentModule
{

    [AutoMapTo(typeof(AppSetting))]
    public class CreateAppSettingDto : IShouldNormalize, IValidatableObject
    {
        [Required]
        public long EnvironmentId { get; set; }

        [Required]
        [MaxLength(EntitySettingBase.MaxNameLength)]
        public string Name { get; set; }

        [MaxLength(EntitySettingBase.MaxDescriptionLength)]
        public string? Description { get; set; }

        [Required]
        public string JsonValues { get; set; }

        public void Normalize()
        {
            if (Name != null)
            {
                Name = Name.Trim();
            }

            if (Description != null)
            {
                Description = Description.Trim();
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(JsonValues) || string.IsNullOrWhiteSpace(JsonValues) || !JsonValues.TryParseJson(out object result))
                yield return new ValidationResult("Values is required", new[] { nameof(JsonValues) });
        }


    }
}
