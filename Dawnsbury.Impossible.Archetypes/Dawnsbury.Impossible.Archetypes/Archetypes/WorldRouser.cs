using Dawnsbury.Audio;
using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.Archetypes;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Zoning;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Modding;
using Microsoft.Xna.Framework;

namespace Dawnsbury.Impossible.Archetypes.Archetypes;

/// <summary>
/// Implements the World Rouser dedication and all World Rouser archetype feats.
/// </summary>
public static class WorldRouser
{
    private const int InitialRadius = 2;
    private const int RadiusIncrease = 2;
    private const string RecallWeaknessActionId = "RecallWeaknessActionID";
    private const string WorldWhispersConcealmentKey = ModData.IdPrefix + "WorldWhispersConcealment";

    internal static void Load()
    {
        foreach (Feat feat in CreateFeats())
        {
            ModManager.AddFeat(feat);
        }
    }

    public static IEnumerable<Feat> CreateFeats()
    {
        Feat dedication = ArchetypeFeats.CreateAgnosticArchetypeDedication(
                ModData.Traits.WorldRouser,
                "You know how to wake the world around you from its slumber, whether through your natural affinity for nature or hard-earned knowledge.",
                """
                You gain the Rouse the World action.

                The DC for an ability gained through this archetype is your class DC or spell DC, whichever is higher. This is your world rouser DC.
                """,
                null)
            .WithPrerequisite(
                values => values.GetProficiency(Trait.Nature) >= Proficiency.Trained,
                "You must be trained in Nature.")
            .WithRulesBlockForCombatAction(CreateRouseTheWorldAction)
            .WithPermanentQEffect(
                "You can use Rouse the World to create a waking world that aids you and your allies.",
                qfFeat =>
                {
                    qfFeat.Name = "World Rouser Dedication";
                    qfFeat.ProvideMainAction = qfThis =>
                        new ActionPossibility(CreateRouseTheWorldAction(qfThis.Owner));
                });

        ModData.FeatNames.WorldRouserDedication = dedication.FeatName;
        yield return dedication;

        yield return new TrueFeat(
                ModData.FeatNames.NaturesEmbrace,
                4,
                "You call on the world to hamper your foes. Plants grab at them, wind or water currents buffet them, and the footing shifts under their feet.",
                "Your waking world becomes difficult terrain for your enemies until the start of your next turn.",
                [Trait.Primal])
            .WithActionCost(1)
            .WithAvailableAsArchetypeFeat(ModData.Traits.WorldRouser)
            .WithRulesBlockForCombatAction(CreateNaturesEmbraceAction)
            .WithPermanentQEffect(
                "You can make your waking world difficult terrain for enemies until the start of your next turn.",
                qfFeat =>
                {
                    qfFeat.Name = "Nature's Embrace";
                    qfFeat.ProvideMainAction = qfThis =>
                        new ActionPossibility(CreateNaturesEmbraceAction(qfThis.Owner));
                });

        yield return new TrueFeat(
                ModData.FeatNames.TheWorldWhispers,
                4,
                "Your waking world tells you about the unseen creatures it holds.",
                "Seek throughout every square of your waking world. On a successful check against a creature, improve its visibility to your party by one step: undetected to hidden, hidden to concealed, or concealed to normally detected.",
                [Trait.Primal])
            .WithActionCost(1)
            .WithAvailableAsArchetypeFeat(ModData.Traits.WorldRouser)
            .WithRulesBlockForCombatAction(CreateTheWorldWhispersAction)
            .WithPermanentQEffect(
                "You can Seek throughout your entire waking world and reveal detected creatures one step at a time.",
                qfFeat =>
                {
                    qfFeat.Name = "The World Whispers";
                    qfFeat.ProvideMainAction = qfThis =>
                        new ActionPossibility(CreateTheWorldWhispersAction(qfThis.Owner));
                });

        yield return new TrueFeat(
                ModData.FeatNames.AllReturnsToSlumber,
                6,
                "Your waking world returns to slumber and lays the peace of deep sleep on all within it.",
                "Dismiss your waking world. Every creature in the area attempts a Will save against your world rouser DC. Animals, beasts, and plants worsen their result by one degree. On a failure, a creature can't use reactions for 1 round. On a critical failure, it is also slowed 1. Each creature then becomes temporarily immune for 1 hour.",
                [Trait.Mental, Trait.Primal, Trait.Sleep])
            .WithActionCost(1)
            .WithAvailableAsArchetypeFeat(ModData.Traits.WorldRouser)
            .WithRulesBlockForCombatAction(CreateAllReturnsToSlumberAction)
            .WithPermanentQEffect(
                "You can dismiss your waking world to suppress the creatures within it.",
                qfFeat =>
                {
                    qfFeat.Name = "All Returns to Slumber";
                    qfFeat.ProvideMainAction = qfThis =>
                        new ActionPossibility(CreateAllReturnsToSlumberAction(qfThis.Owner));
                });

        yield return new TrueFeat(
                ModData.FeatNames.ShelteringHand,
                6,
                "Animals, plants, and the earth itself rush to protect you from harm, interposing themselves between you and danger.",
                "You and your allies in your waking world gain cover from enemies you are aware of until the start of your next turn. Spend 1 action for lesser cover or 2 actions for standard cover.",
                [Trait.Primal])
            .WithAvailableAsArchetypeFeat(ModData.Traits.WorldRouser)
            .WithPermanentQEffect(
                "You can call on your waking world to grant lesser or standard cover to you and your allies.",
                qfFeat =>
                {
                    qfFeat.Name = "Sheltering Hand";
                    qfFeat.ProvideMainAction = qfThis =>
                        CreateShelteringHandPossibility(qfThis.Owner);
                });

        yield return new TrueFeat(
                ModData.FeatNames.DustCloud,
                8,
                "Dust and pollen fill your waking world, obscuring creatures from sight and choking those who linger within.",
                "All creatures in your waking world become concealed, and creatures outside it become concealed to creatures within it, until the start of your next turn. A creature that ends its turn in the area must attempt a Fortitude save against your world rouser DC. On a failure, it is dazzled until the end of its next turn; on a critical failure, it is blinded instead.",
                [Trait.Primal])
            .WithActionCost(1)
            .WithAvailableAsArchetypeFeat(ModData.Traits.WorldRouser)
            .WithRulesBlockForCombatAction(CreateDustCloudAction)
            .WithPermanentQEffect(
                "You can fill your waking world with concealing dust and pollen.",
                qfFeat =>
                {
                    qfFeat.Name = "Dust Cloud";
                    qfFeat.ProvideMainAction = qfThis =>
                        new ActionPossibility(CreateDustCloudAction(qfThis.Owner));
                });

        yield return new TrueFeat(
                ModData.FeatNames.WakeAndTremble,
                8,
                "You have the world wake with a great convulsion.",
                "Trigger You Rouse the World. All other creatures in the area attempt a Reflex save against your world rouser DC. On a failure, a creature falls prone. On a critical failure, it also becomes sickened 1.",
                [Trait.Primal])
            .WithActionCost(0)
            .WithAvailableAsArchetypeFeat(ModData.Traits.WorldRouser)
            .WithRulesBlockForCombatAction(CreateWakeAndTrembleAction);

        // Milestones 8-11: remaining World Rouser feats, kept in level order in this file.
    }

    #region Waking World Infrastructure

    private sealed class WakingWorldState(
        CombatAction rouseAction,
        Zone zone,
        Point origin,
        int radius)
    {
        public CombatAction RouseAction { get; } = rouseAction;
        public Zone Zone { get; } = zone;
        public Point Origin { get; } = origin;
        public int Radius { get; set; } = radius;
        public bool ExpandedThisTurn { get; set; } = true;
    }

    internal static int GetWorldRouserDC(Creature creature)
    {
        return creature.ClassOrSpellDC();
    }

    private static QEffect CreateWakingWorldController(Creature owner)
    {
        return new QEffect(
            "Waking World",
            WakingWorldDescription(InitialRadius),
            ExpirationCondition.Never,
            owner,
            IllustrationName.TerrainTransposition)
        {
            Id = ModData.QEffectIds.WakingWorld,
            CountsAsABuff = true,
            CountsAsBeneficialToSource = true,
            StartOfYourEveryTurn = (qfThis, _) =>
            {
                if (qfThis.Tag is WakingWorldState state)
                {
                    state.ExpandedThisTurn = false;
                }

                return Task.CompletedTask;
            },
            ProvideContextualAction = qfThis =>
            {
                if (qfThis.Tag is not WakingWorldState state || state.ExpandedThisTurn)
                {
                    return null;
                }

                return new ActionPossibility(CreateSustainAction(qfThis, state))
                    .WithPossibilityGroup("Maintain an activity");
            }
        };
    }

    private static QEffect CreateWakingWorldBonus(Creature source, Creature recipient, Zone zone)
    {
        return new QEffect(
            "Waking World",
            "You gain a +1 circumstance bonus to Athletics and Acrobatics checks while in the waking world.",
            ExpirationCondition.Ephemeral,
            source,
            IllustrationName.TerrainTransposition)
        {
            Id = ModData.QEffectIds.WakingWorldBonus,
            CountsAsABuff = true,
            BonusToSkillChecks = (skill, action, target) =>
            {
                if (skill is Skill.Athletics or Skill.Acrobatics)
                {
                    return new Bonus(1, BonusType.Circumstance, "waking world");
                }

                if (recipient != source
                    || skill != Skill.Nature
                    || !IsCreatureOrHazardRecallAction(action)
                    || !RecallKnowledgeSubjectIsInZone(action, target, zone))
                {
                    return null;
                }

                return new Bonus(1, BonusType.Circumstance, "waking world");
            }
        };
    }

    private static string WakingWorldDescription(int radius)
    {
        return $"The world is awake in a {radius * 5}-foot burst. You and your allies in the area gain a +1 circumstance bonus to Athletics and Acrobatics checks.";
    }

    #endregion

    #region Action Factories

    private static CombatAction CreateRouseTheWorldAction(Creature owner)
    {
        CombatAction action = new CombatAction(
                owner,
                IllustrationName.TerrainTransposition,
                "Rouse the World",
                [
                    ModData.ModTrait,
                    ModData.Traits.WorldRouser,
                    Trait.Archetype,
                    Trait.Concentrate,
                    Trait.Primal
                ],
                """
                The flora, fauna, and elements in a 10-foot burst within 30 feet stir, creating your waking world for 10 minutes.

                While in your waking world, you and your allies gain a +1 circumstance bonus to Athletics and Acrobatics checks. You also gain a +1 circumstance bonus to Nature checks to Recall Knowledge about creatures and hazards in the area.

                Once per round on subsequent turns, you can Sustain this ability to increase the burst's size by 10 feet. You can have only one waking world; using this action again ends the previous one.
                """,
                Target.Burst(6, InitialRadius))
            .WithActionCost(1)
            .WithActionId(ModData.ActionIds.RouseTheWorld)
            .WithSoundEffect(SfxName.ElementalBlastEarth);

        action.WithEffectOnChosenTargets(async (rouseAction, self, chosenTargets) =>
        {
            self.RemoveAllQEffects(qf => qf.Id == ModData.QEffectIds.WakingWorld);

            List<Tile> initialTiles = chosenTargets.ChosenTiles
                .Where(tile => !tile.AlwaysBlocksMovement)
                .ToList();
            QEffect controller = CreateWakingWorldController(self);
            Zone zone = Zone.SpawnStaticAndApply(controller, initialTiles, wakingWorldZone =>
            {
                wakingWorldZone.TileEffectCreator = tile =>
                    CreateWakingWorldTileEffect(tile, self, controller);
                wakingWorldZone.StateCheckOnEachCreatureInZone = (_, creature) =>
                {
                    if (creature.FriendOf(self))
                    {
                        creature.AddQEffect(CreateWakingWorldBonus(self, creature, wakingWorldZone));
                    }
                };
            });

            WakingWorldState state = new(
                rouseAction,
                zone,
                chosenTargets.ChosenPointOfOrigin,
                InitialRadius);
            controller.Tag = state;
            zone.ApplyDismissible(rouseAction, "Dismiss your waking world, returning the area to normal.");
            self.AddQEffect(controller);

            await TryUseWakeAndTremble(self, chosenTargets);
        });

        return action;
    }

    private static CombatAction CreateSustainAction(QEffect controller, WakingWorldState state)
    {
        return new CombatAction(
                controller.Owner,
                IllustrationName.TerrainTransposition,
                "Sustain Rouse the World",
                [
                    ModData.ModTrait,
                    ModData.Traits.WorldRouser,
                    Trait.Archetype,
                    Trait.Concentrate,
                    Trait.Primal,
                    Trait.Basic
                ],
                "Increase the radius of your waking world by 10 feet.",
                Target.Self())
            .WithActionCost(1)
            .WithSoundEffect(SfxName.ElementalBlastEarth)
            .WithEffectOnSelf(_ =>
            {
                state.ExpandedThisTurn = true;
                state.Radius += RadiusIncrease;

                BurstAreaTarget burstTarget = (BurstAreaTarget)state.RouseAction.Target;
                burstTarget.Radius = state.Radius;
                AreaSelection? expandedArea = Areas.DetermineTiles(
                    burstTarget,
                    state.Origin,
                    ignoreBurstOriginLoS: true);

                if (expandedArea is not null)
                {
                    state.Zone.MoveTo(expandedArea.TargetedTiles
                        .Where(tile => !tile.AlwaysBlocksMovement)
                        .ToList());
                }

                controller.Description = WakingWorldDescription(state.Radius);
            });
    }

    private static CombatAction CreateNaturesEmbraceAction(Creature owner)
    {
        return new CombatAction(
                owner,
                IllustrationName.NewGrass,
                "Nature's Embrace",
                [
                    ModData.ModTrait,
                    ModData.Traits.WorldRouser,
                    Trait.Archetype,
                    Trait.Primal
                ],
                "Your waking world becomes difficult terrain for your enemies until the start of your next turn.",
                Target.Self().WithAdditionalRestriction(self =>
                    self.QEffects.Any(qf =>
                        qf.Id == ModData.QEffectIds.WakingWorld
                        && qf.Tag is WakingWorldState)
                        ? null
                        : "Your waking world must be active."))
            .WithActionCost(1)
            .WithSoundEffect(SfxName.ElementalBlastEarth)
            .WithEffectOnSelf(self =>
            {
                QEffect? wakingWorld = self.QEffects.FirstOrDefault(qf =>
                    qf.Id == ModData.QEffectIds.WakingWorld
                    && qf.Tag is WakingWorldState);
                if (wakingWorld?.Tag is not WakingWorldState)
                {
                    return;
                }

                self.RemoveAllQEffects(qf => qf.Id == ModData.QEffectIds.NaturesEmbrace);
                self.AddQEffect(new QEffect(
                    "Nature's Embrace",
                    "Your waking world is difficult terrain for your enemies.",
                    ExpirationCondition.ExpiresAtStartOfYourTurn,
                    self,
                    IllustrationName.NewGrass)
                {
                    Id = ModData.QEffectIds.NaturesEmbrace,
                    CountsAsABuff = true,
                    Tag = wakingWorld,
                    StateCheck = qfThis =>
                    {
                        if (!qfThis.Owner.QEffects.Contains(wakingWorld))
                        {
                            qfThis.ExpiresAt = ExpirationCondition.Immediately;
                        }
                    }
                });
            });
    }

    private static CombatAction CreateTheWorldWhispersAction(Creature owner)
    {
        return new CombatAction(
                owner,
                IllustrationName.Seek,
                "The World Whispers",
                [
                    ModData.ModTrait,
                    ModData.Traits.WorldRouser,
                    Trait.Archetype,
                    Trait.Primal,
                    Trait.Secret,
                    Trait.Basic,
                    Trait.IsNotHostile,
                    Trait.DoesNotBreakStealth,
                    Trait.AttackDoesNotTargetAC,
                    Trait.UsesPerception
                ],
                "Seek throughout every square of your waking world. On a successful check against a creature, improve its visibility to your party by one step: undetected to hidden, hidden to concealed, or concealed to normally detected.",
                Target.Self().WithAdditionalRestriction(self =>
                    self.QEffects.Any(qf =>
                        qf.Id == ModData.QEffectIds.WakingWorld
                        && qf.Tag is WakingWorldState)
                        ? null
                        : "Your waking world must be active."))
            .WithActionCost(1)
            .WithActionId(ActionId.Seek)
            .WithSoundEffect(SfxName.OpenPage)
            .WithEffectOnSelf(async (worldWhispers, self) =>
            {
                QEffect? wakingWorld = self.QEffects.FirstOrDefault(qf =>
                    qf.Id == ModData.QEffectIds.WakingWorld
                    && qf.Tag is WakingWorldState);
                if (wakingWorld?.Tag is not WakingWorldState state)
                {
                    return;
                }

                CombatAction creatureSeek = CreateWorldWhispersSeekCheck(
                    self,
                    TaggedChecks.DefenseDC(Defense.Stealth));
                List<Creature> creaturesToSeek = state.Zone.CreaturesInZone
                    .Where(creature =>
                        creature.EnemyOf(self)
                        && NeedsWorldWhispersDetectionStep(self, creature))
                    .Distinct()
                    .ToList();
                foreach (Creature target in creaturesToSeek)
                {
                    CheckBreakdown breakdown = CombatActionExecution.BreakdownAttack(creatureSeek, target);
                    CheckBreakdownResult result = new(breakdown);
                    if (result.CheckResult >= CheckResult.Success)
                    {
                        ApplyWorldWhispersDetectionStep(self, target);
                    }
                }

                await SeekHiddenTileEffects(self, state.Zone.AffectedTiles);
            });
    }

    private static CombatAction CreateWorldWhispersSeekCheck(
        Creature owner,
        TaggedCalculatedNumberProducer dc)
    {
        return new CombatAction(
                owner,
                IllustrationName.Seek,
                "The World Whispers",
                [
                    ModData.ModTrait,
                    ModData.Traits.WorldRouser,
                    Trait.Archetype,
                    Trait.Primal,
                    Trait.Secret,
                    Trait.Basic,
                    Trait.IsNotHostile,
                    Trait.DoesNotBreakStealth,
                    Trait.AttackDoesNotTargetAC,
                    Trait.UsesPerception
                ],
                string.Empty,
                Target.Self())
            .WithActionCost(0)
            .WithActionId(ActionId.Seek)
            .WithActiveRollSpecification(new ActiveRollSpecification(
                TaggedChecks.Perception(),
                dc));
    }

    private static bool NeedsWorldWhispersDetectionStep(Creature observer, Creature target)
    {
        return target.DetectionStatus.IsUndetectedTo(observer)
            || target.DetectionStatus.HiddenTo.Contains(observer)
            || HasWorldWhispersConcealment(observer, target);
    }

    private static void ApplyWorldWhispersDetectionStep(Creature source, Creature target)
    {
        List<Creature> partyObservers = source.Battle.AllCreatures
            .Where(creature => creature.FriendOf(source))
            .ToList();

        if (target.DetectionStatus.IsUndetectedTo(source))
        {
            target.DetectionStatus.Undetected = false;
            foreach (Creature observer in partyObservers)
            {
                target.DetectionStatus.UndetectedTo.Remove(observer);
                target.DetectionStatus.HiddenTo.Add(observer);
            }

            target.DetectionStatus.RecalculateIsHiddenToAnEnemy();
            target.Overhead(
                "hidden",
                Color.Yellow,
                $"{target} is no longer undetected and is now hidden from the party.");
            return;
        }

        if (target.DetectionStatus.HiddenTo.Contains(source))
        {
            foreach (Creature observer in partyObservers)
            {
                target.DetectionStatus.HiddenTo.Remove(observer);
                observer.RemoveAllQEffects(qf => IsWorldWhispersConcealment(qf, target));
                observer.AddQEffect(new QEffect(
                    "The World Whispers",
                    $"You detect {target} as concealed.",
                    ExpirationCondition.Never,
                    target,
                    IllustrationName.None)
                {
                    Id = QEffectId.CannotSeeSourceExceptAsConcealed,
                    Key = WorldWhispersConcealmentKey
                });
            }

            target.DetectionStatus.RecalculateIsHiddenToAnEnemy();
            target.Overhead(
                "concealed",
                Color.Yellow,
                $"{target} is no longer hidden and is now concealed from the party.");
            return;
        }

        if (HasWorldWhispersConcealment(source, target))
        {
            foreach (Creature observer in partyObservers)
            {
                observer.RemoveAllQEffects(qf => IsWorldWhispersConcealment(qf, target));
            }

            target.Overhead(
                "observed",
                Color.Yellow,
                $"{target} is now normally detected by the party.");
        }
    }

    private static bool HasWorldWhispersConcealment(Creature observer, Creature target)
    {
        return observer.QEffects.Any(qf => IsWorldWhispersConcealment(qf, target));
    }

    private static bool IsWorldWhispersConcealment(QEffect qf, Creature target)
    {
        return qf.Id == QEffectId.CannotSeeSourceExceptAsConcealed
            && qf.Source == target
            && qf.Key == WorldWhispersConcealmentKey;
    }

    private static async Task SeekHiddenTileEffects(Creature seeker, IEnumerable<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            foreach (TileQEffect tileEffect in tile.TileQEffects.Where(effect => effect.SeekDC != 0).ToList())
            {
                Creature hiddenObject = Creature.CreateNoncombatCreature(
                    IllustrationName.DisarmTrap,
                    "Something Hidden",
                    [Trait.Pseudocreature, Trait.NeverSetsOccupant]);
                hiddenObject.Battle = tile.Battle;
                hiddenObject.TranslateTo(tile);

                CombatAction tileSeek = CreateWorldWhispersSeekCheck(
                    seeker,
                    Checks.FlatDC(tileEffect.SeekDC));
                CheckBreakdown breakdown = CombatActionExecution.BreakdownAttack(tileSeek, hiddenObject);
                CheckBreakdownResult result = new(breakdown);
                if (result.CheckResult >= CheckResult.Success)
                {
                    tile.TileOverhead(
                        result.CheckResult.ToString(),
                        Color.LightBlue,
                        $"{seeker} successfully Seeks with The World Whispers.",
                        "The World Whispers",
                        breakdown.DescribeWithFinalRollTotal(result));
                    if (tileEffect.WhenSeeked is not null)
                    {
                        await tileEffect.WhenSeeked();
                    }
                }
            }
        }
    }

    private static CombatAction CreateAllReturnsToSlumberAction(Creature owner)
    {
        return new CombatAction(
                owner,
                IllustrationName.TerrainTransposition,
                "All Returns to Slumber",
                [
                    ModData.ModTrait,
                    ModData.Traits.WorldRouser,
                    Trait.Archetype,
                    Trait.Mental,
                    Trait.Primal,
                    Trait.Sleep
                ],
                "Dismiss your waking world. Every creature in the area attempts a Will save against your world rouser DC. Animals, beasts, and plants worsen their result by one degree. On a failure, a creature can't use reactions for 1 round. On a critical failure, it is also slowed 1. Each creature then becomes temporarily immune for 1 hour.",
                Target.Self().WithAdditionalRestriction(self =>
                    self.QEffects.Any(qf =>
                        qf.Id == ModData.QEffectIds.WakingWorld
                        && qf.Tag is WakingWorldState)
                        ? null
                        : "Your waking world must be active."))
            .WithActionCost(1)
            .WithActionId(ModData.ActionIds.AllReturnsToSlumber)
            .WithSoundEffect(SfxName.ElementalBlastEarth)
            .WithEffectOnSelf(async (allReturnsToSlumber, self) =>
            {
                QEffect? wakingWorld = self.QEffects.FirstOrDefault(qf =>
                    qf.Id == ModData.QEffectIds.WakingWorld
                    && qf.Tag is WakingWorldState);
                if (wakingWorld?.Tag is not WakingWorldState state)
                {
                    return;
                }

                List<Creature> targets = state.Zone.CreaturesInZone
                    .Distinct()
                    .ToList();
                self.RemoveAllQEffects(qf => qf == wakingWorld);

                int dc = GetWorldRouserDC(self);
                foreach (Creature target in targets)
                {
                    if (HasAllReturnsToSlumberImmunity(target, self))
                    {
                        continue;
                    }

                    if (!target.IsImmuneTo(allReturnsToSlumber))
                    {
                        CheckResult result = await CommonSpellEffects.RollSavingThrowAsync(
                            target,
                            allReturnsToSlumber,
                            Defense.Will,
                            dc);
                        if (target.HasTrait(Trait.Animal)
                            || target.HasTrait(Trait.Beast)
                            || target.HasTrait(Trait.Plant))
                        {
                            result = Checks.WorsenByOneStep(result);
                            target.Overhead(
                                result.ToString(),
                                Color.Yellow,
                                $"{target}'s save is worsened by All Returns to Slumber.");
                        }

                        ApplyAllReturnsToSlumberResult(self, target, result);
                    }

                    target.AddQEffect(new QEffect(
                        "Temporarily Immune to All Returns to Slumber",
                        "You are immune to this world rouser's All Returns to Slumber for the rest of the encounter (approximating 1 hour).",
                        ExpirationCondition.Never,
                        self,
                        IllustrationName.TerrainTransposition)
                    {
                        Id = ModData.QEffectIds.AllReturnsToSlumberImmunity
                    });
                }
            });
    }

    private static bool HasAllReturnsToSlumberImmunity(Creature target, Creature source)
    {
        return target.QEffects.Any(qf =>
            qf.Id == ModData.QEffectIds.AllReturnsToSlumberImmunity
            && qf.Source == source);
    }

    private static void ApplyAllReturnsToSlumberResult(
        Creature source,
        Creature target,
        CheckResult result)
    {
        if (result is not (CheckResult.Failure or CheckResult.CriticalFailure))
        {
            return;
        }

        QEffect cannotReact = QEffect.CannotTakeReactions();
        cannotReact.Name = "All Returns to Slumber";
        cannotReact.Description = "You can't use reactions for 1 round.";
        cannotReact.ExpiresAt = ExpirationCondition.ExpiresAtStartOfSourcesTurn;
        cannotReact.Source = source;
        cannotReact.Illustration = IllustrationName.TerrainTransposition;
        target.AddQEffect(cannotReact);

        if (result == CheckResult.CriticalFailure)
        {
            QEffect slowed = QEffect.Slowed(1);
            slowed.Source = source;
            slowed.ExpiresAt = ExpirationCondition.ExpiresAtStartOfSourcesTurn;
            target.AddQEffect(slowed);
        }
    }

    private static Possibility CreateShelteringHandPossibility(Creature owner)
    {
        List<Possibility> possibilities =
        [
            new ActionPossibility(CreateShelteringHandAction(owner, 1, CoverKind.Lesser)),
            new ActionPossibility(CreateShelteringHandAction(owner, 2, CoverKind.Standard))
        ];

        return new SubmenuPossibility(IllustrationName.TerrainTransposition, "Sheltering Hand")
        {
            Subsections =
            {
                new PossibilitySection("Sheltering Hand")
                {
                    Possibilities = possibilities
                }
            }
        };
    }

    private static CombatAction CreateShelteringHandAction(
        Creature owner,
        int actionCost,
        CoverKind grantedCover)
    {
        string coverName = grantedCover == CoverKind.Lesser ? "Lesser Cover" : "Standard Cover";
        return new CombatAction(
                owner,
                IllustrationName.TerrainTransposition,
                $"Sheltering Hand — {coverName}",
                [
                    ModData.ModTrait,
                    ModData.Traits.WorldRouser,
                    Trait.Archetype,
                    Trait.Primal
                ],
                $"You and your allies currently in your waking world gain {coverName.ToLowerInvariant()} from enemies you are aware of until the start of your next turn.",
                Target.Self().WithAdditionalRestriction(self =>
                    self.QEffects.Any(qf =>
                        qf.Id == ModData.QEffectIds.WakingWorld
                        && qf.Tag is WakingWorldState)
                        ? null
                        : "Your waking world must be active."))
            .WithActionCost(actionCost)
            .WithSoundEffect(SfxName.RaiseShield)
            .WithEffectOnSelf(self =>
            {
                QEffect? wakingWorld = self.QEffects.FirstOrDefault(qf =>
                    qf.Id == ModData.QEffectIds.WakingWorld
                    && qf.Tag is WakingWorldState);
                if (wakingWorld?.Tag is not WakingWorldState state)
                {
                    return;
                }

                foreach (Creature ally in state.Zone.CreaturesInZone.Where(creature => creature.FriendOf(self)))
                {
                    ApplyShelteringHandCover(self, ally, grantedCover);
                }
            });
    }

    private static void ApplyShelteringHandCover(
        Creature source,
        Creature recipient,
        CoverKind grantedCover)
    {
        QEffect? existing = recipient.QEffects.FirstOrDefault(qf =>
            qf.Id == ModData.QEffectIds.ShelteringHand
            && qf.Source == source);
        if (existing?.Tag is CoverKind existingCover
            && CoverAtLeastAsStrong(existingCover, grantedCover))
        {
            return;
        }

        recipient.RemoveAllQEffects(qf =>
            qf.Id == ModData.QEffectIds.ShelteringHand
            && qf.Source == source);
        recipient.AddQEffect(new QEffect(
            "Sheltering Hand",
            $"You have {grantedCover.ToString().ToLowerInvariant()} cover from enemies the world rouser is aware of.",
            ExpirationCondition.ExpiresAtStartOfSourcesTurn,
            source,
            IllustrationName.TerrainTransposition)
        {
            Id = ModData.QEffectIds.ShelteringHand,
            CountsAsABuff = true,
            Tag = grantedCover,
            IncreaseCover = (qfThis, incomingAction, currentCover) =>
            {
                Creature attacker = incomingAction.Owner;
                if (!attacker.EnemyOf(source)
                    || attacker.DetectionStatus.IsUndetectedTo(source))
                {
                    return currentCover;
                }

                return CoverAtLeastAsStrong(currentCover, grantedCover)
                    ? currentCover
                    : grantedCover;
            }
        });
    }

    private static bool CoverAtLeastAsStrong(CoverKind current, CoverKind candidate)
    {
        return current switch
        {
            CoverKind.Blocked or CoverKind.Greater => true,
            CoverKind.Standard => candidate is CoverKind.Standard or CoverKind.Lesser or CoverKind.None,
            CoverKind.Lesser => candidate is CoverKind.Lesser or CoverKind.None,
            _ => candidate == CoverKind.None
        };
    }

    private static CombatAction CreateDustCloudAction(Creature owner)
    {
        CombatAction action = new CombatAction(
                owner,
                IllustrationName.DriftingPollen,
                "Dust Cloud",
                [
                    ModData.ModTrait,
                    ModData.Traits.WorldRouser,
                    Trait.Archetype,
                    Trait.Primal
                ],
                "Until the start of your next turn, creatures in your waking world are concealed, and creatures outside it are concealed to creatures within it. A creature that ends its turn in the area attempts a Fortitude save against your world rouser DC. On a failure, it is dazzled until the end of its next turn; on a critical failure, it is blinded instead.",
                Target.Self().WithAdditionalRestriction(self =>
                    self.QEffects.Any(qf =>
                        qf.Id == ModData.QEffectIds.WakingWorld
                        && qf.Tag is WakingWorldState)
                        ? null
                        : "Your waking world must be active."))
            .WithActionCost(1)
            .WithActionId(ModData.ActionIds.DustCloud)
            .WithSoundEffect(SfxName.ElementalBlastEarth);

        action.WithEffectOnSelf((dustCloudAction, self) =>
        {
            QEffect? wakingWorld = self.QEffects.FirstOrDefault(qf =>
                qf.Id == ModData.QEffectIds.WakingWorld
                && qf.Tag is WakingWorldState);
            if (wakingWorld?.Tag is not WakingWorldState state)
            {
                return Task.CompletedTask;
            }

            self.RemoveAllQEffects(qf => qf.Id == ModData.QEffectIds.DustCloud);
            self.AddQEffect(new QEffect(
                "Dust Cloud",
                "Dust and pollen obscure sight throughout your waking world until the start of your next turn.",
                ExpirationCondition.ExpiresAtStartOfYourTurn,
                self,
                IllustrationName.DriftingPollen)
            {
                Id = ModData.QEffectIds.DustCloud,
                CountsAsABuff = true,
                Tag = wakingWorld,
                StateCheck = qfThis =>
                {
                    if (!qfThis.Owner.QEffects.Contains(wakingWorld))
                    {
                        qfThis.ExpiresAt = ExpirationCondition.Immediately;
                        return;
                    }

                    ApplyDustCloudConcealment(qfThis, state);
                }
            });

            state.Zone.AfterCreatureEndsItsTurnHere = async creature =>
            {
                if (!self.QEffects.Any(qf =>
                        qf.Id == ModData.QEffectIds.DustCloud
                        && ReferenceEquals(qf.Tag, wakingWorld))
                    || !self.QEffects.Contains(wakingWorld)
                    || creature.IsImmuneTo(dustCloudAction))
                {
                    return;
                }

                CheckResult result = await CommonSpellEffects.RollSavingThrowAsync(
                    creature,
                    dustCloudAction,
                    Defense.Fortitude,
                    GetWorldRouserDC(self));
                if (result is CheckResult.Failure or CheckResult.CriticalFailure)
                {
                    ApplyDustCloudCondition(
                        self,
                        creature,
                        result == CheckResult.CriticalFailure);
                }
            };

            return Task.CompletedTask;
        });

        return action;
    }

    private static void ApplyDustCloudConcealment(QEffect dustCloud, WakingWorldState state)
    {
        HashSet<Creature> creaturesInside = state.Zone.CreaturesInZone.ToHashSet();
        foreach (Creature creatureInside in creaturesInside)
        {
            creatureInside.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
            {
                Id = QEffectId.ConcealedByZoneCloud,
                Source = dustCloud.Owner,
                ThisCreatureCannotBeMoreVisibleThan = DetectionStrength.Concealed
            });

            foreach (Creature creatureOutside in dustCloud.Owner.Battle.AllCreatures.Where(
                         creature => !creaturesInside.Contains(creature)))
            {
                creatureInside.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                {
                    Id = QEffectId.CannotSeeSourceExceptAsConcealed,
                    Source = creatureOutside
                });
            }
        }
    }

    private static void ApplyDustCloudCondition(
        Creature source,
        Creature target,
        bool blinded)
    {
        target.RemoveAllQEffects(qf =>
            qf.Id == ModData.QEffectIds.DustCloudCondition
            && qf.Source == source);

        target.AddQEffect(new QEffect(
            blinded ? "Blinded by Dust Cloud" : "Dazzled by Dust Cloud",
            blinded
                ? "You are blinded until the end of your next turn."
                : "You are dazzled until the end of your next turn.",
            ExpirationCondition.Never,
            source,
            IllustrationName.DriftingPollen)
        {
            Id = ModData.QEffectIds.DustCloudCondition,
            Tag = false,
            StartOfYourEveryTurn = (qfThis, _) =>
            {
                qfThis.Tag = true;
                return Task.CompletedTask;
            },
            EndOfYourTurnBeneficialEffect = (qfThis, _) =>
            {
                if (qfThis.Tag is true)
                {
                    qfThis.ExpiresAt = ExpirationCondition.Immediately;
                }

                return Task.CompletedTask;
            },
            StateCheck = qfThis =>
                qfThis.Owner.AddQEffect(blinded ? QEffect.Blinded() : QEffect.Dazzled())
        });
    }

    private static CombatAction CreateWakeAndTrembleAction(Creature owner)
    {
        return new CombatAction(
                owner,
                IllustrationName.TerrainTransposition,
                "Wake and Tremble",
                [
                    ModData.ModTrait,
                    ModData.Traits.WorldRouser,
                    Trait.Archetype,
                    Trait.Primal
                ],
                "Trigger You Rouse the World. All other creatures in the area attempt a Reflex save against your world rouser DC. On a failure, a creature falls prone. On a critical failure, it also becomes sickened 1.",
                Target.Self())
            .WithActionCost(0)
            .WithActionId(ModData.ActionIds.WakeAndTremble)
            .WithSoundEffect(SfxName.ElementalBlastEarth);
    }

    private static async Task TryUseWakeAndTremble(
        Creature source,
        ChosenTargets chosenTargets)
    {
        if (!source.HasFeat(ModData.FeatNames.WakeAndTremble))
        {
            return;
        }

        List<Creature> targets = chosenTargets.AllCreaturesInArea
            .Where(creature => creature != source)
            .Distinct()
            .ToList();
        if (targets.Count == 0
            || !await source.Battle.AskForConfirmation(
                source,
                IllustrationName.TerrainTransposition,
                "Use Wake and Tremble? All other creatures in the area, including allies, must attempt a Reflex save.",
                "Wake and Tremble",
                "Do not use"))
        {
            return;
        }

        CombatAction wakeAndTremble = CreateWakeAndTrembleAction(source);
        int dc = GetWorldRouserDC(source);
        foreach (Creature target in targets)
        {
            CheckResult result = await CommonSpellEffects.RollSavingThrowAsync(
                target,
                wakeAndTremble,
                Defense.Reflex,
                dc);
            if (result is CheckResult.Failure or CheckResult.CriticalFailure)
            {
                await target.FallProne();
            }

            if (result == CheckResult.CriticalFailure)
            {
                target.AddQEffect(QEffect.Sickened(1, dc).WithSourceAction(wakeAndTremble));
            }
        }
    }

    #endregion

    #region Targeting and Area Helpers

    private static TileQEffect CreateWakingWorldTileEffect(
        Tile tile,
        Creature owner,
        QEffect wakingWorld)
    {
        TileQEffect tileEffect = new TileQEffect(tile)
        {
            Illustration = (tile.X + tile.Y) % 2 == 0
                ? IllustrationName.NewGrass
                : IllustrationName.NewGrass2,
            Name = "Waking World",
            VisibleDescription = "{b}Waking World.{/b} The flora, fauna, and elements here have been roused from their slumber."
        };

        tileEffect.StateCheck = _ =>
        {
            bool dustCloudIsActive = owner.QEffects.Any(qf =>
                qf.Id == ModData.QEffectIds.DustCloud
                && ReferenceEquals(qf.Tag, wakingWorld));
            tileEffect.Illustration = dustCloudIsActive
                ? IllustrationName.PetalMistFog
                : (tile.X + tile.Y) % 2 == 0
                    ? IllustrationName.NewGrass
                    : IllustrationName.NewGrass2;
            tileEffect.VisibleDescription = dustCloudIsActive
                ? "{b}Waking World — Dust Cloud.{/b} Dust and pollen obscure creatures within this area."
                : "{b}Waking World.{/b} The flora, fauna, and elements here have been roused from their slumber.";

            if (owner.QEffects.Any(qf =>
                qf.Id == ModData.QEffectIds.NaturesEmbrace
                && ReferenceEquals(qf.Tag, wakingWorld)))
            {
                tile.DifficultTerrainToComputerControlledCreatures = true;
            }
        };

        return tileEffect;
    }

    private static bool RecallKnowledgeSubjectIsInZone(
        CombatAction action,
        Creature? target,
        Zone zone)
    {
        Creature? creatureSubject = target ?? action.ChosenTargets.ChosenCreature;
        if (creatureSubject is not null)
        {
            return zone.CreaturesInZone.Contains(creatureSubject);
        }

        if (action.ChosenTargets.ChosenTile is not null)
        {
            return zone.AffectedTiles.Contains(action.ChosenTargets.ChosenTile);
        }

        return false;
    }

    private static bool IsCreatureOrHazardRecallAction(CombatAction action)
    {
        return action.Name.Contains("Recall Knowledge", StringComparison.OrdinalIgnoreCase)
            || action.Name.Equals("Recall Weakness", StringComparison.OrdinalIgnoreCase)
            || action.ActionId.ToStringOrTechnical().Equals(
                RecallWeaknessActionId,
                StringComparison.Ordinal);
    }

    #endregion

    #region Temporary Immunities

    // Shared temporary-immunity helpers belong here.

    #endregion
}
