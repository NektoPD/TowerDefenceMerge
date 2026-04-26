using UnityEngine;

namespace GridSystem
{
    public class TowerPlacer : MonoBehaviour
    {
        [SerializeField] private GridPlacement _grid;
        [SerializeField] private Towers.Tower _towerPrefab;
        [SerializeField] private KeyCode _placeKey = KeyCode.Mouse0;

        private void Awake()
        {
            if (_grid == null) _grid = FindFirstObjectByType<GridPlacement>();
        }

        private void Update()
        {
            if (_grid == null || _towerPrefab == null) return;
            if (!Input.GetKeyDown(_placeKey)) return;

            if (_grid.TryGetMouseCell(out var cell))
            {
                _grid.TryPlace(_towerPrefab, cell, out _);
            }
        }
    }
}

