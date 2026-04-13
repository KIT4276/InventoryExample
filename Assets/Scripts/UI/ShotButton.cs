using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ShotButton : MonoBehaviour
{
    [SerializeField] private Button _shot;

    private InventoryActionService _inventoryActionService;

    [Inject]
    private void Construct(InventoryActionService inventoryActionService)
    {
        _inventoryActionService = inventoryActionService;
    }

    private void Start()
    {
        _shot.onClick.AddListener(HandleShot);
    }

    private void HandleShot()
    {
        _inventoryActionService.TryShootRandomWeapon();
    }

    private void OnDestroy()
    {
        _shot.onClick.RemoveListener(HandleShot);
    }
}
