using System;

namespace Economy
{
    public class Wallet
    {
        private int _coins;
        public int Coins => _coins;

        public event Action<int> CoinsChanged;

        public Wallet(int startingCoins = 0)
        {
            _coins = Math.Max(0, startingCoins);
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0) return;
            _coins += amount;
            CoinsChanged?.Invoke(_coins);
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0) return true;
            if (_coins < amount) return false;
            _coins -= amount;
            CoinsChanged?.Invoke(_coins);
            return true;
        }
    }
}

