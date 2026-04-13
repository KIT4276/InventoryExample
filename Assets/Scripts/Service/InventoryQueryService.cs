using System.Collections.Generic;

public class InventoryQueryService
{
    private readonly InventoryData _inventoryData;
    private readonly ItemDatabase _itemDatabase;

    public IReadOnlyList<InventorySlotData> Slots => _inventoryData.Slots;
    public float TotalWeight => CalculateTotalWeight();

    public InventoryQueryService(InventoryData inventoryData, ItemDatabase itemDatabase)
    {
        _inventoryData = inventoryData;
        _itemDatabase = itemDatabase;
    }

    public IReadOnlyList<InventorySlotEntry> GetOccupiedUnlockedSlots()
    {
        return _inventoryData.GetOccupiedUnlockedSlots();
    }

    public IReadOnlyList<WeaponDefinition> GetWeapons()
    {
        return _inventoryData.GetWeapons();
    }

    public IReadOnlyList<InventorySlotEntry> GetAmmoStacksFor(AmmoDefinition ammoDefinition)
    {
        return _inventoryData.GetAmmoStacksFor(ammoDefinition);
    }

    public bool TryGetFirstFreeUnlockedSlot(out InventorySlotEntry freeSlot)
    {
        return _inventoryData.TryGetFirstFreeUnlockedSlot(out freeSlot);
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
}
