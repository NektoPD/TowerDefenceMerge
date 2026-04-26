using System;
using UnityEngine;

namespace Towers
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private float _radius;
        [SerializeField] private float _attackCooldown;

        private void OnTriggerEnter2D(Collider2D other)
        {
            
        }
    }
}