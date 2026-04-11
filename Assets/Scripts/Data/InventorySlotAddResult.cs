public readonly struct InventorySlotAddResult
{
    public int SlotIndex { get; }
    public int AddedCount { get; }

    public InventorySlotAddResult(int slotIndex, int addedCount)
    {
        SlotIndex = slotIndex;
        AddedCount = addedCount;
    }
}
