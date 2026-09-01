# Dawnsbury.Impossible.Archetypes

A Dawnsbury Days implementation of archetypes from *Pathfinder Impossible Magic*.

## Planned archetypes

| Archetype | Status |
| --- | --- |
| World Rouser | Levels 2–8 implemented and release-ready; level 10+ feats deferred until Dawnsbury Days supports that tier |
| Hedge Mage | Planned after World Rouser |

Each archetype is implemented in a single source file under `Archetypes/`, following the organization used by the More Archetypes mod.

## Current release scope

This release contains the complete level 2–8 World Rouser progression currently relevant to Dawnsbury Days:

- World Rouser Dedication and Rouse the World
- Nature's Embrace
- The World Whispers
- All Returns to Slumber
- Sheltering Hand
- Dust Cloud
- Wake and Tremble

The level 10–14 World Rouser feats are intentionally deferred rather than exposed as incomplete options.

## Rules source

- [World Rouser](https://2e.aonprd.com/Archetypes.aspx?ID=396)
- [Hedge Mage](https://2e.aonprd.com/Archetypes.aspx?ID=391)

## World Rouser implementation notes

- The waking world is a persistent, visibly marked burst centered on the chosen point rather than on the world rouser.
- It remains in place across turns and grants its skill bonuses only to allies currently occupying the area.
- Its Nature bonus recognizes both standard Recall Knowledge checks and the Recall Weakness action from Lores and Weaknesses when the subject is in the area.
- Starting on the following turn, Rouse the World can be Sustained once per round to increase the same burst by 10 feet.
- Nature's Embrace makes the current waking world difficult terrain for enemies, but not allies, until the start of the rouser's next turn; tiles added by Sustain inherit the effect.
- The World Whispers Seeks every square in the waking world and improves successfully detected creatures one visibility step for the entire party: undetected to hidden, hidden to concealed, and concealed to normally detected. It also checks hidden tile objects such as traps.
- All Returns to Slumber dismisses the waking world, resolves Will saves for its occupants, worsens saves for animals, beasts, and plants, and applies encounter-long immunity as an approximation of its one-hour immunity.
- Sheltering Hand offers separate one-action lesser-cover and two-action standard-cover options, affecting friendly creatures in the waking world at activation and filtering out enemies undetected by the rouser.
- Dust Cloud dynamically conceals creatures across the waking-world boundary, visually marks the cloud, and calls for a Fortitude save when any creature ends its turn within the area; its dazzled and blinded effects last through that creature's next turn.
- Wake and Tremble offers an optional free-action trigger whenever the character Rouses the World, resolving Reflex saves for every other creature in the initial burst before applying prone and sickened outcomes.
- Dawnsbury Days encounters do not track ten-minute exploration durations, so the waking world lasts for the rest of the encounter unless dismissed or replaced.
