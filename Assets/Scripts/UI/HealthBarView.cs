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
            if (_enemyHealth == null) _enemyHealth = GetComponentInParent<EnemyHealth>();
        }

        private void OnEnable()
        {
            if (_enemyHealth != null)
            {
                _enemyHealth.Changed += OnHealthChanged;
                _enemyHealth.ResetToFull(); // ensures slider init on spawn if needed
            }
        }

        private void OnDisable()
        {
            if (_enemyHealth != null)
                _enemyHealth.Changed -= OnHealthChanged;
        }

        private void OnHealthChanged(float current, float max)
        {
            if (_slider == null) return;
            _slider.maxValue = max;
            _slider.value = current;
        }
    }
}

