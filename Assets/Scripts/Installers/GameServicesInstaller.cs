using UnityEngine;
using Zenject;

public class GameServicesInstaller : MonoInstaller
{
    [SerializeField] private WalletBalanceConfig _walletBalanceConfig;

    public override void InstallBindings()
    {
        Container.Bind<WalletBalanceConfig>().FromInstance(_walletBalanceConfig).AsSingle();
        Container.Bind<SaveLoadService>().AsSingle();
        Container.Bind<WalletData>().AsSingle();
        Container.Bind<WalletService>().AsSingle();
        Container.Bind<GameplayService>().AsSingle();
    }
}
