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

        public WaypointMover GetFromPool()
        {
            if (_waypointMoversInPool.Any())
            {
                var mover = _waypointMoversInPool.Dequeue();
                mover.gameObject.SetActive(true);
                mover.MoverRemoved += ReturnToPool;
                _activeMovers.Add(mover);
                return mover;
            }

            var newMover = Instantiate(_prefab, _parentPosition);
            newMover.gameObject.SetActive(true);
            _activeMovers.Add(newMover);
            return newMover;
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