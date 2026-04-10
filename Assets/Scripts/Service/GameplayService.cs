public class GameplayService
{
    private readonly WalletService _walletService;

    public GameplayService(WalletService walletService)
    {
        _walletService = walletService;
    }

    public void RewardCoinsForAction(int rewardAmount)
    {
        _walletService.AddCoins(rewardAmount);
    }

    public bool TrySpendCoinsForAction(int cost)
    {
        return _walletService.TrySpendCoins(cost);
    }
}
