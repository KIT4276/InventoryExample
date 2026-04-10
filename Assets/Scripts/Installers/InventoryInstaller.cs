using System;
using UnityEngine;
using Zenject;

public class InventoryInstaller : MonoInstaller
{
    [SerializeField] private InitialInventoryConfig _initialInventoryConfig;
    [SerializeField] private ItemDatabaseConfig _itemDatabaseConfig;

    public override void InstallBindings()
    {
        ItemDefinition[] itemDefinitions = _itemDatabaseConfig != null
            ? _itemDatabaseConfig.ItemDefinitions ?? Array.Empty<ItemDefinition>()
            : Array.Empty<ItemDefinition>();

        Container.Bind<InitialInventoryConfig>().FromInstance(_initialInventoryConfig).AsSingle();
        Container.Bind<ItemDatabaseConfig>().FromInstance(_itemDatabaseConfig).AsSingle();
        Container.Bind<ItemDefinition[]>().FromInstance(itemDefinitions).AsSingle();

        foreach (ItemDefinition itemDefinition in itemDefinitions)
        {
            if (itemDefinition == null)
                continue;

            Container.Bind<ItemDefinition>().WithId(itemDefinition.ID).FromInstance(itemDefinition);
        }

        Container.Bind<ItemDatabase>().AsSingle();
        Container.Bind<InventoryData>().AsSingle();
        Container.Bind<InventoryService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InventoryInitializer>().AsSingle().NonLazy();
    }
}
