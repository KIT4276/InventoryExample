using System;
using UnityEngine;

public class WalletService
{
    private readonly WalletData _walletData;
    private readonly SaveLoadService _saveLoadService;
    private readonly WalletBalanceConfig _walletBalanceConfig;

    public event Action<int> BalanceChanged;

    public int Balance => _walletData.Coins;

    public WalletService(
        WalletData walletData,
        SaveLoadService saveLoadService,
        WalletBalanceConfig walletBalanceConfig)
    {
        _walletData = walletData;
        _saveLoadService = saveLoadService;
        _walletBalanceConfig = walletBalanceConfig;

        LoadBalance();
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        _walletData.AddCoins(amount);
        SaveBalance();
        BalanceChanged?.Invoke(_walletData.Coins);

        Debug.Log($"Добавлено {amount} монет");
    }

    public bool CanSpend(int amount)
    {
        return _walletData.CanSpend(amount);
    }

    public bool TrySpendCoins(int amount)
    {
        if (!_walletData.TrySpendCoins(amount))
            return false;

        SaveBalance();
        BalanceChanged?.Invoke(_walletData.Coins);
        return true;
    }

    private void LoadBalance()
    {
        if (!_saveLoadService.HasSave())
        {
            int initialCoins = _walletBalanceConfig != null ? _walletBalanceConfig.InitialCoins : 0;
            _walletData.SetBalance(initialCoins);
            SaveBalance();
            return;
        }

        GameSaveData saveData = _saveLoadService.Load();
        _walletData.SetBalance(saveData.coins);
    }

    private void SaveBalance()
    {
        GameSaveData saveData = _saveLoadService.Load();
        saveData.coins = _walletData.Coins;
        _saveLoadService.Save(saveData);
    }
}
