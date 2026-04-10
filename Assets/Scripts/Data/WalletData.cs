public class WalletData
{
    public int Coins { get; private set; }

    public void SetBalance(int amount)
    {
        Coins = amount < 0 ? 0 : amount;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        Coins += amount;
    }

    public bool CanSpend(int amount)
    {
        return amount > 0 && Coins >= amount;
    }

    public bool TrySpendCoins(int amount)
    {
        if (!CanSpend(amount))
            return false;

        Coins -= amount;
        return true;
    }
}
