using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ItemDatabaseConfig", fileName = "ItemDatabaseConfig")]
public class ItemDatabaseConfig : ScriptableObject
{
    [SerializeField] private ItemDefinition[] _itemDefinitions;

    public ItemDefinition[] ItemDefinitions => _itemDefinitions;
}
