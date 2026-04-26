using System.Collections.Generic;
using UnityEngine;

namespace Towers
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private TowerStats _stats;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private Projectiles.ProjectilePool _projectilePool;

        private readonly List<Enemy.EnemyHealth> _targets = new();
        private float _cooldownTimer;
        private CircleCollider2D _rangeTrigger;

        private void Awake()
        {
            _rangeTrigger = GetComponent<CircleCollider2D>();
            if (_rangeTrigger == null) _rangeTrigger = gameObject.AddComponent<CircleCollider2D>();
            _rangeTrigger.isTrigger = true;

            ApplyStatsToTrigger();

            if (_shootPoint == null) _shootPoint = transform;
            _cooldownTimer = 0f;
        }

        private void OnValidate()
        {
            if (_shootPoint == null) _shootPoint = transform;
            ApplyStatsToTrigger();
        }

        private void Update()
        {
            if (_stats == null || _projectilePool == null) return;

            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer > 0f) return;

            var target = GetBestTarget();
            if (target == null) return;

            FireAt(target);
            _cooldownTimer = _stats.Cooldown;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & _enemyLayer.value) == 0) return;
            if (!other.TryGetComponent(out Enemy.EnemyHealth enemyHealth)) return;
            if (!_targets.Contains(enemyHealth)) _targets.Add(enemyHealth);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Enemy.EnemyHealth enemyHealth)) return;
            _targets.Remove(enemyHealth);
        }

        private Enemy.EnemyHealth GetBestTarget()
        {
            for (int i = _targets.Count - 1; i >= 0; i--)
            {
                if (_targets[i] == null || !_targets[i].isActiveAndEnabled || _targets[i].IsDead)
                    _targets.RemoveAt(i);
            }

            if (_targets.Count == 0) return null;

            Enemy.EnemyHealth best = null;
            float bestDist = float.PositiveInfinity;
            var from = (Vector2)transform.position;

            foreach (var t in _targets)
            {
                if (t == null) continue;
                float d = Vector2.SqrMagnitude((Vector2)t.transform.position - from);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = t;
                }
            }

            return best;
        }

        private void FireAt(Enemy.EnemyHealth target)
        {
            if (_stats.ProjectilePrefab == null) return;

            var projectile = _projectilePool.Get(_stats.ProjectilePrefab);
            projectile.transform.position = _shootPoint.position;
            projectile.gameObject.SetActive(true);
            projectile.Launch(
                target,
                _stats.Damage,
                _stats.ProjectileStats,
                _projectilePool
            );
        }

        private void ApplyStatsToTrigger()
        {
            if (_rangeTrigger == null) _rangeTrigger = GetComponent<CircleCollider2D>();
            if (_rangeTrigger == null || _stats == null) return;
            _rangeTrigger.radius = _stats.Range;
        }
    }
}