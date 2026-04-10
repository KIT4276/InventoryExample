using System.Collections.Generic;

public class InventoryService
{
    private readonly InventoryData _inventoryData;
    private readonly InitialInventoryConfig _initialInventoryConfig;
    private readonly WalletService _walletService;
    private readonly SaveLoadService _saveLoadService;

    public IReadOnlyList<InventorySlotData> Slots => _inventoryData.Slots;

    public InventoryService(
        InventoryData inventoryData,
        InitialInventoryConfig initialInventoryConfig,
        WalletService walletService,
        SaveLoadService saveLoadService)
    {
        _inventoryData = inventoryData;
        _initialInventoryConfig = initialInventoryConfig;
        _walletService = walletService;
        _saveLoadService = saveLoadService;
    }

    public bool TryAddOneItemToFreeSlot(ItemDefinition item)
    {
        bool result = _inventoryData.TryAddOneItemToFreeSlot(item);
        if (result)
            SaveSlots();

        return result;
    }

    public bool TryStackSlots(InventorySlotData sourceSlot, InventorySlotData targetSlot)
    {
        bool result = _inventoryData.TryStackSlot(sourceSlot, targetSlot);
        if (result)
            SaveSlots();

        return result;
    }

    public bool TryConsumeAmmoForWeapon(WeaponDefinition weaponDefinition)
    {
        bool result = _inventoryData.TryConsumeAmmoForWeapon(weaponDefinition);
        if (result)
            SaveSlots();

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
            SaveSlots();

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
    }

    private void SaveSlots()
    {
        GameSaveData saveData = _saveLoadService.Load();
        saveData.slots = _inventoryData.CreateSaveData();
        _saveLoadService.Save(saveData);
    }
}
