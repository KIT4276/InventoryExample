# AGENTS.md

## Project Summary

- Unity project for an inventory prototype with wallet, slot unlocking, save/load, and a small UI.
- Unity version: `2022.3.62f2`.
- Main scene: `Assets/Scenes/GameScene.unity`.
- Dependency injection is built with Zenject via scene/project contexts and `MonoInstaller` classes.

## Important Folders

- `Assets/Scripts/Configs`: `ScriptableObject` configs for initial inventory, wallet balance, and item database.
- `Assets/Scripts/Data`: runtime state containers and serializable save DTOs.
- `Assets/Scripts/Definitions`: item definitions (`ItemDefinition`, `WeaponDefinition`, `AmmoDefinition`, `ArmorDefinition`).
- `Assets/Scripts/Service`: business logic for inventory, wallet, gameplay rewards, initialization, and save/load.
- `Assets/Scripts/UI`: MonoBehaviours for buttons and inventory/wallet views.
- `Assets/Scripts/Installers`: Zenject bindings for configs, services, data, and initialization.
- `Assets/Configs`: authored assets used by the installers.
- `Assets/Prefabs`: scene/UI prefabs such as inventory canvas and slot prefab.

## Architecture Notes

- `InventoryData` owns the runtime slot list and low-level slot operations.
- `InventoryService` is the main facade for UI interactions:
  - add random items
  - stack slots
  - consume ammo
  - unlock slots with wallet spending
  - save inventory changes
- `WalletService` owns coin balance, random coin rewards, persistence, and `BalanceChanged`.
- `SaveLoadService` reads/writes one JSON file at `Application.persistentDataPath/save.json`.
- `InventoryInitializer` creates the slot layout from config and then applies saved slot state.
- `ItemDatabase` is built from `ItemDatabaseConfig` and resolves item definitions by ID at runtime.

## Current Gameplay Rules

- Total slot count and initially unlocked slot count come from `InitialInventoryConfig`.
- Slot unlock cost is index-based and also comes from `InitialInventoryConfig`.
- A slot can be unlocked only if:
  - index is valid
  - the slot is currently locked
  - the previous slot is already unlocked
  - a configured unlock cost exists
  - the wallet can spend that amount
- Random item add currently picks only `WeaponDefinition` and `AmmoDefinition`.
- Stacking works only for same item ID, unlocked slots, and respects `ItemDefinition.MaxStack`.

## Data And Persistence Constraints

- `GameSaveData` currently stores:
  - `saveVersion`
  - `coins`
  - `slots`
- Inventory save/load assumes `InventoryData.Initialize(...)` has already created the slot list before `ApplySaveData(...)`.
- `InventorySlotData.SetItem(count <= 0)` clears the slot.
- Item IDs are generated in `ItemDefinition.OnValidate()` from the asset GUID. Do not hand-edit IDs unless there is a migration plan.
- If item assets are recreated, IDs may change and old saves may stop resolving items correctly.

## Dependency Injection Notes

- `GameServicesInstaller` binds wallet-related services and save/load as singletons.
- `InventoryInstaller` binds:
  - inventory configs
  - the item database config and item array
  - each `ItemDefinition` instance keyed by item ID
  - `ItemDatabase`, `InventoryData`, `InventoryService`
  - `InventoryInitializer` as `NonLazy`
- When adding a new gameplay service, prefer binding it in the installer layer rather than using `FindObjectOfType`.

## UI Notes

- `InventoryGridView` builds slot views dynamically from `InitialInventoryConfig.TotalSlots`.
- `InventorySlotView` is a passive renderer with a click callback.
- `WalletView` listens to `WalletService.BalanceChanged`.
- `AddItemButton` and `AddCoinsButton` are thin UI adapters and should stay thin.
- When changing UI behavior, keep service logic in services/data rather than moving business rules into MonoBehaviours.

## Existing Risks To Notice

- The git worktree is currently dirty. Do not overwrite unrelated user changes.
- Some UI/log strings show mojibake/encoding issues. Be careful when editing localized text and keep file encoding consistent.
- `InventoryGridView` tracks `_selectedSlotView`, but `InventorySlotView.Render(...)` currently has no selected-state rendering.
- `InventoryService.TryAddRandomItem()` manually scans slots before calling `InventoryData.TryAddOneItemToFreeSlot(...)`; keep both behaviors aligned if add logic changes.
- `InventoryData.ApplySaveData(...)` only applies up to the current slot count and ignores overflow. Keep this in mind if slot counts change between versions.

## Working Guidelines For Future Agents

- Read the installer and config path before changing runtime behavior. Many issues in this repo are wiring issues, not algorithm issues.
- Treat `ScriptableObject` assets in `Assets/Configs` as gameplay data, not code defaults.
- Prefer extending `InventoryService`/`WalletService` for new use cases instead of letting UI scripts mutate data directly.
- Preserve event notifications:
  - inventory changes should keep `InventoryService.Changed` accurate
  - wallet changes should keep `WalletService.BalanceChanged` accurate
- If a change affects persistence, update both save and load paths in the same task.
- If a change affects item identity or config assets, call out migration risk explicitly.
- Avoid touching `Library`, `Temp`, or generated project files unless the task truly requires it.

## Safe Validation Checklist

- Confirm Zenject bindings still provide every referenced config/service.
- Confirm inventory can still initialize from config with and without an existing save.
- Confirm adding coins updates `WalletView`.
- Confirm unlocking a slot spends coins and persists after restart.
- Confirm adding/staking items updates the grid and persists after restart.
- Confirm button listeners are still attached and detached safely.

## Useful Entry Points

- `Assets/Scripts/Service/InventoryService.cs`
- `Assets/Scripts/Data/InventoryData.cs`
- `Assets/Scripts/Service/WalletService.cs`
- `Assets/Scripts/Service/SaveLoadService.cs`
- `Assets/Scripts/Installers/InventoryInstaller.cs`
- `Assets/Scripts/Installers/GameServicesInstaller.cs`
- `Assets/Scripts/UI/InventoryGridView.cs`
- `Assets/Scripts/UI/InventorySlotView.cs`
