using System.Collections.Generic;
using UnityEngine;

public class InventoryActionService
{
    private readonly InventoryQueryService _inventoryQueryService;
    private readonly InventoryMutationService _inventoryMutationService;
    private readonly AmmoRewardConfig _ammoRewardConfig;
    private readonly ItemDatabase _itemDatabase;

    public InventoryActionService(
        InventoryQueryService inventoryQueryService,
        InventoryMutationService inventoryMutationService,
        AmmoRewardConfig ammoRewardConfig,
        ItemDatabase itemDatabase)
    {
        _inventoryQueryService = inventoryQueryService;
        _inventoryMutationService = inventoryMutationService;
        _ammoRewardConfig = ammoRewardConfig;
        _itemDatabase = itemDatabase;
    }

    public void TryAddRandomItem()
    {
        IReadOnlyList<ItemDefinition> possibleItems = GetRandomLootItems();
        if (possibleItems.Count == 0)
            return;

        if (!_inventoryQueryService.TryGetFirstFreeUnlockedSlot(out InventorySlotEntry freeSlot))
        {
            LogInventoryFull();
            return;
        }

        ItemDefinition randomItem = PickRandom(possibleItems);
        if (!_inventoryMutationService.TryAddOneItemToFreeSlot(randomItem))
            return;

        LogItemAdded(randomItem, freeSlot.SlotIndex);
    }

    public void TryAddRandomAmmo()
    {
        IReadOnlyList<AmmoDefinition> possibleAmmo = GetAvailableAmmoTypes();
        if (possibleAmmo.Count == 0)
            return;

        AmmoDefinition randomAmmo = PickRandom(possibleAmmo);
        int addAmount = GetRandomAmmoAmount();

        List<InventorySlotAddResult> addResults = new();
        bool added = _inventoryMutationService.TryAddItems(randomAmmo, addAmount, addResults, out int remainder);
        if (!added)
        {
            LogInventoryFull();
            return;
        }

        LogAmmoAdded(randomAmmo, addResults);

        if (remainder > 0)
            LogInventoryFull();
    }

    public void TryShootRandomWeapon()
    {
        IReadOnlyList<WeaponDefinition> weapons = _inventoryQueryService.GetWeapons();
        if (weapons.Count == 0)
        {
            LogNoWeapons();
            return;
        }

        WeaponDefinition randomWeapon = PickRandom(weapons);
        if (_inventoryQueryService.GetAmmoStacksFor(randomWeapon.AmmoDefinition).Count == 0)
        {
            LogNoAmmo(randomWeapon);
            return;
        }

        if (!_inventoryMutationService.TryConsumeAmmoForWeapon(randomWeapon))
        {
            LogNoAmmo(randomWeapon);
            return;
        }

        LogShot(randomWeapon);
    }

    private IReadOnlyList<ItemDefinition> GetRandomLootItems()
    {
        List<ItemDefinition> possibleItems = new();

        if (_itemDatabase == null || _itemDatabase.ItemDefinitions == null || _itemDatabase.ItemDefinitions.Count == 0)
            return possibleItems;

        foreach (ItemDefinition item in _itemDatabase.ItemDefinitions.Values)
        {
            if (item is AmmoDefinition)
                continue;

            possibleItems.Add(item);
        }

        return possibleItems;
    }

    private IReadOnlyList<AmmoDefinition> GetAvailableAmmoTypes()
    {
        List<AmmoDefinition> possibleAmmo = new();

        if (_ammoRewardConfig == null)
            return possibleAmmo;

        foreach (AmmoDefinition ammoDefinition in _ammoRewardConfig.AvailableAmmoDefinitions)
        {
            if (ammoDefinition != null)
                possibleAmmo.Add(ammoDefinition);
        }

        return possibleAmmo;
    }

    private int GetRandomAmmoAmount()
    {
        return Random.Range(_ammoRewardConfig.MinAddAmount, _ammoRewardConfig.MaxAddAmount + 1);
    }

    private static T PickRandom<T>(IReadOnlyList<T> items)
    {
        return items[Random.Range(0, items.Count)];
    }

    private static void LogInventoryFull()
    {
        Debug.LogError("\u0418\u043d\u0432\u0435\u043d\u0442\u0430\u0440\u044c \u043f\u043e\u043b\u043e\u043d");
    }

    private static void LogNoWeapons()
    {
        Debug.LogError("\u041d\u0435\u0442 \u043e\u0440\u0443\u0436\u0438\u044f");
    }

    private static void LogNoAmmo(WeaponDefinition weaponDefinition)
    {
        Debug.LogError($"\u041d\u0435\u0442 \u043f\u0430\u0442\u0440\u043e\u043d\u043e\u0432 \u0434\u043b\u044f {weaponDefinition.DisplayName}");
    }

    private static void LogItemAdded(ItemDefinition item, int slotIndex)
    {
        Debug.Log($"\u0414\u043e\u0431\u0430\u0432\u043b\u0435\u043d\u043e {item.DisplayName} \u0432 \u0441\u043b\u043e\u0442: {slotIndex + 1}");
    }

    private static void LogAmmoAdded(AmmoDefinition ammoDefinition, IReadOnlyList<InventorySlotAddResult> addResults)
    {
        foreach (InventorySlotAddResult addResult in addResults)
        {
            Debug.Log($"\u0414\u043e\u0431\u0430\u0432\u043b\u0435\u043d\u043e ({addResult.AddedCount}) {ammoDefinition.DisplayName} \u0432 \u0441\u043b\u043e\u0442: {addResult.SlotIndex + 1}");
        }
    }

    private static void LogShot(WeaponDefinition weaponDefinition)
    {
        string ammoName = weaponDefinition.AmmoDefinition != null
            ? weaponDefinition.AmmoDefinition.DisplayName
            : "\u041d\u0435\u0442";

        Debug.Log($"\u0412\u044b\u0441\u0442\u0440\u0435\u043b \u0438\u0437 {weaponDefinition.DisplayName}, \u043f\u0430\u0442\u0440\u043e\u043d\u044b: {ammoName}, \u0443\u0440\u043e\u043d: {weaponDefinition.Damage}");
    }
}
