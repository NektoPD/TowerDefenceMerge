using EnemyMovementSystem;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Returns pooled enemy (WaypointMover root) to pool when its health reaches 0.
    /// Put this on the same object as WaypointMover (enemy root).
    /// </summary>
    public class EnemyDeathToPool : MonoBehaviour
    {
        [SerializeField] private EnemyTarget _target;
        [SerializeField] private WaypointMover _mover;

        private void Awake()
        {
            if (_target == null) _target = GetComponent<EnemyTarget>();
            if (_mover == null) _mover = GetComponent<WaypointMover>();
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
            if (_mover != null)
                _mover.OnReachedEnd(); // triggers spawner return + holder cleanup
            else
                gameObject.SetActive(false);
        }
    }
}

