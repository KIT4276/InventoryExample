using TMPro;
using UnityEngine;
using Zenject;

public class InventoryWeightView : MonoBehaviour
{
    [SerializeField] private string _prefix = "\u0412\u0435\u0441: ";
    [SerializeField] private TMP_Text _weightText;

    private InventoryService _inventoryService;

    [Inject]
    private void Construct(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    private void Start()
    {
        RefreshWeight();
        _inventoryService.Changed += RefreshWeight;
    }

    private void RefreshWeight()
    {
        if (_weightText == null || _inventoryService == null)
            return;

        _weightText.text = _prefix + _inventoryService.TotalWeight.ToString("0.###");
    }

    private void OnDestroy()
    {
        if (_inventoryService != null)
            _inventoryService.Changed -= RefreshWeight;
    }
}
