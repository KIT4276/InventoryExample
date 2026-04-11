using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AddItemButton : MonoBehaviour
{
    [SerializeField] private Button _addItem;

    private InventoryService _inventoryService;

    [Inject]
    private void Construct(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    private void Start()
    {
        _addItem.onClick.AddListener(AddItemPressed);
    }

    private void AddItemPressed()
    {
        _inventoryService.TryAddRandomItem();
    }

    private void OnDestroy()
    {
        _addItem.onClick.RemoveListener(AddItemPressed);

    }
}