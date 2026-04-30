using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Economy
{
    public class CoinFlyText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField, Min(0f)] private float _duration = 0.8f;
        [SerializeField] private Ease _ease = Ease.OutCubic;

        private RectTransform _rt;
        private CanvasGroup _cg;
        private Tween _tween;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            _cg = GetComponent<CanvasGroup>();
            if (_cg == null) _cg = gameObject.AddComponent<CanvasGroup>();
            if (_text == null) _text = GetComponentInChildren<TMP_Text>();
        }

        private void OnDisable()
        {
            _tween?.Kill();
            _tween = null;
        }

        public void Play(int coins, Vector2 startScreenPos, RectTransform targetUI)
        {
            if (_rt == null) _rt = GetComponent<RectTransform>();
            if (_cg == null) _cg = GetComponent<CanvasGroup>();
            if (_cg == null) _cg = gameObject.AddComponent<CanvasGroup>();

            if (_text != null)
                _text.text = $"+{coins}";

            _rt.position = startScreenPos;
            _cg.alpha = 1f;

            _tween?.Kill();

            var seq = DOTween.Sequence();
            seq.Join(_rt.DOMove(targetUI.position, _duration).SetEase(_ease));
            seq.Join(_cg.DOFade(0f, _duration).SetEase(Ease.InQuad));
            seq.OnComplete(() => gameObject.SetActive(false));
            _tween = seq;
        }
    }
}

