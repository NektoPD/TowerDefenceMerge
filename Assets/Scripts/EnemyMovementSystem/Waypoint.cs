using UnityEngine;
using UnityEngine.Serialization;

namespace EnemyMovementSystem
{
    public class Waypoint : MonoBehaviour
    {
        [field: SerializeField] public Transform TargetPoint { get; private set; }
        
        public bool IsEmpty { get; private set; }

        public void SetOccupation(bool status)
        {
            IsEmpty = status;
        } 
    }
}