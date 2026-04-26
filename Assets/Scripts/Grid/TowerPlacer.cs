using UnityEngine;
using Zenject;

namespace GridSystem
{
    public class TowerPlacer : MonoBehaviour
    {
        [SerializeField] private Towers.Tower _towerPrefab;
        [SerializeField] private KeyCode _placeKey = KeyCode.Mouse0;

        [Header("Spots")]
        [SerializeField] private Camera _camera;
        [SerializeField, Min(0.01f)] private float _snapRadius = 0.6f;

        [Header("Build mode")]
        [SerializeField] private bool _buildMode = true;
        [SerializeField] private KeyCode _toggleBuildModeKey = KeyCode.B;

        [Header("Preview")]
        [SerializeField] private bool _showPreview = true;
        [SerializeField] private Color _canPlaceColor = new Color(0.2f, 1f, 0.2f, 0.65f);
        [SerializeField] private Color _cannotPlaceColor = new Color(1f, 0.2f, 0.2f, 0.65f);

        private DiContainer _container;
        private GameObject _previewInstance;
        private SpriteRenderer[] _previewRenderers;
        private TowerPlacementSpot _currentSpot;
        private TowerPlacementSpot _previousSpot;
        private TowerPlacementSpot[] _spots;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;
            RefreshSpots();
        }

        public void RefreshSpots()
        {
            _spots = FindObjectsByType<TowerPlacementSpot>(FindObjectsSortMode.None);
        }

        private void OnDisable()
        {
            if (_previewInstance != null)
                Destroy(_previewInstance);

            if (_previousSpot != null)
                _previousSpot.SetHovered(false);
            _previousSpot = null;
        }

        private void Update()
        {
            if (_towerPrefab == null || _container == null) return;
            if (_camera == null) return;

            if (Input.GetKeyDown(_toggleBuildModeKey))
            {
                _buildMode = !_buildMode;
                if (!_buildMode)
                {
                    SetPreviewActive(false);
                    if (_previousSpot != null) _previousSpot.SetHovered(false);
                    _previousSpot = null;
                }
            }

            if (!_buildMode)
                return;

            UpdateCurrentSpotAndHighlight();

            if (!Input.GetKeyDown(_placeKey)) return;
            if (_currentSpot == null) return;
            if (!_currentSpot.CanPlace) return;

            _currentSpot.TryPlace(_towerPrefab, _container, out _);
        }

        private void UpdateCurrentSpotAndHighlight()
        {
            var mouseWorld = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition);
            _currentSpot = FindNearestSpot(mouseWorld);

            if (_currentSpot == null)
            {
                SetPreviewActive(false);
                if (_previousSpot != null)
                    _previousSpot.SetHovered(false);
                _previousSpot = null;
                return;
            }

            if (_previousSpot != _currentSpot)
            {
                if (_previousSpot != null) _previousSpot.SetHovered(false);
                _previousSpot = _currentSpot;
            }

            _currentSpot.SetHovered(true);
            UpdatePreview(_currentSpot);
        }

        private TowerPlacementSpot FindNearestSpot(Vector2 world)
        {
            if (_spots == null || _spots.Length == 0) RefreshSpots();
            if (_spots == null || _spots.Length == 0) return null;

            TowerPlacementSpot best = null;
            float bestDist = _snapRadius * _snapRadius;

            foreach (var s in _spots)
            {
                if (s == null) continue;
                var d = (world - (Vector2)s.SnapPosition);
                float sq = d.sqrMagnitude;
                if (sq <= bestDist)
                {
                    bestDist = sq;
                    best = s;
                }
            }

            return best;
        }

        private void UpdatePreview(TowerPlacementSpot spot)
        {
            if (!_showPreview) return;

            if (_previewInstance == null)
                CreatePreview();

            if (_previewInstance == null) return;

            _previewInstance.transform.position = spot.SnapPosition;
            SetPreviewActive(true);

            var col = spot.CanPlace ? _canPlaceColor : _cannotPlaceColor;
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

        private void OnDrawGizmosSelected()
        {
            if (_camera == null) return;
            Gizmos.color = new Color(0.3f, 0.9f, 1f, 0.2f);
            var mouseWorld = _camera.ScreenToWorldPoint(Input.mousePosition);
            Gizmos.DrawWireSphere(mouseWorld, _snapRadius);
        }
    }
}

