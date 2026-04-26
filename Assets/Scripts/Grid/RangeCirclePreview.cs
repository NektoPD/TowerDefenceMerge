using UnityEngine;

namespace GridSystem
{
    [RequireComponent(typeof(LineRenderer))]
    public class RangeCirclePreview : MonoBehaviour
    {
        [SerializeField, Min(3)] private int _segments = 48;
        [SerializeField, Min(0f)] private float _radius = 1f;

        private LineRenderer _lr;

        private void Awake()
        {
            _lr = GetComponent<LineRenderer>();
            _lr.useWorldSpace = false;
            _lr.loop = true;
            _lr.positionCount = _segments;
            Draw();
        }

        public void SetRadius(float radius)
        {
            _radius = Mathf.Max(0f, radius);
            Draw();
        }

        public void SetColor(Color c)
        {
            if (_lr == null) _lr = GetComponent<LineRenderer>();
            _lr.startColor = c;
            _lr.endColor = c;
        }

        private void Draw()
        {
            if (_lr == null) _lr = GetComponent<LineRenderer>();
            if (_segments < 3) _segments = 3;
            _lr.positionCount = _segments;

            float step = (Mathf.PI * 2f) / _segments;
            for (int i = 0; i < _segments; i++)
            {
                float a = step * i;
                _lr.SetPosition(i, new Vector3(Mathf.Cos(a) * _radius, Mathf.Sin(a) * _radius, 0f));
            }
        }
    }
}

