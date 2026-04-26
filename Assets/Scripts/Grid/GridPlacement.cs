using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GridSystem
{
    public class GridPlacement : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField, Min(0.1f)] private float _cellSize = 1f;
        [SerializeField] private Vector2 _origin = Vector2.zero;
        [SerializeField] private Vector2Int _gridSize = new Vector2Int(20, 12);
        [SerializeField] private bool _drawGridGizmos = true;
        [SerializeField] private Color _gridColor = new Color(1f, 1f, 1f, 0.25f);
        [SerializeField] private Color _occupiedColor = new Color(1f, 0.2f, 0.2f, 0.25f);

        [Header("Placement")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private Camera _camera;

        private DiContainer _container;

        private readonly Dictionary<Vector2Int, Towers.Tower> _occupied = new();

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

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

        public float CellSize => _cellSize;
        public Vector2 Origin => _origin;

        public bool CanPlaceAt(Vector2Int cell) => !_occupied.ContainsKey(cell);

        public bool TryPlace(Towers.Tower towerPrefab, Vector2Int cell, out Towers.Tower placed)
        {
            placed = null;
            if (towerPrefab == null) return false;
            if (!CanPlaceAt(cell)) return false;

            var world = CellToWorld(cell);
            placed = _container.InstantiatePrefabForComponent<Towers.Tower>(towerPrefab, world, Quaternion.identity, null);
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

        private void OnDrawGizmos()
        {
            if (!_drawGridGizmos) return;
            if (_cellSize <= 0f) return;

            Gizmos.color = _gridColor;

            for (int x = 0; x <= _gridSize.x; x++)
            {
                var from = (Vector3)(_origin + new Vector2(x * _cellSize, 0f));
                var to = (Vector3)(_origin + new Vector2(x * _cellSize, _gridSize.y * _cellSize));
                Gizmos.DrawLine(from, to);
            }

            for (int y = 0; y <= _gridSize.y; y++)
            {
                var from = (Vector3)(_origin + new Vector2(0f, y * _cellSize));
                var to = (Vector3)(_origin + new Vector2(_gridSize.x * _cellSize, y * _cellSize));
                Gizmos.DrawLine(from, to);
            }

            Gizmos.color = _occupiedColor;
            foreach (var kvp in _occupied)
            {
                var c = kvp.Key;
                var center = (Vector3)(CellToWorld(c));
                Gizmos.DrawCube(center, new Vector3(_cellSize * 0.95f, _cellSize * 0.95f, 0.01f));
            }
        }
    }
}

