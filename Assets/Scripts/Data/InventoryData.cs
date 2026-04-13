using System.Collections.Generic;

public class InventoryData
{
    private readonly ItemDatabase _itemDatabase;
    private readonly List<InventorySlotData> _slots = new();

    public IReadOnlyList<InventorySlotData> Slots => _slots;

    public InventoryData(ItemDatabase itemDatabase)
    {
        _itemDatabase = itemDatabase;
    }

    public void Initialize(InitialInventoryConfig initialInventoryConfig)
    {
        _slots.Clear();

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
            _slots.Add(slot);
        }
    }

    public IReadOnlyList<InventorySlotEntry> GetOccupiedUnlockedSlots()
    {
        List<InventorySlotEntry> occupiedSlots = new();

        for (int i = 0; i < _slots.Count; i++)
        {
            InventorySlotData slot = _slots[i];
            if (slot == null || !slot.IsUnlocked || slot.IsEmpty)
                continue;

            occupiedSlots.Add(new InventorySlotEntry(i, slot));
        }

        return occupiedSlots;
    }

    public IReadOnlyList<WeaponDefinition> GetWeapons()
    {
        List<WeaponDefinition> weapons = new();

        foreach (InventorySlotEntry occupiedSlot in GetOccupiedUnlockedSlots())
        {
            if (!TryGetItemDefinition(occupiedSlot.SlotData.ItemId, out WeaponDefinition weaponDefinition))
                continue;

            weapons.Add(weaponDefinition);
        }

        return weapons;
    }

    public IReadOnlyList<InventorySlotEntry> GetAmmoStacksFor(AmmoDefinition ammoDefinition)
    {
        List<InventorySlotEntry> ammoStacks = new();
        if (ammoDefinition == null)
            return ammoStacks;

        foreach (InventorySlotEntry occupiedSlot in GetOccupiedUnlockedSlots())
        {
            if (occupiedSlot.SlotData.ItemId != ammoDefinition.ID)
                continue;

            ammoStacks.Add(occupiedSlot);
        }

        return ammoStacks;
    }

    public bool TryGetFirstFreeUnlockedSlot(out InventorySlotEntry freeSlot)
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            InventorySlotData slot = _slots[i];
            if (slot == null || !slot.IsUnlocked || !slot.IsEmpty)
                continue;

            freeSlot = new InventorySlotEntry(i, slot);
            return true;
        }

        freeSlot = default;
        return false;
    }

    public bool TryAddOneItemToFreeSlot(ItemDefinition item)
    {
        if (item == null)
            return false;

        if (!TryGetFirstFreeUnlockedSlot(out InventorySlotEntry freeSlot))
            return false;

        return freeSlot.SlotData.AddItemWithRemainder(item.ID, 1, item.MaxStack) == 0;
    }

    public int AddItems(ItemDefinition item, int count, List<InventorySlotAddResult> addResults)
    {
        if (item == null || count <= 0)
            return count;

        int remainder = count;
        addResults ??= new List<InventorySlotAddResult>();

        remainder = AddItemsToExistingStacks(item, remainder, addResults);
        if (remainder <= 0)
            return 0;

        return AddItemsToEmptySlots(item, remainder, addResults);
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

        foreach (InventorySlotEntry ammoSlot in GetAmmoStacksFor(weaponDefinition.AmmoDefinition))
        {
            if (ammoSlot.SlotData.TryRemoveItems(1))
                return true;
        }

        return false;
    }

    public bool TryUnlockSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Count)
            return false;

        InventorySlotData slot = _slots[slotIndex];
        if (slot.IsUnlocked)
            return false;

        slot.SetUnlocked(true);
        return true;
    }

    public bool TryClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Count)
            return false;

        InventorySlotData slot = _slots[slotIndex];
        if (!slot.IsUnlocked || slot.IsEmpty)
            return false;

        slot.Clear();
        return true;
    }

    public List<InventorySlotSaveData> CreateSaveData()
    {
        List<InventorySlotSaveData> saveSlots = new();

        foreach (InventorySlotData slot in _slots)
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

        int count = saveSlots.Count < _slots.Count ? saveSlots.Count : _slots.Count;

        for (int i = 0; i < count; i++)
        {
            InventorySlotSaveData saveSlot = saveSlots[i];
            InventorySlotData slot = _slots[i];

            slot.SetUnlocked(saveSlot.isUnlocked);
            slot.SetItem(saveSlot.itemId, saveSlot.count);
        }
    }

    private int AddItemsToExistingStacks(ItemDefinition item, int count, List<InventorySlotAddResult> addResults)
    {
        int remainder = count;

        for (int i = 0; i < _slots.Count; i++)
        {
            InventorySlotData slot = _slots[i];
            if (!slot.IsUnlocked || slot.IsEmpty)
                continue;

            if (slot.ItemId != item.ID || slot.Count >= item.MaxStack)
                continue;

            int beforeCount = slot.Count;
            remainder = slot.AddItemWithRemainder(item.ID, remainder, item.MaxStack);
            int added = slot.Count - beforeCount;
            if (added > 0)
                addResults.Add(new InventorySlotAddResult(i, added));

            if (remainder <= 0)
                break;
        }

        return remainder;
    }

    private int AddItemsToEmptySlots(ItemDefinition item, int count, List<InventorySlotAddResult> addResults)
    {
        int remainder = count;

        for (int i = 0; i < _slots.Count; i++)
        {
            InventorySlotData slot = _slots[i];
            if (!slot.IsUnlocked || !slot.IsEmpty)
                continue;

            int beforeCount = slot.Count;
            remainder = slot.AddItemWithRemainder(item.ID, remainder, item.MaxStack);
            int added = slot.Count - beforeCount;
            if (added > 0)
                addResults.Add(new InventorySlotAddResult(i, added));

            if (remainder <= 0)
                break;
        }

        return remainder;
    }

    private bool TryGetItemDefinition(int itemId, out ItemDefinition itemDefinition)
    {
        itemDefinition = null;

        if (_itemDatabase == null)
            return false;

        return _itemDatabase.TryGetById(itemId, out itemDefinition);
    }

    private bool TryGetItemDefinition<TDefinition>(int itemId, out TDefinition itemDefinition) where TDefinition : ItemDefinition
    {
        itemDefinition = null;

        if (_itemDatabase == null)
            return false;

        return _itemDatabase.TryGetById(itemId, out itemDefinition);
    }
}
