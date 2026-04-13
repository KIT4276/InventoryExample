using TMPro;
using UnityEngine;
using Zenject;

public class InventoryWeightView : MonoBehaviour
{
    [SerializeField] private string _prefix = "\u0412\u0435\u0441: ";
    [SerializeField] private TMP_Text _weightText;

    private InventoryQueryService _inventoryQueryService;
    private InventoryMutationService _inventoryMutationService;

    [Inject]
    private void Construct(InventoryQueryService inventoryQueryService, InventoryMutationService inventoryMutationService)
    {
        _inventoryQueryService = inventoryQueryService;
        _inventoryMutationService = inventoryMutationService;
    }

    private void Start()
    {
        RefreshWeight();
        _inventoryMutationService.Changed += RefreshWeight;
    }

    private void RefreshWeight()
    {
        if (_weightText == null || _inventoryQueryService == null)
            return;

        _weightText.text = _prefix + _inventoryQueryService.TotalWeight.ToString("0.###");
    }

    private void OnDestroy()
    {
        if (_inventoryMutationService != null)
            _inventoryMutationService.Changed -= RefreshWeight;
    }
}
