using UnityEngine;

namespace GridSystem
{
    public class TowerPlacer : MonoBehaviour
    {
        [SerializeField] private GridPlacement _grid;
        [SerializeField] private Towers.Tower _towerPrefab;
        [SerializeField] private KeyCode _placeKey = KeyCode.Mouse0;

        [Header("Preview")]
        [SerializeField] private bool _showPreview = true;
        [SerializeField] private Color _canPlaceColor = new Color(0.2f, 1f, 0.2f, 0.65f);
        [SerializeField] private Color _cannotPlaceColor = new Color(1f, 0.2f, 0.2f, 0.65f);

        private GameObject _previewInstance;
        private SpriteRenderer[] _previewRenderers;
        private Vector2Int _lastCell;
        private bool _hasCell;

        private void OnDisable()
        {
            if (_previewInstance != null)
                Destroy(_previewInstance);
        }

        private void Update()
        {
            if (_grid == null || _towerPrefab == null) return;

            _hasCell = _grid.TryGetMouseCell(out var cell);
            if (_hasCell)
            {
                _lastCell = cell;
                UpdatePreview(cell);
            }
            else
            {
                SetPreviewActive(false);
            }

            if (!Input.GetKeyDown(_placeKey)) return;
            if (!_hasCell) return;

            if (_grid.CanPlaceAt(_lastCell))
            {
                _grid.TryPlace(_towerPrefab, _lastCell, out _);
            }
        }

        private void UpdatePreview(Vector2Int cell)
        {
            if (!_showPreview) return;

            if (_previewInstance == null)
                CreatePreview();

            if (_previewInstance == null) return;

            _previewInstance.transform.position = _grid.CellToWorld(cell);
            SetPreviewActive(true);

            bool canPlace = _grid.CanPlaceAt(cell);
            var col = canPlace ? _canPlaceColor : _cannotPlaceColor;
            if (_previewRenderers != null)
            {
                foreach (var r in _previewRenderers)
                {
                    if (r != null) r.color = col;
                }
            }
        }

        private void CreatePreview()
        {
            _previewInstance = Instantiate(_towerPrefab.gameObject);
            _previewInstance.name = $"{_towerPrefab.name}_PREVIEW";

            // disable all behaviour/colliders on preview
            foreach (var mb in _previewInstance.GetComponentsInChildren<MonoBehaviour>(true))
                mb.enabled = false;
            foreach (var col in _previewInstance.GetComponentsInChildren<Collider2D>(true))
                col.enabled = false;

            _previewRenderers = _previewInstance.GetComponentsInChildren<SpriteRenderer>(true);
        }

        private void SetPreviewActive(bool active)
        {
            if (_previewInstance != null && _previewInstance.activeSelf != active)
                _previewInstance.SetActive(active);
        }
    }
}

