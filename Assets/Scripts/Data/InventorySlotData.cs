using UnityEngine;

public class InventorySlotData
{
    public bool IsUnlocked { get; private set; }
    public int ItemId { get; private set; }
    public int Count { get; private set; }

    public bool IsEmpty => Count == 0;

    public void SetUnlocked(bool isUnlocked)
    {
        IsUnlocked = isUnlocked;
    }

    public void SetItem(int itemId, int count)
    {
        if (count <= 0)
        {
            Clear();
            return;
        }

        ItemId = itemId;
        Count = count;
    }

    public void Clear()
    {
        ItemId = 0;
        Count = 0;
    }

    public int AddItemWithRemainder(int itemId, int count, int maxStack)
    {
        if (count <= 0)
        {
            Debug.LogError("[InventorySlotData] attempt to add a negative value");
            return count;
        }

        if (!IsEmpty && ItemId != itemId)
            return count;

        ItemId = itemId;

        int slotLimit = Mathf.Max(1, maxStack);
        int freeSpace = slotLimit - Count;
        if (freeSpace <= 0)
            return count;

        int added = Mathf.Min(count, freeSpace);
        Count += added;

        return count - added;
    }

    public bool TryRemoveItems(int count)
    {
        if (count <= 0 || Count < count)
            return false;

        Count -= count;

        if (Count == 0)
            Clear();

        return true;
    }
}
