using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class GridPlacement : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField, Min(0.1f)] private float _cellSize = 1f;
        [SerializeField] private Vector2 _origin = Vector2.zero;

        [Header("Placement")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private Camera _camera;

        private readonly Dictionary<Vector2Int, Towers.Tower> _occupied = new();

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;
        }

        public Vector2Int WorldToCell(Vector2 world)
        {
            var local = world - _origin;
            int x = Mathf.RoundToInt(local.x / _cellSize);
            int y = Mathf.RoundToInt(local.y / _cellSize);
            return new Vector2Int(x, y);
        }

        public Vector2 CellToWorld(Vector2Int cell)
        {
            return _origin + new Vector2(cell.x * _cellSize, cell.y * _cellSize);
        }

        public bool CanPlaceAt(Vector2Int cell) => !_occupied.ContainsKey(cell);

        public bool TryPlace(Towers.Tower towerPrefab, Vector2Int cell, out Towers.Tower placed)
        {
            placed = null;
            if (towerPrefab == null) return false;
            if (!CanPlaceAt(cell)) return false;

            var world = CellToWorld(cell);
            placed = Instantiate(towerPrefab, world, Quaternion.identity);
            _occupied[cell] = placed;
            return true;
        }

        public bool TryGetMouseCell(out Vector2Int cell)
        {
            cell = default;
            if (_camera == null) return false;

            var world = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition);

            // optional: require clicking on ground collider(s)
            if (_groundMask.value != 0)
            {
                var hit = Physics2D.OverlapPoint(world, _groundMask);
                if (hit == null) return false;
            }

            cell = WorldToCell(world);
            return true;
        }
    }
}

