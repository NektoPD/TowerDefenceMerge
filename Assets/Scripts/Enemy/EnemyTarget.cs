using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Cached references for towers/projectiles to avoid repeated component lookups.
    /// Put this on the enemy root (same object as movement/root).
    /// </summary>
    public class EnemyTarget : MonoBehaviour
    {
        [SerializeField] private EnemyHealth _health;
        [SerializeField] private Transform _aimPoint;
        [SerializeField] private EnemyStats _stats;

        public EnemyHealth Health => _health;
        public Transform AimPoint => _aimPoint != null ? _aimPoint : transform;
        public EnemyStats Stats => _stats;
        public EnemyType Type => _stats != null ? _stats.Type : EnemyType.Fast;
        public bool IsPhase => Type == EnemyType.Phase;

        private void Awake()
        {
            if (_health == null) _health = GetComponentInChildren<EnemyHealth>(true);
            if (_aimPoint == null && _health != null) _aimPoint = _health.transform;
        }

        public void ApplyStats(EnemyStats stats)
        {
            _stats = stats;
        }
    }
}

