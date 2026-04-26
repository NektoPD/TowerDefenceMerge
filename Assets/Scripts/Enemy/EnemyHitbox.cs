using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Put this on the collider object that enters tower range.
    /// It forwards to a cached EnemyTarget on the root.
    /// </summary>
    public class EnemyHitbox : MonoBehaviour
    {
        [SerializeField] private EnemyTarget _target;

        public EnemyTarget Target => _target;

        private void Awake()
        {
            if (_target == null) _target = GetComponentInParent<EnemyTarget>();
        }
    }
}

