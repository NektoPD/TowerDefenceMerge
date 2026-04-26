using UnityEngine;

namespace Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private HealthModal _health = new();
        [SerializeField, Min(1f)] private float _maxHp = 10f;

        public bool IsDead => _health.Current <= 0f;
        public event System.Action<EnemyHealth> Died;
        private bool _diedInvoked;

        public event System.Action<float, float> Changed
        {
            add => _health.Changed += value;
            remove => _health.Changed -= value;
        }

        private void OnEnable()
        {
            // Ensures pooled enemies always come back alive.
            _diedInvoked = false;
            if (_health.Current <= 0f || _health.Max != _maxHp)
                ResetToFull();
        }

        public void ResetToFull()
        {
            _health.SetMax(_maxHp, fillToMax: true);
        }

        public void TakeDamage(float amount)
        {
            _health.TakeDamage(amount);
            if (!_diedInvoked && IsDead)
            {
                _diedInvoked = true;
                Died?.Invoke(this);
            }
        }
    }
}

