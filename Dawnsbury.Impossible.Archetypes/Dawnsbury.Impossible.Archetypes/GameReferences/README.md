# Dawnsbury Days assembly references

This folder can contain the game assemblies needed to compile the mod on a computer where Dawnsbury Days is not installed at the project's default location.

Required files:

- `Dawnsbury Days.dll`
- `Common.dll`
- `MonoGame.Framework.dll`

The repository ignores DLLs in this directory. Do not redistribute the game assemblies with the mod.

To reference another installation without changing the project file, build with:

```text
dotnet build -p:GameDataDirectory="/path/to/Dawnsbury Days/Data"
```
