using UnityEngine;

namespace UI
{
    public class WorldFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 0.5f, 0f);
        [SerializeField] private bool _faceCamera = true;

        private Camera _cam;

        public void SetTarget(Transform target) => _target = target;

        private void Awake()
        {
            _cam = Camera.main;
        }

        private void LateUpdate()
        {
            if (_target == null) return;
            transform.position = _target.position + _offset;

            if (_faceCamera)
            {
                if (_cam == null) _cam = Camera.main;
                if (_cam != null) transform.forward = _cam.transform.forward;
            }
        }
    }
}

