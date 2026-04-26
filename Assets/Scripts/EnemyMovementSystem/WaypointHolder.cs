using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace EnemyMovementSystem
{
    public class WaypointHolder : MonoBehaviour
    {
        [SerializeField] private List<Waypoint> _waypoints;

        private List<WaypointMover> _waypointMovers = new List<WaypointMover>();

        public event System.Action<WaypointMover> MoverReachedEnd;

        public void SetNewMover(WaypointMover mover)
        {
            if (_waypointMovers.Contains(mover))
                return;

            _waypointMovers.Add(mover);
            mover.ReadyToMove += MoveOneMover;
            mover.MoverRemoved += OnMoverRemoved;
            mover.ActivateAndPlace(_waypoints[0].TargetPoint.position);
        }

        private void MoveOneMover(WaypointMover waypointMover)
        {
            if (waypointMover.MovedIndex >= _waypoints.Count)
            {
                MoverReachedEnd?.Invoke(waypointMover);
                waypointMover.OnReachedEnd();
                return;
            }
            
            waypointMover.MoveToNextPosition(_waypoints[waypointMover.MovedIndex].TargetPoint.position);
        }

        private void OnMoverRemoved(WaypointMover waypointMover)
        {
            if (!_waypointMovers.Contains(waypointMover))
                return;

            _waypointMovers.Remove(waypointMover);
            waypointMover.ReadyToMove -= MoveOneMover;
            waypointMover.MoverRemoved -= OnMoverRemoved;
        }
    }
}
