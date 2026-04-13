using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DeleteItemButton : MonoBehaviour
{
    [SerializeField] private Button _removeItem;

    private InventoryMutationService _inventoryMutationService;

    [Inject]
    private void Construct(InventoryMutationService inventoryMutationService)
    {
        _inventoryMutationService = inventoryMutationService;
    }

    private void Start()
    {
        _removeItem.onClick.AddListener(HandleRemoveItem);
    }

    private void HandleRemoveItem()
    {
        _inventoryMutationService.TryRemoveRandomItem();
    }

    private void OnDestroy()
    {
        _removeItem.onClick.RemoveListener(HandleRemoveItem);
    }
}
