using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int saveVersion = 1;
    public int coins;
    public List<InventorySlotSaveData> slots = new();
}
