# Asset Scope

## Canonical Production Scope

- Primary gameplay scene: `Assets/Game/Scenes/SampleScene.unity`
- Production gameplay code: `Assets/Game/Scripts/**`
- Production gameplay data: `Assets/Game/ScriptableObjects/**`
- Production prefabs: `Assets/Game/Prefabs/**`

## Sample and Third-Party Scope

These directories are retained in the repository but treated as non-production reference/sample content:

- `Assets/TextMesh Pro/Examples & Extras/**`
- `Assets/Game/Models/Tiny Swords/Scenes/**`

## Working Convention

- New feature work should target `Assets/Game/**`.
- Build scene list should stay limited to canonical gameplay scenes unless intentionally expanded.
- Sample scenes and package demo scripts should not be modified during normal gameplay development.
