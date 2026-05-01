using System;
using System.Collections.Generic;
using System.Linq;
using EnemyMovementSystem;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private WaypointMover _prefab;
        [SerializeField] private int _poolCapacity;
        [SerializeField] private Transform _parentPosition;

        private Queue<WaypointMover> _waypointMoversInPool = new();
        private List<WaypointMover> _activeMovers = new();

        private void Awake()
        {
            for (int i = 0; i < _poolCapacity; i++)
            {
                WaypointMover waypointMover = Instantiate(_prefab, _parentPosition);
                waypointMover.gameObject.SetActive(false);
                _waypointMoversInPool.Enqueue(waypointMover);
            }
        }

        public WaypointMover GetFromPool(EnemyStats stats = null)
        {
            if (_waypointMoversInPool.Any())
            {
                var mover = _waypointMoversInPool.Dequeue();
                mover.gameObject.SetActive(true);
                ApplyStats(mover, stats);
                mover.MoverRemoved += ReturnToPool;
                _activeMovers.Add(mover);
                return mover;
            }

            var newMover = Instantiate(_prefab, _parentPosition);
            newMover.gameObject.SetActive(true);
            ApplyStats(newMover, stats);
            newMover.MoverRemoved += ReturnToPool;
            _activeMovers.Add(newMover);
            return newMover;
        }

        private void ApplyStats(WaypointMover mover, EnemyStats stats)
        {
            if (mover == null) return;

            if (stats != null)
            {
                mover.SetMoveSpeed(stats.MoveSpeed);

                if (mover.TryGetComponent(out EnemyTarget target))
                    target.ApplyStats(stats);

                var health = mover.GetComponentInChildren<EnemyHealth>(true);
                if (health != null)
                    health.SetMaxHp(stats.MaxHp, fillToMax: true);
            }
            else
            {
                var health = mover.GetComponentInChildren<EnemyHealth>(true);
                if (health != null)
                    health.ResetToFull();
            }
        }

        public void ReturnToPool(WaypointMover waypointMover)
        {
            if (!_activeMovers.Contains(waypointMover)) return;
            
            _activeMovers.Remove(waypointMover);
            waypointMover.MoverRemoved -= ReturnToPool;
            waypointMover.gameObject.SetActive(false);
            
            _waypointMoversInPool.Enqueue(waypointMover);
        }
    }
}