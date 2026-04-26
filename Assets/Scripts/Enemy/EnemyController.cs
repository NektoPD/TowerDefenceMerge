using System;
using EnemyMovementSystem;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private WaypointHolder _waypointHolder;
        [SerializeField] private int _enemiesPerWave;

        private void Start()
        {
            StartWave();
        }

        private void StartWave()
        {
            for (int i = 0; i < _enemiesPerWave; i++)
            {
                _waypointHolder.SetNewMover(_enemySpawner.GetFromPool());
            }
        }
    }
}