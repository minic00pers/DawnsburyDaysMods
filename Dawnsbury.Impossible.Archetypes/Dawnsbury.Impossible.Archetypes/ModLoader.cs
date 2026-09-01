using Dawnsbury.Impossible.Archetypes.Archetypes;
using Dawnsbury.Modding;

namespace Dawnsbury.Impossible.Archetypes;

public static class ModLoader
{
    [DawnsburyDaysModMainMethod]
    public static void LoadMod()
    {
        ModData.Load();

        WorldRouser.Load();

        // HedgeMage.Load();
    }
}
