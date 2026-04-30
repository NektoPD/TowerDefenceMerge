using Enemy;
using UnityEngine;
using Zenject;

namespace Economy
{
    public class EnemyCoinReward : MonoBehaviour
    {
        [SerializeField, Min(0)] private int _coinsOnKill = 1;
        [SerializeField] private EnemyTarget _target;

        [Header("UI fly text")]
        [SerializeField] private Canvas _uiCanvas;
        [SerializeField] private CoinFlyText _flyTextInstance;

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
            if (_target == null) _target = GetComponent<EnemyTarget>();
        }

        private void OnEnable()
        {
            if (_target != null && _target.Health != null)
                _target.Health.Died += OnDied;
        }

        private void OnDisable()
        {
            if (_target != null && _target.Health != null)
                _target.Health.Died -= OnDied;
        }

        private void OnDied(EnemyHealth health)
        {
            if (_coinsOnKill <= 0) return;
            _wallet?.AddCoins(_coinsOnKill);

            var targetUI = _coinCounterTarget != null ? _coinCounterTarget.Target : null;
            if (_uiCanvas == null || _flyTextInstance == null || targetUI == null) return;

            var cam = _uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _uiCanvas.worldCamera;
            var screenPos = RectTransformUtility.WorldToScreenPoint(cam, transform.position);

            _flyTextInstance.transform.SetParent(_uiCanvas.transform, worldPositionStays: true);
            _flyTextInstance.gameObject.SetActive(true);
            _flyTextInstance.Play(_coinsOnKill, screenPos, targetUI);
        }
    }
}

