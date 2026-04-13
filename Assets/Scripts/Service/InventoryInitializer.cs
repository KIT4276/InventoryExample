using Zenject;

public class InventoryInitializer : IInitializable
{
    private readonly InventoryData _inventoryData;
    private readonly InventoryMutationService _inventoryMutationService;
    private readonly InitialInventoryConfig _initialInventoryConfig;

    public InventoryInitializer(
        InventoryData inventoryData,
        InventoryMutationService inventoryMutationService,
        InitialInventoryConfig initialInventoryConfig)
    {
        _inventoryData = inventoryData;
        _inventoryMutationService = inventoryMutationService;
        _initialInventoryConfig = initialInventoryConfig;
    }

    public void Initialize()
    {
        _inventoryData.Initialize(_initialInventoryConfig);
        _inventoryMutationService.LoadSlots();
    }
}
