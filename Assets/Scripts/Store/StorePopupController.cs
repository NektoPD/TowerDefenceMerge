using Economy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Waves;
using Zenject;

namespace Store
{
    public class StorePopupController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private WaveController _waves;
        private Wallet _wallet;
        private ResourceInventory _resources;

        [Header("Popup")]
        [SerializeField] private RectTransform _popupRoot;
        [SerializeField] private bool _startHidden = true;

        [Header("Zone 1: Next wave preview")]
        [SerializeField] private TMP_Text _nextWaveText;

        [Header("Zone 2: Resources")]
        [SerializeField] private TMP_Text _woodCount;
        [SerializeField] private TMP_Text _metalCount;
        [SerializeField] private TMP_Text _crystalCount;
        [SerializeField] private TMP_Text _mysteryCount;

        [Header("Zone 3: Controls")]
        [SerializeField] private Button _startWaveButton;
        [SerializeField] private TMP_Text _coinsText;

        [Header("Zone 2: Purchase buttons (placeholder)")]
        [SerializeField] private Button _buyWood;
        [SerializeField] private Button _buyMetal;
        [SerializeField] private Button _buyCrystal;
        [SerializeField] private Button _buyMysteryX2;
        [SerializeField, Min(0)] private int _costWood = 5;
        [SerializeField, Min(0)] private int _costMetal = 5;
        [SerializeField, Min(0)] private int _costCrystal = 5;
        [SerializeField, Min(0)] private int _costMysteryX2 = 8;

        private System.Action<int> _coinsHandler;
        private System.Action<ResourceType, int> _resHandler;

        [Inject]
        public void Construct(Wallet wallet, ResourceInventory resources)
        {
            _wallet = wallet;
            _resources = resources;
        }

        private void Awake()
        {
            if (_startWaveButton != null)
                _startWaveButton.onClick.AddListener(OnStartWaveClicked);

            if (_buyWood != null) _buyWood.onClick.AddListener(() => Buy(ResourceType.Wood, 1, _costWood));
            if (_buyMetal != null) _buyMetal.onClick.AddListener(() => Buy(ResourceType.Metal, 1, _costMetal));
            if (_buyCrystal != null) _buyCrystal.onClick.AddListener(() => Buy(ResourceType.Crystal, 1, _costCrystal));
            if (_buyMysteryX2 != null) _buyMysteryX2.onClick.AddListener(BuyMysteryX2);

            if (_startHidden && _popupRoot != null)
                _popupRoot.gameObject.SetActive(false);

            RefreshAll();
        }

        private void OnEnable()
        {
            if (_waves != null) _waves.WaveCompleted += OnWaveCompleted;

            _coinsHandler ??= _ => RefreshCoins();
            _resHandler ??= (_, __) => RefreshResources();
            if (_wallet != null) _wallet.CoinsChanged += _coinsHandler;
            if (_resources != null) _resources.ResourceChanged += _resHandler;
            RefreshAll();
        }

        private void OnDisable()
        {
            if (_waves != null) _waves.WaveCompleted -= OnWaveCompleted;
            if (_wallet != null && _coinsHandler != null) _wallet.CoinsChanged -= _coinsHandler;
            if (_resources != null && _resHandler != null) _resources.ResourceChanged -= _resHandler;
        }

        private void OnWaveCompleted(int waveIndex)
        {
            Show();
            RefreshNextWavePreview();
        }

        private void OnStartWaveClicked()
        {
            Hide();
            if (_waves != null) _waves.Continue();
        }

        public void Show()
        {
            if (_popupRoot == null) return;
            _popupRoot.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_popupRoot == null) return;
            _popupRoot.gameObject.SetActive(false);
        }

        private void RefreshAll()
        {
            RefreshCoins();
            RefreshResources();
            RefreshNextWavePreview();
        }

        private void RefreshCoins()
        {
            if (_coinsText == null || _wallet == null) return;
            _coinsText.text = _wallet.Coins.ToString();
        }

        private void RefreshResources()
        {
            if (_resources == null) return;
            if (_woodCount != null) _woodCount.text = _resources.Get(ResourceType.Wood).ToString();
            if (_metalCount != null) _metalCount.text = _resources.Get(ResourceType.Metal).ToString();
            if (_crystalCount != null) _crystalCount.text = _resources.Get(ResourceType.Crystal).ToString();
            if (_mysteryCount != null) _mysteryCount.text = _resources.Get(ResourceType.Mystery).ToString();
        }

        private void RefreshNextWavePreview()
        {
            if (_nextWaveText == null || _waves == null) return;
            int next = _waves.CurrentWaveIndex + 1;
            if (next < 0) next = 0;
            if (_waves.TotalWaves == 0 || next >= _waves.TotalWaves)
            {
                _nextWaveText.text = "Next wave: -";
                return;
            }

            int count = _waves.Config != null ? _waves.Config.GetWaveEnemyCount(next) : 0;
            string label = count <= 6 ? "low" : count <= 16 ? "medium" : "many";
            _nextWaveText.text = $"Next wave: {label}";
        }

        private void Buy(ResourceType type, int amount, int cost)
        {
            if (_wallet == null || _resources == null) return;
            if (!_wallet.TrySpend(cost)) return;
            _resources.Add(type, amount);
        }

        private void BuyMysteryX2()
        {
            if (_wallet == null || _resources == null) return;
            if (!_wallet.TrySpend(_costMysteryX2)) return;

            var roll = Random.Range(0, 3); // unknown type
            var type = roll == 0 ? ResourceType.Wood : roll == 1 ? ResourceType.Metal : ResourceType.Crystal;
            _resources.Add(type, 2);
            _resources.Add(ResourceType.Mystery, 1);
        }
    }
}

