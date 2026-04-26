using Enemy;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private EnemyHealth _enemyHealth;
        [SerializeField] private Slider _slider;

        private void Awake()
        {
            if (_slider == null) _slider = GetComponentInChildren<Slider>();
            ResolveHealth();
        }

        private void OnEnable()
        {
            ResolveHealth();
            if (_enemyHealth == null) return;

            _enemyHealth.Changed += OnHealthChanged;
            OnHealthChanged(_enemyHealth.CurrentHp, _enemyHealth.MaxHp);
        }

        private void OnDisable()
        {
            if (_enemyHealth != null)
                _enemyHealth.Changed -= OnHealthChanged;
        }

        private void ResolveHealth()
        {
            if (_enemyHealth != null) return;

            // Preferred: bind through EnemyTarget on root (health may live in child)
            var target = GetComponentInParent<EnemyTarget>();
            if (target != null && target.Health != null)
            {
                _enemyHealth = target.Health;
                return;
            }

            // Fallbacks:
            _enemyHealth = GetComponentInParent<EnemyHealth>();
            if (_enemyHealth == null) _enemyHealth = GetComponentInChildren<EnemyHealth>(true);
        }

        private void OnHealthChanged(float current, float max)
        {
            if (_slider == null) return;
            _slider.maxValue = max;
            _slider.value = current;
        }
    }
}

