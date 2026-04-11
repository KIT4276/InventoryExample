using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AddAmmoButton : MonoBehaviour
{
    [SerializeField] private Button _addAmmo;

    private InventoryService _inventoryService;

    [Inject]
    private void Construct(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    private void Start()
    {
        _addAmmo.onClick.AddListener(AddAmmoPressed);
    }

    private void AddAmmoPressed()
    {
        _inventoryService.TryAddRandomAmmo();
    }

    private void OnDestroy()
    {
        _addAmmo.onClick.RemoveListener(AddAmmoPressed);
    }
}
