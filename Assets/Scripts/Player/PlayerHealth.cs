using UnityEngine;
using UnityEngine.UI;
using EnemyMovementSystem;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private HealthModal _health = new();

        [Header("End-of-path damage")]
        [SerializeField] private WaypointHolder[] _lines;
        [SerializeField] private float _damagePerEnemy = 1f;

        [Header("UI (optional)")]
        [SerializeField] private Slider _hpSlider;

        private void OnEnable()
        {
            if (_lines != null)
            {
                foreach (var line in _lines)
                {
                    if (line != null) line.MoverReachedEnd += OnEnemyReachedEnd;
                }
            }

            _health.Changed += OnHealthChanged;
            OnHealthChanged(_health.Current, _health.Max);
        }

        private void OnDisable()
        {
            if (_lines != null)
            {
                foreach (var line in _lines)
                {
                    if (line != null) line.MoverReachedEnd -= OnEnemyReachedEnd;
                }
            }

            _health.Changed -= OnHealthChanged;
        }

        private void OnEnemyReachedEnd(WaypointMover mover)
        {
            _health.TakeDamage(_damagePerEnemy);
        }

        private void OnHealthChanged(float current, float max)
        {
            if (_hpSlider == null) return;
            _hpSlider.maxValue = max;
            _hpSlider.value = current;
        }
    }
}

