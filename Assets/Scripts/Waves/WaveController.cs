using System.Collections;
using Enemy;
using EnemyMovementSystem;
using UnityEngine;

namespace Waves
{
    public class WaveController : MonoBehaviour
    {
        [SerializeField] private EnemySpawner _spawner;
        [SerializeField] private WaveConfig _config;
        [SerializeField] private bool _autoStart = true;
        [SerializeField] private WaypointHolder[] _lines;

        private int _aliveInWave;
        private bool _waveSpawningDone;

        private void Start()
        {
            if (_autoStart)
                StartCoroutine(RunAllWaves());
        }

        public void StartWaves()
        {
            StopAllCoroutines();
            StartCoroutine(RunAllWaves());
        }

        private IEnumerator RunAllWaves()
        {
            if (_spawner == null || _config == null || _config.waves == null) yield break;

            for (int waveIndex = 0; waveIndex < _config.waves.Count; waveIndex++)
            {
                var wave = _config.waves[waveIndex];
                _aliveInWave = 0;
                _waveSpawningDone = false;

                if (wave.startDelay > 0f)
                    yield return new WaitForSeconds(wave.startDelay);

                if (wave.groups != null)
                {
                    foreach (var group in wave.groups)
                    {
                        if (group == null || group.count <= 0) continue;
                        yield return StartCoroutine(SpawnGroup(group));
                    }
                }

                _waveSpawningDone = true;

                while (_aliveInWave > 0)
                    yield return null;

                if (_config.timeBetweenWaves > 0f && waveIndex < _config.waves.Count - 1)
                    yield return new WaitForSeconds(_config.timeBetweenWaves);
            }
        }

        private IEnumerator SpawnGroup(WaveConfig.SpawnGroup group)
        {
            if (_lines == null || _lines.Length == 0) yield break;
            if (group.lineIndex < 0 || group.lineIndex >= _lines.Length) yield break;
            var line = _lines[group.lineIndex];
            if (line == null) yield break;

            for (int i = 0; i < group.count; i++)
            {
                var mover = _spawner.GetFromPool();
                _aliveInWave++;
                mover.MoverRemoved += OnMoverRemoved;
                line.SetNewMover(mover);

                if (group.spawnInterval > 0f)
                    yield return new WaitForSeconds(group.spawnInterval);
                else
                    yield return null;
            }
        }

        private void OnMoverRemoved(WaypointMover mover)
        {
            mover.MoverRemoved -= OnMoverRemoved;
            _aliveInWave = Mathf.Max(0, _aliveInWave - 1);
        }
    }
}

