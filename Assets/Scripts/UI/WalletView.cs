using TMPro;
using UnityEngine;
using Zenject;

public class WalletView : MonoBehaviour
{
    [SerializeField] private string _prefix = "\u041c\u043e\u043d\u0435\u0442\u044b: ";
    [SerializeField] private TMP_Text _coins;

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
    }

    private void RefreshBalance(int balance)
    {
        _coins.text = _prefix + balance;
    }

    private void OnDestroy()
    {
        _walletService.BalanceChanged -= RefreshBalance;
    }
}
