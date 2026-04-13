using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMutationService
{
    private readonly InventoryData _inventoryData;
    private readonly InventoryQueryService _inventoryQueryService;
    private readonly InitialInventoryConfig _initialInventoryConfig;
    private readonly WalletService _walletService;
    private readonly SaveLoadService _saveLoadService;
    private readonly ItemDatabase _itemDatabase;

    public event Action Changed;

    public InventoryMutationService(
        InventoryData inventoryData,
        InventoryQueryService inventoryQueryService,
        InitialInventoryConfig initialInventoryConfig,
        WalletService walletService,
        SaveLoadService saveLoadService,
        ItemDatabase itemDatabase)
    {
        _inventoryData = inventoryData;
        _inventoryQueryService = inventoryQueryService;
        _initialInventoryConfig = initialInventoryConfig;
        _walletService = walletService;
        _saveLoadService = saveLoadService;
        _itemDatabase = itemDatabase;
    }

    public bool TryAddOneItemToFreeSlot(ItemDefinition item)
    {
        return ExecuteMutation(() => _inventoryData.TryAddOneItemToFreeSlot(item));
    }

    public bool TryAddItems(ItemDefinition item, int count, List<InventorySlotAddResult> addResults, out int remainder)
    {
        remainder = _inventoryData.AddItems(item, count, addResults);
        bool hasChanges = addResults != null && addResults.Count > 0;
        if (hasChanges)
            PersistAndNotify();

        return hasChanges;
    }

    public bool TryStackSlots(InventorySlotData sourceSlot, InventorySlotData targetSlot)
    {
        return ExecuteMutation(() => _inventoryData.TryStackSlot(sourceSlot, targetSlot));
    }

    public bool TryConsumeAmmoForWeapon(WeaponDefinition weaponDefinition)
    {
        return ExecuteMutation(() => _inventoryData.TryConsumeAmmoForWeapon(weaponDefinition));
    }

    public bool TryUnlockSlot(int slotIndex)
    {
        if (!CanUnlockSlot(slotIndex, out int cost))
            return false;

        if (!_walletService.TrySpendCoins(cost))
            return false;

        return ExecuteMutation(() => _inventoryData.TryUnlockSlot(slotIndex));
    }

    public bool CanUnlockSlot(int slotIndex, out int cost)
    {
        cost = 0;

        if (slotIndex < 0 || slotIndex >= _inventoryData.Slots.Count)
            return false;

        InventorySlotData slot = _inventoryData.Slots[slotIndex];
        if (slot.IsUnlocked)
            return false;

        if (slotIndex > 0 && !_inventoryData.Slots[slotIndex - 1].IsUnlocked)
            return false;

        if (_initialInventoryConfig == null || !_initialInventoryConfig.TryGetUnlockCost(slotIndex, out cost))
            return false;

        return _walletService.CanSpend(cost);
    }

    public bool TryRemoveRandomItem()
    {
        IReadOnlyList<InventorySlotEntry> occupiedSlots = _inventoryQueryService.GetOccupiedUnlockedSlots();
        if (occupiedSlots.Count == 0)
        {
            LogInventoryEmpty();
            return false;
        }

        InventorySlotEntry slotEntry = PickRandom(occupiedSlots);
        int removedCount = slotEntry.SlotData.Count;
        string itemName = ResolveItemName(slotEntry.SlotData.ItemId);

        bool removed = ExecuteMutation(() => _inventoryData.TryClearSlot(slotEntry.SlotIndex));
        if (!removed)
            return false;

        LogItemRemoved(itemName, removedCount, slotEntry.SlotIndex);
        return true;
    }

    public void LoadSlots()
    {
        GameSaveData saveData = _saveLoadService.Load();
        _inventoryData.ApplySaveData(saveData.slots);
        Changed?.Invoke();
    }

    private string ResolveItemName(int itemId)
    {
        return _itemDatabase.TryGetById(itemId, out ItemDefinition itemDefinition) && itemDefinition != null
            ? itemDefinition.DisplayName
            : itemId.ToString();
    }

    private static T PickRandom<T>(IReadOnlyList<T> items)
    {
        return items[UnityEngine.Random.Range(0, items.Count)];
    }

    private bool ExecuteMutation(Func<bool> mutation)
    {
        bool result = mutation();
        if (!result)
            return false;

        PersistAndNotify();
        return true;
    }

    private void PersistAndNotify()
    {
        GameSaveData saveData = _saveLoadService.Load();
        saveData.slots = _inventoryData.CreateSaveData();
        _saveLoadService.Save(saveData);
        Changed?.Invoke();
    }

    private static void LogInventoryEmpty()
    {
        Debug.LogError("\u0418\u043d\u0432\u0435\u043d\u0442\u0430\u0440\u044c \u043f\u0443\u0441\u0442");
    }

    private static void LogItemRemoved(string itemName, int removedCount, int slotIndex)
    {
        Debug.Log($"\u0423\u0434\u0430\u043b\u0435\u043d\u043e ({removedCount}) {itemName} \u0438\u0437 \u0441\u043b\u043e\u0442\u0430: {slotIndex + 1}");
    }
}
