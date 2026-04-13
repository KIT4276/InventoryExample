public readonly struct InventorySlotEntry
{
    public int SlotIndex { get; }
    public InventorySlotData SlotData { get; }

    public InventorySlotEntry(int slotIndex, InventorySlotData slotData)
    {
        SlotIndex = slotIndex;
        SlotData = slotData;
    }
}
