using System;
using System.Collections;
using UnityEngine;

namespace EnemyMovementSystem
{
    public class WaypointMover : MonoBehaviour
    {
        [SerializeField] private float _moveInterval;
        [SerializeField] private float _moveSpeed;
        
        public int MovedIndex { get; private set; }

        private Coroutine _movementCoroutine;
        
        public event Action<WaypointMover> ReadyToMove;
        public event Action<WaypointMover> MoverRemoved;

        public void ActivateAndPlace(Vector2 position)
        {
            StopCurrentMovement();

            MovedIndex = 0;
            MoveToNextPosition(position);
        }

        public void MoveToNextPosition(Vector2 position)
        {
            StopCurrentMovement();
            _movementCoroutine = StartCoroutine(MoveToNextPoint(position));
        }

        public void OnReachedEnd()
        {
            MoverRemoved?.Invoke(this);
        }

        private IEnumerator MoveToNextPoint(Vector2 targetPosition)
        {
            while (Vector2.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector2.Lerp(
                    transform.position,
                    targetPosition,
                    _moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            transform.position = targetPosition;

            _movementCoroutine = null;

            MovedIndex++;

            yield return new WaitForSeconds(_moveInterval);
            
            ReadyToMove?.Invoke(this);
        }

        private void StopCurrentMovement()
        {
            if (_movementCoroutine != null)
            {
                StopCoroutine(_movementCoroutine);
                _movementCoroutine = null;
            }
        }
    }
}