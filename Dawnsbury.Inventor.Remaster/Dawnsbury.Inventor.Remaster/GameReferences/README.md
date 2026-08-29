# Dawnsbury Days assembly references

This folder contains the game assemblies needed to compile the mod on a computer where Dawnsbury Days is not installed.

Required files:

- `Dawnsbury Days.dll`
- `Common.dll`
- `MonoGame.Framework.dll`

Keep these files synchronized with the version of Dawnsbury Days against which the mod will be tested. Do not include them when distributing the mod; distribute only the mod DLL and its own supporting files.

To reference a different game installation without changing the project file, build with:

```text
dotnet build -p:GameDataDirectory="/path/to/Dawnsbury Days/Data"
```

On Windows, `GameInstallDirectory` can also be overridden to control both the reference location and the post-build `CustomMods` destination.
