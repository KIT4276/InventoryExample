using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryService
{
    private readonly InventoryData _inventoryData;
    private readonly InitialInventoryConfig _initialInventoryConfig;
    private readonly AmmoRewardConfig _ammoRewardConfig;
    private readonly WalletService _walletService;
    private readonly SaveLoadService _saveLoadService;
    private readonly ItemDatabase _itemDatabase;

    public event Action Changed;

    public IReadOnlyList<InventorySlotData> Slots => _inventoryData.Slots;
    public float TotalWeight => CalculateTotalWeight();

    public InventoryService(
        InventoryData inventoryData,
        InitialInventoryConfig initialInventoryConfig,
        AmmoRewardConfig ammoRewardConfig,
        WalletService walletService,
        SaveLoadService saveLoadService,
        ItemDatabase itemDatabase)
    {
        _inventoryData = inventoryData;
        _initialInventoryConfig = initialInventoryConfig;
        _ammoRewardConfig = ammoRewardConfig;
        _walletService = walletService;
        _saveLoadService = saveLoadService;
        _itemDatabase = itemDatabase;
    }

    public void TryAddRandomItem()
    {
        if (_itemDatabase == null || _itemDatabase.ItemDefinitions == null || _itemDatabase.ItemDefinitions.Count == 0)
        {
            return;
        }

        List<ItemDefinition> possibleItems = new();

        foreach (ItemDefinition item in _itemDatabase.ItemDefinitions.Values)
        {
            if (item is AmmoDefinition)
                continue;

            possibleItems.Add(item);
        }

        if (possibleItems.Count == 0)
        {
            return;
        }

        InventorySlotData freeSlot = null;
        int freeSlotId = -1;

        for (int i = 0; i < _inventoryData.Slots.Count; i++)
        {
            InventorySlotData slot = _inventoryData.Slots[i];

            if (!slot.IsUnlocked || !slot.IsEmpty)
                continue;

            freeSlot = slot;
            freeSlotId = i;
            break;
        }

        if (freeSlot == null)
        {
            Debug.LogError("Инвентарь полон");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, possibleItems.Count);
        ItemDefinition randomItem = possibleItems[randomIndex];

        bool added = _inventoryData.TryAddOneItemToFreeSlot(randomItem);
        if (!added)
        {
            return;
        }

        SaveSlots();
        Debug.Log($"Добавлено {randomItem.DisplayName} в слот: {freeSlotId+1}");
        NotifyChanged();
    }

    public void TryAddRandomAmmo()
    {
        if (_ammoRewardConfig == null || _ammoRewardConfig.AvailableAmmoDefinitions == null || _ammoRewardConfig.AvailableAmmoDefinitions.Length == 0)
            return;

        List<AmmoDefinition> possibleAmmo = new();

        foreach (AmmoDefinition ammoDefinition in _ammoRewardConfig.AvailableAmmoDefinitions)
        {
            if (ammoDefinition != null)
                possibleAmmo.Add(ammoDefinition);
        }

        if (possibleAmmo.Count == 0)
            return;

        int ammoIndex = UnityEngine.Random.Range(0, possibleAmmo.Count);
        AmmoDefinition randomAmmo = possibleAmmo[ammoIndex];
        int addAmount = UnityEngine.Random.Range(_ammoRewardConfig.MinAddAmount, _ammoRewardConfig.MaxAddAmount + 1);

        List<InventorySlotAddResult> addResults = new();
        int remainder = _inventoryData.AddItems(randomAmmo, addAmount, addResults);

        if (addResults.Count == 0)
        {
            Debug.LogError("\u0418\u043d\u0432\u0435\u043d\u0442\u0430\u0440\u044c \u043f\u043e\u043b\u043e\u043d");
            return;
        }

        SaveSlots();

        foreach (InventorySlotAddResult addResult in addResults)
        {
            Debug.Log($"\u0414\u043e\u0431\u0430\u0432\u043b\u0435\u043d\u043e ({addResult.AddedCount}) {randomAmmo.DisplayName} \u0432 \u0441\u043b\u043e\u0442: {addResult.SlotIndex + 1}");
        }

        if (remainder > 0)
            Debug.LogError("\u0418\u043d\u0432\u0435\u043d\u0442\u0430\u0440\u044c \u043f\u043e\u043b\u043e\u043d");

        NotifyChanged();
    }

    public bool TryAddOneItemToFreeSlot(ItemDefinition item)
    {
        bool result = _inventoryData.TryAddOneItemToFreeSlot(item);
        if (result)
        {
            SaveSlots();
            NotifyChanged();
        }

        return result;
    }

    public bool TryStackSlots(InventorySlotData sourceSlot, InventorySlotData targetSlot)
    {
        bool result = _inventoryData.TryStackSlot(sourceSlot, targetSlot);
        if (result)
        {
            SaveSlots();
            NotifyChanged();
        }

        return result;
    }

    public bool TryConsumeAmmoForWeapon(WeaponDefinition weaponDefinition)
    {
        bool result = _inventoryData.TryConsumeAmmoForWeapon(weaponDefinition);
        if (result)
        {
            SaveSlots();
            NotifyChanged();
        }

        return result;
    }

    public bool TryUnlockSlot(int slotIndex)
    {
        if (!CanUnlockSlot(slotIndex, out int cost))
            return false;

        if (!_walletService.TrySpendCoins(cost))
            return false;

        bool result = _inventoryData.TryUnlockSlot(slotIndex);
        if (result)
        {
            SaveSlots();
            NotifyChanged();
        }

        return result;
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

    public void LoadSlots()
    {
        GameSaveData saveData = _saveLoadService.Load();
        _inventoryData.ApplySaveData(saveData.slots);
        NotifyChanged();
    }

    private void SaveSlots()
    {
        GameSaveData saveData = _saveLoadService.Load();
        saveData.slots = _inventoryData.CreateSaveData();
        _saveLoadService.Save(saveData);
    }

    private float CalculateTotalWeight()
    {
        float totalWeight = 0f;

        foreach (InventorySlotData slot in _inventoryData.Slots)
        {
            if (slot == null || !slot.IsUnlocked || slot.IsEmpty)
                continue;

            if (!_itemDatabase.TryGetById(slot.ItemId, out ItemDefinition itemDefinition) || itemDefinition == null)
                continue;

            totalWeight += itemDefinition.Weight * slot.Count;
        }

        return totalWeight;
    }

    private void NotifyChanged()
    {
        Changed?.Invoke();
    }

}
