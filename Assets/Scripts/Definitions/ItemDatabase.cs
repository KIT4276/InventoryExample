using System.Collections.Generic;

public class ItemDatabase
{
    private readonly Dictionary<int, ItemDefinition> _definitionsById = new();

    public IReadOnlyDictionary<int, ItemDefinition> ItemDefinitions => _definitionsById;

    public ItemDatabase(ItemDatabaseConfig config)
    {
        if (config == null || config.ItemDefinitions == null)
            return;

        foreach (ItemDefinition itemDefinition in config.ItemDefinitions)
        {
            if (itemDefinition == null)
                continue;

            _definitionsById[itemDefinition.ID] = itemDefinition;
        }
    }

    public bool TryGetById(int itemId, out ItemDefinition itemDefinition)
    {
        return _definitionsById.TryGetValue(itemId, out itemDefinition);
    }

    public bool TryGetById<TDefinition>(int itemId, out TDefinition itemDefinition) where TDefinition : ItemDefinition
    {
        itemDefinition = null;

        if (!_definitionsById.TryGetValue(itemId, out ItemDefinition baseDefinition))
            return false;

        itemDefinition = baseDefinition as TDefinition;
        return itemDefinition != null;
    }
}
