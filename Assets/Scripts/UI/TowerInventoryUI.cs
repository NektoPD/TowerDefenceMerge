using System;
using System.Collections.Generic;
using DG.Tweening;
using GridSystem;
using Towers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TowerInventoryUI : MonoBehaviour
    {
        [Serializable]
        public class TowerEntry
        {
            public string displayName;
            public Tower towerPrefab;
            public Sprite icon;
        }

        [SerializeField] private TowerPlacer _towerPlacer;

        [Header("Layout")]
        [SerializeField] private RectTransform _panel; // the whole inventory panel
        [SerializeField] private Button _toggleButton; // small visible button
        [SerializeField] private bool _startHidden = true;
        [SerializeField, Min(0f)] private float _hiddenYOffset = 350f;

        [Header("Animation")]
        [SerializeField, Min(0f)] private float _animDuration = 0.35f;
        [SerializeField] private Ease _easeShow = Ease.OutCubic;
        [SerializeField] private Ease _easeHide = Ease.InCubic;

        [SerializeField] private Transform _content;
        [SerializeField] private Button _buttonPrefab;
        [SerializeField] private List<TowerEntry> _towers = new();

        private readonly List<Button> _spawnedButtons = new();
        private Tween _panelTween;
        private Vector2 _shownPos;
        private Vector2 _hiddenPos;
        private bool _isShown;

        private void Awake()
        {
            if (_panel == null) _panel = GetComponent<RectTransform>();
            if (_panel != null)
            {
                _shownPos = _panel.anchoredPosition;
                _hiddenPos = _shownPos + Vector2.down * _hiddenYOffset;
            }

            if (_toggleButton != null)
                _toggleButton.onClick.AddListener(Toggle);

            Rebuild();

            if (_startHidden)
                Hide(immediate: true);
            else
                Show(immediate: true);
        }

        private void OnDisable()
        {
            _panelTween?.Kill();
            _panelTween = null;
        }

        public void Rebuild()
        {
            if (_content == null || _buttonPrefab == null) return;

            foreach (var b in _spawnedButtons)
            {
                if (b != null) Destroy(b.gameObject);
            }
            _spawnedButtons.Clear();

            foreach (var entry in _towers)
            {
                if (entry == null || entry.towerPrefab == null) continue;

                var btn = Instantiate(_buttonPrefab, _content);
                _spawnedButtons.Add(btn);

                var label = btn.GetComponentInChildren<Text>();
                if (label != null)
                    label.text = string.IsNullOrEmpty(entry.displayName) ? entry.towerPrefab.name : entry.displayName;

                var img = btn.GetComponentInChildren<Image>();
                if (img != null && entry.icon != null)
                    img.sprite = entry.icon;

                btn.onClick.AddListener(() =>
                {
                    if (_towerPlacer != null)
                        _towerPlacer.BeginBuild(entry.towerPrefab);
                });
            }
        }

        public void Toggle()
        {
            if (_isShown) Hide(immediate: false);
            else Show(immediate: false);
        }

        public void Show(bool immediate)
        {
            if (_panel == null) return;
            _isShown = true;
            _panelTween?.Kill();

            if (immediate)
            {
                _panel.anchoredPosition = _shownPos;
                return;
            }

            _panelTween = _panel.DOAnchorPos(_shownPos, _animDuration).SetEase(_easeShow);
        }

        public void Hide(bool immediate)
        {
            if (_panel == null) return;
            _isShown = false;
            _panelTween?.Kill();

            if (immediate)
            {
                _panel.anchoredPosition = _hiddenPos;
                return;
            }

            _panelTween = _panel.DOAnchorPos(_hiddenPos, _animDuration).SetEase(_easeHide);
        }
    }
}

