using Abp.Extensions;
using AuthorizationModule.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MDM.EnvironmentModule
{
    static public class EntityExtension
    {
        static public void ApplyConceptsForAutoUpgradeVersion(EntityEntry entry)
        {
            if (entry.Entity is IAutoUpgradeVersion && !string.IsNullOrEmpty(entry.Entity.As<IAutoUpgradeVersion>().Version))
            {
                var version = new Version(entry.Entity.As<IAutoUpgradeVersion>().Version);
                entry.Entity.As<IAutoUpgradeVersion>().Version =
                    new Version(version.Major, version.Minor, (version.Build + 1)).ToString();
            }
        }

        static public void ApplyConceptsForAdd(EntityEntry entry)
        {
            if (entry.Entity is IAutoUpgradeVersion)
            {
                entry.Entity.As<IAutoUpgradeVersion>().Version = new Version(1, 0, 0).ToString();
            }
        }
    }
}
