using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AddItemButton : MonoBehaviour
{
    [SerializeField] private Button _addItem;

    private InventoryActionService _inventoryActionService;

    [Inject]
    private void Construct(InventoryActionService inventoryActionService)
    {
        _inventoryActionService = inventoryActionService;
    }

    private void Start()
    {
        _addItem.onClick.AddListener(AddItemPressed);
    }

    private void AddItemPressed()
    {
        _inventoryActionService.TryAddRandomItem();
    }

    private void OnDestroy()
    {
        _addItem.onClick.RemoveListener(AddItemPressed);
    }
}
