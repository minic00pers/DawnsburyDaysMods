using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;

namespace Dawnsbury.Impossible.Archetypes;

/// <summary>
/// Shared registered data for every archetype in this mod.
/// </summary>
public static class ModData
{
    public const string IdPrefix = "Dawnsbury.Impossible.Archetypes.";

    public static Trait ModTrait { get; private set; }

    public static void Load()
    {
        ModTrait = ModManager.ModBeingLoadedTrait!.Value;
        Traits.Initialize();
        FeatNames.Initialize();
        ActionIds.Initialize();
        QEffectIds.Initialize();
    }

    public static class Traits
    {
        public static Trait WorldRouser { get; private set; }

        internal static void Initialize()
        {
            WorldRouser = ModManager.RegisterTrait(
                IdPrefix + "WorldRouser",
                new TraitProperties("World Rouser", relevant: true));
        }
    }

    public static class ActionIds
    {
        public static ActionId RouseTheWorld { get; private set; }
        public static ActionId AllReturnsToSlumber { get; private set; }
        public static ActionId DustCloud { get; private set; }
        public static ActionId WakeAndTremble { get; private set; }

        internal static void Initialize()
        {
            RouseTheWorld = ModManager.RegisterEnumMember<ActionId>(IdPrefix + "RouseTheWorld");
            AllReturnsToSlumber = ModManager.RegisterEnumMember<ActionId>(IdPrefix + "AllReturnsToSlumber");
            DustCloud = ModManager.RegisterEnumMember<ActionId>(IdPrefix + "DustCloud");
            WakeAndTremble = ModManager.RegisterEnumMember<ActionId>(IdPrefix + "WakeAndTremble");
        }
    }

    public static class FeatNames
    {
        public static FeatName WorldRouserDedication { get; internal set; }
        public static FeatName NaturesEmbrace { get; private set; }
        public static FeatName TheWorldWhispers { get; private set; }
        public static FeatName AllReturnsToSlumber { get; private set; }
        public static FeatName ShelteringHand { get; private set; }
        public static FeatName DustCloud { get; private set; }
        public static FeatName WakeAndTremble { get; private set; }

        internal static void Initialize()
        {
            NaturesEmbrace = ModManager.RegisterFeatName(
                IdPrefix + "NaturesEmbrace",
                "Nature's Embrace");
            TheWorldWhispers = ModManager.RegisterFeatName(
                IdPrefix + "TheWorldWhispers",
                "The World Whispers");
            AllReturnsToSlumber = ModManager.RegisterFeatName(
                IdPrefix + "AllReturnsToSlumber",
                "All Returns to Slumber");
            ShelteringHand = ModManager.RegisterFeatName(
                IdPrefix + "ShelteringHand",
                "Sheltering Hand");
            DustCloud = ModManager.RegisterFeatName(
                IdPrefix + "DustCloud",
                "Dust Cloud");
            WakeAndTremble = ModManager.RegisterFeatName(
                IdPrefix + "WakeAndTremble",
                "Wake and Tremble");
        }
    }

    public static class QEffectIds
    {
        public static QEffectId WakingWorld { get; private set; }
        public static QEffectId WakingWorldBonus { get; private set; }
        public static QEffectId NaturesEmbrace { get; private set; }
        public static QEffectId AllReturnsToSlumberImmunity { get; private set; }
        public static QEffectId ShelteringHand { get; private set; }
        public static QEffectId DustCloud { get; private set; }
        public static QEffectId DustCloudCondition { get; private set; }

        internal static void Initialize()
        {
            WakingWorld = ModManager.RegisterEnumMember<QEffectId>(IdPrefix + "WakingWorld");
            WakingWorldBonus = ModManager.RegisterEnumMember<QEffectId>(IdPrefix + "WakingWorldBonus");
            NaturesEmbrace = ModManager.RegisterEnumMember<QEffectId>(IdPrefix + "NaturesEmbrace");
            AllReturnsToSlumberImmunity = ModManager.RegisterEnumMember<QEffectId>(IdPrefix + "AllReturnsToSlumberImmunity");
            ShelteringHand = ModManager.RegisterEnumMember<QEffectId>(IdPrefix + "ShelteringHand");
            DustCloud = ModManager.RegisterEnumMember<QEffectId>(IdPrefix + "DustCloud");
            DustCloudCondition = ModManager.RegisterEnumMember<QEffectId>(IdPrefix + "DustCloudCondition");
        }
    }
}
