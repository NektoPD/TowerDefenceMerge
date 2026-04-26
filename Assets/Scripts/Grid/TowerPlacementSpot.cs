using UnityEngine;
using Zenject;

namespace GridSystem
{
    public class TowerPlacementSpot : MonoBehaviour
    {
        [SerializeField] private bool _occupied;
        [SerializeField] private Color _gizmoColor = new Color(1f, 0.85f, 0.2f, 0.8f);
        [SerializeField] private float _gizmoRadius = 0.15f;

        [Header("Highlight (optional)")]
        [SerializeField] private SpriteRenderer _highlightRenderer;
        [SerializeField] private Color _idleColor = new Color(1f, 1f, 1f, 0.35f);
        [SerializeField] private Color _hoverCanPlaceColor = new Color(0.2f, 1f, 0.2f, 0.8f);
        [SerializeField] private Color _hoverCannotPlaceColor = new Color(1f, 0.2f, 0.2f, 0.8f);

        private Towers.Tower _placedTower;

        public bool IsOccupied => _occupied && _placedTower != null;

        public bool CanPlace => !IsOccupied;

        public Vector3 SnapPosition => transform.position;

        private void Awake()
        {
            if (_highlightRenderer == null) _highlightRenderer = GetComponentInChildren<SpriteRenderer>();
            ApplyColor(_idleColor);
        }

        public bool TryPlace(Towers.Tower towerPrefab, DiContainer container, out Towers.Tower placed)
        {
            placed = null;
            if (towerPrefab == null || container == null) return false;
            if (!CanPlace) return false;

            placed = container.InstantiatePrefabForComponent<Towers.Tower>(
                towerPrefab,
                SnapPosition,
                Quaternion.identity,
                null
            );

            _placedTower = placed;
            _occupied = true;
            ApplyColor(_hoverCannotPlaceColor);
            return true;
        }

        public void SetHovered(bool hovered)
        {
            if (!hovered)
            {
                ApplyColor(_idleColor);
                return;
            }

            ApplyColor(CanPlace ? _hoverCanPlaceColor : _hoverCannotPlaceColor);
        }

        private void ApplyColor(Color c)
        {
            if (_highlightRenderer == null) return;
            _highlightRenderer.color = c;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawWireSphere(transform.position, _gizmoRadius);
        }
    }
}

