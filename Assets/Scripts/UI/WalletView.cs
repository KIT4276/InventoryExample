using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WalletView : MonoBehaviour
{
    [SerializeField] private string _prefix = "Монеты: ";
    [SerializeField] private TMP_Text _coins;
    [SerializeField] private Button _addCoins;
    [SerializeField] private int _debugAddCoinsAmount = 20;

    private WalletService _walletService;

    [Inject]
    private void Construct(WalletService walletService)
    {
        _walletService = walletService;
    }

    private void Start()
    {
        RefreshBalance(_walletService.Balance);
        _walletService.BalanceChanged += RefreshBalance;
        _addCoins.onClick.AddListener(AddCoinsPressed);
    }

    private void AddCoinsPressed()
    {
        _walletService.AddCoins(_debugAddCoinsAmount);
    }

    private void RefreshBalance(int balance)
    {
        _coins.text = _prefix + balance;
    }

    private void OnDestroy()
    {
        _walletService.BalanceChanged -= RefreshBalance;
        _addCoins.onClick.RemoveListener(AddCoinsPressed);
    }
}
