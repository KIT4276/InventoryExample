using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/WalletBalanceConfig", fileName = "WalletBalanceConfig")]
public class WalletBalanceConfig : ScriptableObject
{
    [SerializeField] private int _initialCoins = 0;
    [SerializeField] private int _minAddOfCoins = 9;
    [SerializeField] private int _maxAddOfCoins = 99;

    public int InitialCoins => _initialCoins;

    public int MinAddOfCoins => _minAddOfCoins;
    public int MaxAddOfCoins => _maxAddOfCoins;
}
