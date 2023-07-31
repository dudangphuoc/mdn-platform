using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorizationModule.Entities
{
    /// <summary>
    /// Base class for App.
    /// </summary>
    [Table("Application")]
    public class Application : EntitySettingBase
    {
        public override string Name { get => base.Name; set => base.Name = value; }
        public ICollection<Environment> Environments { get; set; }

        override public void SetNormalizedNames()
        {
            NormalizedName = Name.ToUpperInvariant();
            NormalizedDescription = Description.ToUpperInvariant();
        }
    }
}
