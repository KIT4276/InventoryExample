using System.Collections.Generic;
using Zenject;
using UnityEngine;

public class InventoryGridView : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private InventorySlotView _slotPrefab;

    [Inject] private InitialInventoryConfig _initialInventoryConfig;
    [Inject] private InventoryQueryService _inventoryQueryService;
    [Inject] private InventoryMutationService _inventoryMutationService;
    [Inject] private ItemDatabase _itemDatabase;

    private readonly List<InventorySlotView> _slotViews = new();

    private void Start()
    {
        Build();
        _inventoryMutationService.Changed += RefreshView;
        RefreshView();
    }

    public void RefreshView()
    {
        IReadOnlyList<InventorySlotData> slots = _inventoryQueryService.Slots;

        for (int i = 0; i < _slotViews.Count; i++)
        {
            InventorySlotData slotData = i < slots.Count ? slots[i] : null;
            ItemDefinition itemDefinition = null;

            if (slotData != null && !slotData.IsEmpty)
                _itemDatabase.TryGetById(slotData.ItemId, out itemDefinition);

            _slotViews[i].Render(i, i + 1, slotData, itemDefinition);
        }
    }

    private void Build()
    {
        if (_slotPrefab == null || _content == null)
            return;

        ClearSpawnedSlots();

        int totalSlots = _initialInventoryConfig != null ? _initialInventoryConfig.TotalSlots : 0;

        for (int i = 0; i < totalSlots; i++)
        {
            InventorySlotView slotView = Instantiate(_slotPrefab, _content);
            slotView.BindClick(HandleSlotClick);
            _slotViews.Add(slotView);
        }
    }

    private void ClearSpawnedSlots()
    {
        foreach (InventorySlotView slotView in _slotViews)
        {
            if (slotView != null)
                Destroy(slotView.gameObject);
        }

        _slotViews.Clear();
    }

    private void HandleSlotClick(InventorySlotView clickedSlotView)
    {
        if (clickedSlotView == null || clickedSlotView.SlotData == null)
            return;

        if (!clickedSlotView.SlotData.IsUnlocked && _inventoryMutationService.TryUnlockSlot(clickedSlotView.SlotIndex))
            RefreshView();
    }

    private void OnDestroy()
    {
        _inventoryMutationService.Changed -= RefreshView;
    }
}
