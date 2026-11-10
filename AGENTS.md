# Realm Custom Map Agents Guide

Realm is an RTS Game Engine with a custom map editor for UGC arcade maps.

## This map composes with the Melee Ruleset

`references.json` names the Melee Ruleset (`../MeleeRuleset`), and every save re-imports that map into this folder, so the map that is tested, exported, published and played is this folder.

- **Rules** live in `MeleeRuleset/MapScript.cs`. The `MapScript.cs` here is a read-only copy the save writes, so a rule change belongs in that repository. Its `MapScript.csproj` is likewise the referenced one.
- **Metadata** is merged: this map's `metadata.json` merges last over the rules map's. Objects merge key by key, arrays combine with each entry kept once by its `TemplateID`, and a property this map leaves `null` lets the rules map's value through. Edit this map's own templates, terrain and identity here; edit the melee templates, relics and factions in the rules map.
- **Board content** is authored here only: `terrain.json`, its height, splat and water fields, `terrain_pathing.png`, the props placed on it, the artwork under `Assets/`, and the `Models` and environment settings in `metadata.json`. That includes the placement the rules read from the board: the four `SpawnAnchors` and the twelve props using the rules map's `resource/melee_gold_mine` and `resource/melee_woodland` templates. Moving a seat or a node is an edit here, not in the rules map.
- **Generated code** — `MapTemplates.g.cs` and `WasmEntryPoint.g.cs` — is written from this map's merged metadata and from the engine's template on every workspace setup, and is never taken from the rules map.
- **`lib/`** must carry the current `Realm.MapAPI`, `Realm.Ecs` and `Realm.EditorAPI` assemblies, because the referenced ruleset is compiled in this folder.

## Map Scripting (MapScript.cs)

The script this map runs comes from the rules map. The API is the same one every map uses: `Initialize(IGameAPI api)` when the match starts and `Update(IGameAPI api, float delta)` every simulation tick (30Hz), with all map operations proxied through `IGameAPI`.

## Debugging

- Attach the `Attach to Realm Authority (Map Script WASM)` launch configuration to the authority, which is the only process that executes a map script.
- `realm_lldb.py` is refreshed from `MapTemplate` on every workspace setup.
