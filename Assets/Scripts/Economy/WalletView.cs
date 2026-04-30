using TMPro;
using UnityEngine;
using Zenject;

namespace Economy
{
    public class WalletView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        private Wallet _wallet;
        private CoinCounterTarget _coinCounterTarget;

        [Inject]
        public void Construct(Wallet wallet, CoinCounterTarget coinCounterTarget)
        {
            _wallet = wallet;
            _coinCounterTarget = coinCounterTarget;
        }

        private void Awake()
        {
            if (_text == null) _text = GetComponentInChildren<TMP_Text>();
            Refresh();

            var rt = transform as RectTransform;
            if (rt != null)
                _coinCounterTarget?.Set(rt);
        }

        private void OnEnable()
        {
            if (_wallet != null) _wallet.CoinsChanged += OnCoinsChanged;
            Refresh();
        }

        private void OnDisable()
        {
            if (_wallet != null) _wallet.CoinsChanged -= OnCoinsChanged;
        }

        private void OnCoinsChanged(int coins) => Refresh();

        private void Refresh()
        {
            if (_wallet == null || _text == null) return;
            _text.text = _wallet.Coins.ToString();
        }
    }
}

