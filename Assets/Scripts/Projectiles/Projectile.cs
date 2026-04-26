using UnityEngine;

namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private ProjectileStats _stats;

        private Projectile _prefabKey;
        private Enemy.EnemyHealth _target;
        private float _damage;
        private float _lifeLeft;
        private ProjectilePool _pool;

        public ProjectileStats Stats => _stats;
        public Projectile PrefabKey => _prefabKey;

        public void SetPrefabKey(Projectile prefabKey)
        {
            _prefabKey = prefabKey;
        }

        public void Launch(Enemy.EnemyHealth target, float damage, ProjectileStats overrideStats, ProjectilePool pool)
        {
            _target = target;
            _damage = damage;
            _stats = overrideStats != null ? overrideStats : _stats;
            _pool = pool;
            _lifeLeft = _stats != null ? _stats.Lifetime : 2f;
        }

        private void Update()
        {
            _lifeLeft -= Time.deltaTime;
            if (_lifeLeft <= 0f)
            {
                ReturnToPool();
                return;
            }

            if (_target == null || !_target.isActiveAndEnabled || _target.IsDead)
            {
                ReturnToPool();
                return;
            }

            var current = (Vector2)transform.position;
            var targetPos = (Vector2)_target.transform.position;

            float speed = _stats != null ? _stats.Speed : 8f;
            var next = Vector2.MoveTowards(current, targetPos, speed * Time.deltaTime);
            transform.position = next;

            float hitRadius = _stats != null ? _stats.HitRadius : 0.05f;
            if (Vector2.SqrMagnitude(next - targetPos) <= hitRadius * hitRadius)
            {
                _target.TakeDamage(_damage);
                if (_stats == null || _stats.DestroyOnHit)
                    ReturnToPool();
            }
        }

        private void OnDisable()
        {
            _target = null;
            _pool = null;
            _damage = 0f;
            _lifeLeft = 0f;
        }

        private void ReturnToPool()
        {
            if (_pool != null)
                _pool.Return(this);
            else
                gameObject.SetActive(false);
        }
    }
}

