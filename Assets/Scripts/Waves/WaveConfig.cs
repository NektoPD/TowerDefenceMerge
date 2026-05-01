using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waves
{
    [CreateAssetMenu(menuName = "Tower Defence/Waves/Wave Config", fileName = "WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        [Serializable]
        public class SpawnGroup
        {
            public Enemy.EnemyStats enemy;
            [Min(0)] public int lineIndex = 0;
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

        public int GetWaveEnemyCount(int waveIndex)
        {
            if (waves == null) return 0;
            if (waveIndex < 0 || waveIndex >= waves.Count) return 0;
            var wave = waves[waveIndex];
            if (wave == null || wave.groups == null) return 0;

            int total = 0;
            foreach (var g in wave.groups)
            {
                if (g == null) continue;
                total += Mathf.Max(0, g.count);
            }
            return total;
        }

        public bool GetWavePreview(int waveIndex, out bool hasFast, out bool hasTank, out bool hasPhase, out int totalCount)
        {
            hasFast = hasTank = hasPhase = false;
            totalCount = 0;
            if (waves == null) return false;
            if (waveIndex < 0 || waveIndex >= waves.Count) return false;

            var wave = waves[waveIndex];
            if (wave == null || wave.groups == null) return false;

            foreach (var g in wave.groups)
            {
                if (g == null) continue;
                totalCount += Mathf.Max(0, g.count);

                if (g.enemy == null) continue;
                switch (g.enemy.Type)
                {
                    case Enemy.EnemyType.Fast: hasFast = true; break;
                    case Enemy.EnemyType.Tank: hasTank = true; break;
                    case Enemy.EnemyType.Phase: hasPhase = true; break;
                }
            }

            return true;
        }
    }
}

