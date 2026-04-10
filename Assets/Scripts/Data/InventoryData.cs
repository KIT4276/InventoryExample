using System.Collections.Generic;

public class InventoryData
{
    private readonly ItemDatabase _itemDatabase;

    public List<InventorySlotData> Slots { get; private set; } = new();

    public InventoryData(ItemDatabase itemDatabase)
    {
        _itemDatabase = itemDatabase;
    }

    public void Initialize(InitialInventoryConfig initialInventoryConfig)
    {
        Slots.Clear();

        if (initialInventoryConfig == null)
            return;

        int totalSlots = initialInventoryConfig.TotalSlots < 0 ? 0 : initialInventoryConfig.TotalSlots;
        int initialUnlocked = initialInventoryConfig.InitialUnlocked;

        if (initialUnlocked < 0)
            initialUnlocked = 0;
        if (initialUnlocked > totalSlots)
            initialUnlocked = totalSlots;

        for (int i = 0; i < totalSlots; i++)
        {
            InventorySlotData slot = new();
            slot.SetUnlocked(i < initialUnlocked);
            Slots.Add(slot);
        }
    }

    public bool TryAddOneItemToFreeSlot(ItemDefinition item)
    {
        if (item == null)
            return false;

        if (!TryGetFirstFreeSlot(out InventorySlotData freeSlot))
            return false;

        return freeSlot.AddItemWithRemainder(item.ID, 1, item.MaxStack) == 0;
    }

    public bool TryStackSlot(InventorySlotData sourceSlot, InventorySlotData targetSlot)
    {
        if (sourceSlot == null || targetSlot == null)
            return false;

        if (sourceSlot == targetSlot)
            return false;

        if (!sourceSlot.IsUnlocked || !targetSlot.IsUnlocked)
            return false;

        if (sourceSlot.IsEmpty || targetSlot.IsEmpty)
            return false;

        if (sourceSlot.ItemId != targetSlot.ItemId)
            return false;

        if (!TryGetItemDefinition(sourceSlot.ItemId, out ItemDefinition itemDefinition))
            return false;

        int remainder = targetSlot.AddItemWithRemainder(sourceSlot.ItemId, sourceSlot.Count, itemDefinition.MaxStack);
        if (remainder == sourceSlot.Count)
            return false;

        if (remainder == 0)
        {
            sourceSlot.Clear();
            return true;
        }

        sourceSlot.SetItem(sourceSlot.ItemId, remainder);
        return true;
    }

    public bool TryConsumeAmmoForWeapon(WeaponDefinition weaponDefinition)
    {
        if (weaponDefinition == null || weaponDefinition.AmmoDefinition == null)
            return false;

        foreach (InventorySlotData slot in Slots)
        {
            if (!slot.IsUnlocked || slot.IsEmpty)
                continue;

            if (!weaponDefinition.CanUseAmmo(slot.ItemId))
                continue;

            return slot.TryRemoveItems(1);
        }

        return false;
    }

    public bool TryUnlockSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= Slots.Count)
            return false;

        InventorySlotData slot = Slots[slotIndex];
        if (slot.IsUnlocked)
            return false;

        slot.SetUnlocked(true);
        return true;
    }

    public List<InventorySlotSaveData> CreateSaveData()
    {
        List<InventorySlotSaveData> saveSlots = new();

        foreach (InventorySlotData slot in Slots)
        {
            saveSlots.Add(new InventorySlotSaveData
            {
                isUnlocked = slot.IsUnlocked,
                itemId = slot.ItemId,
                count = slot.Count
            });
        }

        return saveSlots;
    }

    public void ApplySaveData(List<InventorySlotSaveData> saveSlots)
    {
        if (saveSlots == null)
            return;

        int count = saveSlots.Count < Slots.Count ? saveSlots.Count : Slots.Count;

        for (int i = 0; i < count; i++)
        {
            InventorySlotSaveData saveSlot = saveSlots[i];
            InventorySlotData slot = Slots[i];

            slot.SetUnlocked(saveSlot.isUnlocked);
            slot.SetItem(saveSlot.itemId, saveSlot.count);
        }
    }

    private bool TryGetFirstFreeSlot(out InventorySlotData freeSlot)
    {
        freeSlot = null;

        foreach (InventorySlotData slot in Slots)
        {
            if (!slot.IsUnlocked || !slot.IsEmpty)
                continue;

            freeSlot = slot;
            return true;
        }

        return false;
    }

    private bool TryGetItemDefinition(int itemId, out ItemDefinition itemDefinition)
    {
        itemDefinition = null;

        if (_itemDatabase == null)
            return false;

        return _itemDatabase.TryGetById(itemId, out itemDefinition);
    }
}
