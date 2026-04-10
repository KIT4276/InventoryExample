using Zenject;

public class InventoryInitializer : IInitializable
{
    private readonly InventoryData _inventoryData;
    private readonly InventoryService _inventoryService;
    private readonly InitialInventoryConfig _initialInventoryConfig;

    public InventoryInitializer(
        InventoryData inventoryData,
        InventoryService inventoryService,
        InitialInventoryConfig initialInventoryConfig)
    {
        _inventoryData = inventoryData;
        _inventoryService = inventoryService;
        _initialInventoryConfig = initialInventoryConfig;
    }

    public void Initialize()
    {
        _inventoryData.Initialize(_initialInventoryConfig);
        _inventoryService.LoadSlots();
    }
}
