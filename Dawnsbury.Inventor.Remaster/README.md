# Dawnsbury.Inventor.Remaster

A Dawnsbury Days implementation of the Pathfinder 2e Remastered Inventor class. This project began as a remaster-focused fork of Brick264's [Inventor Class mod](https://github.com/DINGLEBOB/DawnsburyMods/tree/main/Inventor).

## Optional SpiritDamage compatibility

[SpiritDamage](https://github.com/SilchasRuin/SpiritDamage/tree/master/SpiritDamage) is optional and is not a compile-time dependency.

When SpiritDamage is installed, Otherworldly Protection detects the custom `Spirit` damage kind at runtime and gains spirit resistance equal to 3 + half the Inventor's level. Without SpiritDamage, this mod continues to load normally and the additional resistance is simply not added.

## Optional homebrew settings

### Unstable Contingencies

`Settings → Mod Settings → Inventor Remaster: Enable Unstable Contingencies`

This option is disabled by default. When enabled, full Inventors gain the homebrew-tagged **Unstable Contingencies** class feature at level 3:

- From level 3 through level 6, unstable actions are locked for the remainder of the battle after the second failed unstable check instead of the first.
- At level 7 and above, unstable actions are locked after the third failed unstable check.
- Critical failures count toward the limit and still deal their normal damage.
- Unstable actions used by a construct companion count against the Inventor's same shared failure limit.
- The current number of failed checks is tracked on the Inventor during battle.

When this option is disabled, unstable actions use the normal rule and are locked after the first failed check.

## Implemented innovations

### Initial armor modifications

1. Harmonic Oscillator
2. Metallic Reactance
3. Muscular Exoskeleton
4. Otherworldly Protection
5. Phlogistonic Regulator
6. Speed Boosters
7. Subtle Dampeners

### Initial construct modifications

1. Flight Chassis
2. Accelerated Mobility
3. Projectile Launcher
4. Wonder Gears

### Initial weapon modifications

1. Entangling Form
2. Hampering Spikes
3. Hefty Composition
4. Razor Prongs

### Breakthrough armor modifications — level 7

1. Antimagic Plating
2. Dense Plating
3. Hyper Boosters
4. Layered Mesh
5. Tensile Absorption

### Breakthrough construct modifications — level 7

1. Antimagic Construction
2. Durable Construction
3. Marvelous Gears
4. Turret Configuration

### Breakthrough weapon modifications — level 7

1. Advanced Rangefinder
2. Aerodynamic Construction
3. Inconspicuous Appearance

## Implemented class feats

### Level 1

- Construct Companion
- Explosive Leap
- Tamper
- Variable Core

### Level 2

- Oil Fire
- Searing Restoration

### Level 4

- Advanced Construct Companion
- Megaton Strike
- Soaring Armor

### Level 6

- Megavolt
- Visual Fidelity

### Level 8

- Gigaton Strike
- Incredible Construct Companion
- Manifold Modifications
- Overdrive Ally
