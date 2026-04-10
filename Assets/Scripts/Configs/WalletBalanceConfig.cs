using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/WalletBalanceConfig", fileName = "WalletBalanceConfig")]
public class WalletBalanceConfig : ScriptableObject
{
    [SerializeField] private int _initialCoins = 0;

    public int InitialCoins => _initialCoins;
}
