# Secluded Crest

The board: `terrain.json`, its height and splat maps, the water field, the props it places, the artwork under `Assets/`, and the `Models` and environment settings its `metadata.json` declares.

It also states the game's placement: the four `SpawnAnchors` a seated player starts at, the twelve melee resource nodes, placed as props whose templates the Melee Ruleset declares, and the neutral camps - four Crest Boars and one Ridge Giant - placed as units nobody owns. The rules read all of it from here rather than carrying coordinates of their own, so this board can change where a seat begins, where a mine stands or where a camp waits without the ruleset changing at all.

## Composed with the Melee Ruleset

This folder is also a composing map. `references.json` names the Melee Ruleset, and every save takes that map's files into this folder, so the result is one ordinary map: it can be edited, tested, exported, published and played like any other, and nothing downstream has to resolve anything.

| Referenced map | What it contributes |
| :--- | :--- |
| Melee Ruleset v1.0.0 (pinned in `references.json`) | The rules engine of the map: `MapScript.cs` (the script itself), the melee templates it registers in `metadata.json` (including the two resource node types this board places), and its relics, factions, deck and economy. |

Who keeps a file:

* `metadata.json` is always merged. Objects merge key by key, arrays are combined with each entry kept once by its `TemplateID`, and this map merges last, so everything it states stands over the rules map's. A property this map leaves `null` states nothing and lets the rules map's value through, which is how the board keeps its own name, build number, shroud and camera while the game rules, units, abilities and relics arrive from the rules map.
* Every other file comes whole from exactly one map. This map keeps its own copy of anything both carry, unless `references.json` states a priority; `MapScript.cs` is the one file stated to come from the rules map, because the ruleset is defined there.
* `MapTemplates.g.cs` and `WasmEntryPoint.g.cs` are generated for this workspace from this map's merged metadata and from the engine's current template, so they are never taken from a referenced map.
* Files taken from the rules map are read-only here. Editing one is reverted by the next save, which reports every file it replaced.

A save reads the rules map from the helper folder `references.json` names while that folder exists, and otherwise from the map store by name, version and pinned manifest hash. `metadata.partial.json` holds the keys this map overrides in the merge, and is regenerated on every save by diffing this map's `metadata.json` against the merge of the referenced map, so upgrading the rules map to a new version and re-saving drops the old version's data instead of carrying it forward.

### Prerequisites

* `references.json`'s `FolderPath` points at the sibling `MeleeRuleset` checkout.
* The script compiles here, in this folder, from the referenced `MapScript.cs` and `MapScript.csproj`. The rules map's own `bin/` is build output and is never copied.
* `lib/` holds the current `Realm.MapAPI`, `Realm.Ecs` and `Realm.EditorAPI` assemblies. A map that compiles the ruleset needs all three, or the referenced script will not build.

## What changed when this map became the composing map

The board's `GameBuildNumber` is `v0.0.6`, the build its content is played on, and it stands over the rules map's for the same reason.

## The board has no rules of its own

Its `MapScript.cs` is the rules map's script once a save has run. Until then the file here is the empty template every new map starts from, and a save replaces it.
