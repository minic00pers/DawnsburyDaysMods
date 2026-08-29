using Dawnsbury.Core;
using Dawnsbury.Modding;

namespace InventorRemaster
{
    public class ModLoader
    {
        public const string UnstableContingenciesSetting = "Dawnsbury.Inventor.Remaster.UnstableContingencies";

        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            ModManager.RegisterBooleanSettingsOption(
                UnstableContingenciesSetting,
                "Inventor Remaster: Enable Unstable Contingencies",
                "Grants Inventors the homebrew Unstable Contingencies class feature at level 3. Unstable actions are locked after the second failed unstable check, or after the third failed check at level 7.",
                false);

            Dawnsbury.Core.CharacterBuilder.FeatsDb.AllFeats.All.AddRange(InventorRemaster.LoadAll());
        }
    }
}
