using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Towers
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private TowerStats _stats;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private Color _gizmoRangeColor = new Color(0.2f, 0.8f, 1f, 0.25f);

        private Projectiles.ProjectilePool _projectilePool;

        private readonly List<Enemy.EnemyTarget> _targets = new();
        private float _cooldownTimer;
        private CircleCollider2D _rangeTrigger;

        [Inject]
        public void Construct(Projectiles.ProjectilePool projectilePool)
        {
            _projectilePool = projectilePool;
        }

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
            Debug.Log($"[Tower] OnTriggerEnter2D: self={name} other={other.name} layer={LayerMask.LayerToName(other.gameObject.layer)}", this);

            if (((1 << other.gameObject.layer) & _enemyLayer.value) == 0)
            {
                Debug.Log($"[Tower] Ignored (layer not in enemy mask). enemyMask={_enemyLayer.value}", this);
                return;
            }

            var hitbox = other.GetComponent<Enemy.EnemyHitbox>();
            if (hitbox == null)
            {
                Debug.Log("[Tower] Ignored (missing EnemyHitbox on collider object).", this);
                return;
            }

            var target = hitbox.Target;
            if (target == null)
            {
                Debug.Log("[Tower] Ignored (EnemyHitbox.Target is null).", this);
                return;
            }

            if (target.Health == null)
            {
                Debug.Log("[Tower] Ignored (EnemyTarget.Health is null).", this);
                return;
            }

            if (_targets.Contains(target))
            {
                Debug.Log("[Tower] Ignored (target already tracked).", this);
                return;
            }

            _targets.Add(target);
            Debug.Log($"[Tower] Target added. trackedCount={_targets.Count} target={target.name}", this);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var hitbox = other.GetComponent<Enemy.EnemyHitbox>();
            if (hitbox == null) return;
            var target = hitbox.Target;
            if (target == null) return;
            _targets.Remove(target);
        }

        private Enemy.EnemyTarget GetBestTarget()
        {
            for (int i = _targets.Count - 1; i >= 0; i--)
            {
                if (_targets[i] == null || !_targets[i].isActiveAndEnabled || _targets[i].Health == null || _targets[i].Health.IsDead)
                    _targets.RemoveAt(i);
            }

            if (_targets.Count == 0) return null;

            Enemy.EnemyTarget best = null;
            float bestDist = float.PositiveInfinity;
            var from = (Vector2)transform.position;

            foreach (var t in _targets)
            {
                if (t == null) continue;
                float d = Vector2.SqrMagnitude((Vector2)t.AimPoint.position - from);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = t;
                }
            }

            return best;
        }

        private void FireAt(Enemy.EnemyTarget target)
        {
            Debug.Log($"[Tower] FireAt: self={name} target={(target != null ? target.name : "null")}", this);

            if (_stats == null)
            {
                Debug.Log("[Tower] FireAt aborted: _stats is null.", this);
                return;
            }

            if (_projectilePool == null)
            {
                Debug.Log("[Tower] FireAt aborted: _projectilePool is null (Zenject binding/injection missing?).", this);
                return;
            }

            if (target == null)
            {
                Debug.Log("[Tower] FireAt aborted: target is null.", this);
                return;
            }

            if (target.Health == null)
            {
                Debug.Log("[Tower] FireAt aborted: target.Health is null.", this);
                return;
            }

            if (_stats.ProjectilePrefab == null)
            {
                Debug.Log("[Tower] FireAt aborted: TowerStats.ProjectilePrefab is null.", this);
                return;
            }

            if (_shootPoint == null)
            {
                Debug.Log("[Tower] FireAt: _shootPoint is null, using tower transform.", this);
                _shootPoint = transform;
            }

            var projectile = _projectilePool.Get(_stats.ProjectilePrefab);
            if (projectile == null)
            {
                Debug.Log("[Tower] FireAt aborted: pool returned null projectile.", this);
                return;
            }

            Debug.Log($"[Tower] Spawned projectile={projectile.name} prefab={_stats.ProjectilePrefab.name} dmg={_stats.Damage} projStats={( _stats.ProjectileStats != null ? _stats.ProjectileStats.name : "null")}", this);
            projectile.transform.position = _shootPoint.position;
            projectile.gameObject.SetActive(true);
            Debug.Log($"[Tower] Launch projectile from={_shootPoint.position} toAim={target.AimPoint.position}", this);
            projectile.Launch(
                target.Health,
                _stats.Damage,
                _stats.ProjectileStats,
                _projectilePool
            );
            Debug.Log("[Tower] FireAt done.", this);
        }

        private void ApplyStatsToTrigger()
        {
            if (_rangeTrigger == null) _rangeTrigger = GetComponent<CircleCollider2D>();
            if (_rangeTrigger == null || _stats == null) return;
            _rangeTrigger.radius = _stats.Range;
        }

        private void OnDrawGizmosSelected()
        {
            if (_stats == null) return;
            Gizmos.color = _gizmoRangeColor;
            Gizmos.DrawWireSphere(transform.position, _stats.Range);
        }
    }
}