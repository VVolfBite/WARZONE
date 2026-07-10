# Content Authoring Rules

## Scope
Content owns static definitions only:
- weapons
- enemies
- missions
- sites
- items
- resource packages
- outposts
- loot profiles
- environmental zones
- vision equipment
- base modules

## Rules
- Content definitions do not depend on `UnityEngine`.
- Content definitions do not encode campaign, combat, or application rules.
- Content data is referenced by id from runtime state and save data.
- Save files store ids and runtime state, not definition objects.
- `ContentCatalog` is the single lookup surface for content definitions.

## Id conventions
- Use stable lowercase ids with dots or underscores.
- Do not reuse ids across different definition types.
- Duplicate ids must be rejected or recorded by validation.

## Reference rules
- Missions may reference objectives, reward profiles, and required site types.
- Sites may reference default outposts.
- Loot profiles may reference resources, items, and weapons.
- Outposts and base modules may reference resource package ids for costs.

## Validation
- Missing referenced ids are validation issues.
- Duplicate ids are validation issues.
- Critical demo catalog validation should pass with no critical issues.

## Save compatibility
- Persistent data stores ids only.
- Loading resolves ids through the active catalog.
- Missing content ids should fail cleanly or fall back explicitly.

## Runtime mapping boundary
- Static content enums stay in `Content`.
- Runtime enums stay in `Combat`.
- `Application` is the only layer that maps between the two.
- `Content` must not reference `Combat` runtime types.
