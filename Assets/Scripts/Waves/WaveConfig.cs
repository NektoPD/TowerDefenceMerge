using System;
using System.Collections.Generic;
using EnemyMovementSystem;
using UnityEngine;

namespace Waves
{
    [CreateAssetMenu(menuName = "Tower Defence/Waves/Wave Config", fileName = "WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        [Serializable]
        public class SpawnGroup
        {
            public WaypointHolder line;
            [Min(0)] public int count = 10;
            [Min(0f)] public float spawnInterval = 0.4f;
        }

        [Serializable]
        public class Wave
        {
            [Min(0f)] public float startDelay = 0f;
            public List<SpawnGroup> groups = new();
        }

        [Min(0f)] public float timeBetweenWaves = 2f;
        public List<Wave> waves = new();
    }
}

