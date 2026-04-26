using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private int _prewarmPerPrefab = 16;
        [SerializeField] private Transform _parent;

        private readonly Dictionary<Projectile, Queue<Projectile>> _pool = new();

        private void Awake()
        {
            if (_parent == null) _parent = transform;
        }

        public Projectile Get(Projectile prefab)
        {
            if (prefab == null) return null;

            EnsurePool(prefab);

            var q = _pool[prefab];
            Projectile p = q.Count > 0 ? q.Dequeue() : Instantiate(prefab, _parent);
            p.SetPrefabKey(prefab);
            p.transform.SetParent(null, true);
            return p;
        }

        public void Return(Projectile projectile)
        {
            if (projectile == null) return;

            var prefab = projectile.PrefabKey;
            if (prefab == null || !_pool.ContainsKey(prefab))
            {
                projectile.gameObject.SetActive(false);
                projectile.transform.SetParent(_parent, true);
                return;
            }

            projectile.gameObject.SetActive(false);
            projectile.transform.SetParent(_parent, true);
            _pool[prefab].Enqueue(projectile);
        }

        private void EnsurePool(Projectile prefab)
        {
            if (_pool.ContainsKey(prefab)) return;

            _pool[prefab] = new Queue<Projectile>(_prewarmPerPrefab);

            for (int i = 0; i < _prewarmPerPrefab; i++)
            {
                var p = Instantiate(prefab, _parent);
                p.SetPrefabKey(prefab);
                p.gameObject.SetActive(false);
                _pool[prefab].Enqueue(p);
            }
        }
    }
}

