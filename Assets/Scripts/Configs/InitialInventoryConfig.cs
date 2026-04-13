using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/InitialInventoryConfig", fileName = "InitialInventoryConfig")]
public class InitialInventoryConfig : ScriptableObject
{
    [SerializeField] private int _totalSlots = 50;
    [SerializeField] private int _initialUnlocked = 15;
    [SerializeField] private int[] _slotUnlockCosts =
    {
        10, 15, 20, 25, 30, 35, 40, 45, 50, 55,
        60, 65, 70, 75, 80, 85, 90, 95, 100, 105,
        110, 115, 120, 125, 130, 135, 140, 145, 150, 155,
        160, 165, 170, 175, 180, 185, 190, 195, 200, 205,
        210, 215, 220, 225, 230, 235, 240, 245, 250, 255
    };

    public int TotalSlots => _totalSlots;
    public int InitialUnlocked => _initialUnlocked;
    public IReadOnlyList<int> SlotUnlockCosts => _slotUnlockCosts;

    public bool TryGetUnlockCost(int slotIndex, out int cost)
    {
        cost = 0;

        if (_slotUnlockCosts == null)
            return false;

        if (slotIndex < 0 || slotIndex >= _slotUnlockCosts.Length)
            return false;

        cost = _slotUnlockCosts[slotIndex];
        return cost >= 0;
    }
}
