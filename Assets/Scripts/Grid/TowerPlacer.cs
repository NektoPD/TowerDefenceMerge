using UnityEngine;
using Zenject;

namespace GridSystem
{
    public class TowerPlacer : MonoBehaviour
    {
        [SerializeField] private Towers.Tower _towerPrefab;
        [SerializeField] private KeyCode _placeKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode _cancelKey = KeyCode.Mouse1;

        [Header("Spots")]
        [SerializeField] private Camera _camera;
        [SerializeField, Min(0.01f)] private float _snapRadius = 0.6f;
        [SerializeField] private TowerPlacementSpot[] _spots;

        [Header("Build mode")]
        [SerializeField] private bool _buildMode = false;

        [Header("Preview")]
        [SerializeField] private bool _showPreview = true;
        [SerializeField] private Color _canPlaceColor = new Color(0.2f, 1f, 0.2f, 0.65f);
        [SerializeField] private Color _cannotPlaceColor = new Color(1f, 0.2f, 0.2f, 0.65f);
        [SerializeField, Range(0f, 1f)] private float _followCursorAlpha = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _canBuildAlpha = 0.8f;

        private DiContainer _container;
        private GameObject _previewInstance;
        private SpriteRenderer[] _previewRenderers;
        private RangeCirclePreview _rangePreview;
        private TowerPlacementSpot _currentSpot;
        private TowerPlacementSpot _previousSpot;
        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;
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
            if (_container == null) return;
            if (_camera == null) return;

            if (_buildMode && Input.GetKeyDown(_cancelKey))
            {
                CancelBuild();
                return;
            }

            if (!_buildMode)
                return;

            if (_towerPrefab == null)
            {
                SetPreviewActive(false);
                return;
            }

            UpdateCurrentSpotAndHighlight();

            if (!Input.GetKeyDown(_placeKey)) return;
            if (_currentSpot == null) return;
            if (!_currentSpot.CanPlace) return;

            _currentSpot.TryPlace(_towerPrefab, _container, out _);
        }

        public void BeginBuild(Towers.Tower towerPrefab)
        {
            _towerPrefab = towerPrefab;
            _buildMode = _towerPrefab != null;
        }

        public void CancelBuild()
        {
            _buildMode = false;
            _towerPrefab = null;
            SetPreviewActive(false);
            if (_previousSpot != null) _previousSpot.SetHovered(false);
            _previousSpot = null;
            _currentSpot = null;
        }

        private void UpdateCurrentSpotAndHighlight()
        {
            var mouseWorld = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition);
            _currentSpot = FindNearestSpot(mouseWorld);

            if (_currentSpot == null)
            {
                if (_previousSpot != null)
                    _previousSpot.SetHovered(false);
                _previousSpot = null;
                UpdatePreviewAt(mouseWorld, canPlace: false, snapped: false);
                return;
            }

            if (_previousSpot != _currentSpot)
            {
                if (_previousSpot != null) _previousSpot.SetHovered(false);
                _previousSpot = _currentSpot;
            }

            _currentSpot.SetHovered(true);
            UpdatePreviewAt(_currentSpot.SnapPosition, _currentSpot.CanPlace, snapped: true);
        }

        private TowerPlacementSpot FindNearestSpot(Vector2 world)
        {
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

        private void UpdatePreviewAt(Vector3 worldPos, bool canPlace, bool snapped)
        {
            if (!_showPreview) return;

            if (_previewInstance == null)
                CreatePreview();

            if (_previewInstance == null) return;

            _previewInstance.transform.position = worldPos;
            SetPreviewActive(true);

            var baseCol = canPlace ? _canPlaceColor : _cannotPlaceColor;
            var alpha = canPlace ? _canBuildAlpha : _followCursorAlpha;
            var col = new Color(baseCol.r, baseCol.g, baseCol.b, alpha);
            if (_previewRenderers != null)
            {
                foreach (var r in _previewRenderers)
                {
                    if (r != null) r.color = col;
                }
            }

            if (_rangePreview != null)
                _rangePreview.SetColor(col);
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

            var tower = _previewInstance.GetComponent<Towers.Tower>();
            if (tower != null)
            {
                var circleObj = new GameObject("RangePreview");
                circleObj.transform.SetParent(_previewInstance.transform, false);
                circleObj.transform.localPosition = Vector3.zero;

                var lr = circleObj.AddComponent<LineRenderer>();
                lr.widthMultiplier = 0.03f;
                lr.numCapVertices = 2;
                lr.numCornerVertices = 2;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.sortingOrder = 9999;

                _rangePreview = circleObj.AddComponent<RangeCirclePreview>();
                _rangePreview.SetRadius(tower.Range);
            }
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

