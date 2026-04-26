using System;
using System.Collections.Generic;
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
        [SerializeField] private Transform _content;
        [SerializeField] private Button _buttonPrefab;
        [SerializeField] private List<TowerEntry> _towers = new();

        private readonly List<Button> _spawnedButtons = new();

        private void Awake()
        {
            Rebuild();
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
    }
}

