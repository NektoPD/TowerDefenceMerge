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
        [SerializeField] private TMP_Text _enemyTypesText;

        [Header("Zone 2: Resources")]
        [SerializeField] private TMP_Text _eyeCount;
        [SerializeField] private TMP_Text _bookCount;
        [SerializeField] private TMP_Text _voidCount;
        [SerializeField] private TMP_Text _mysteryCount;

        [Header("Zone 3: Controls")]
        [SerializeField] private Button _startWaveButton;
        [SerializeField] private TMP_Text _coinsText;

        [Header("Zone 2: Purchase buttons (placeholder)")]
        [SerializeField] private Button _buyEye;
        [SerializeField] private Button _buyBook;
        [SerializeField] private Button _buyVoid;
        [SerializeField] private Button _buyMysteryX2;
        [SerializeField, Min(0)] private int _costEye = 5;
        [SerializeField, Min(0)] private int _costBook = 5;
        [SerializeField, Min(0)] private int _costVoid = 5;
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
            if (_waves == null) _waves = FindFirstObjectByType<WaveController>();
            if (_popupRoot == null) _popupRoot = transform as RectTransform;

            if (_startWaveButton != null)
                _startWaveButton.onClick.AddListener(OnStartWaveClicked);

            if (_buyEye != null) _buyEye.onClick.AddListener(() => Buy(ResourceType.Eye, 1, _costEye));
            if (_buyBook != null) _buyBook.onClick.AddListener(() => Buy(ResourceType.Book, 1, _costBook));
            if (_buyVoid != null) _buyVoid.onClick.AddListener(() => Buy(ResourceType.Void, 1, _costVoid));
            if (_buyMysteryX2 != null) _buyMysteryX2.onClick.AddListener(BuyMysteryX2);

            if (_startHidden && _popupRoot != null)
                _popupRoot.gameObject.SetActive(false);

            RefreshAll();
        }

        private void OnEnable()
        {
            if (_waves == null) _waves = FindFirstObjectByType<WaveController>();
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

        private void Start()
        {
            // Show store/preview before wave 0 as well.
            if (_waves != null && _waves.CurrentWaveIndex < 0)
            {
                Show();
                RefreshNextWavePreview();
            }
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
            if (_eyeCount != null) _eyeCount.text = _resources.Get(ResourceType.Eye).ToString();
            if (_bookCount != null) _bookCount.text = _resources.Get(ResourceType.Book).ToString();
            if (_voidCount != null) _voidCount.text = _resources.Get(ResourceType.Void).ToString();
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
                if (_enemyTypesText != null) _enemyTypesText.text = "Enemies: -";
                return;
            }

            bool hasFast, hasTank, hasPhase;
            int count;
            _waves.Config.GetWavePreview(next, out hasFast, out hasTank, out hasPhase, out count);

            string qty = count <= 6 ? "Few" : "Many";
            _nextWaveText.text = $"Next wave: {qty}";

            if (_enemyTypesText != null)
            {
                string types = "";
                if (hasFast) types += "🏃 Fast ";
                if (hasTank) types += "🛡 Tank ";
                if (hasPhase) types += "👻 Phase ";
                if (string.IsNullOrWhiteSpace(types)) types = "-";
                _enemyTypesText.text = $"Enemies: {types.Trim()}";
            }
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
            var type = roll == 0 ? ResourceType.Eye : roll == 1 ? ResourceType.Book : ResourceType.Void;
            _resources.Add(type, 2);
            _resources.Add(ResourceType.Mystery, 1);
        }
    }
}

