using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AddCoinsButton : MonoBehaviour
{
    [SerializeField] private Button _addCoins;

    private WalletService _walletService;

    [Inject]
    private void Construct(WalletService walletService)
    {
        _walletService = walletService;
    }
    private void Start()
    {
        _addCoins.onClick.AddListener(AddCoinsPressed);
    }

    private void AddCoinsPressed()
    {
        _walletService.AddRandomCoins();
    }

    private void OnDestroy()
    {
        _addCoins.onClick.RemoveListener(AddCoinsPressed);

    }
}
