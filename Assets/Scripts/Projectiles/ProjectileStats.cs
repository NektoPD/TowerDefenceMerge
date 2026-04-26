using UnityEngine;

namespace Projectiles
{
    [CreateAssetMenu(menuName = "Tower Defence/Projectile Stats", fileName = "ProjectileStats")]
    public class ProjectileStats : ScriptableObject
    {
        [field: SerializeField, Min(0.1f)] public float Speed { get; private set; } = 8f;
        [field: SerializeField, Min(0.05f)] public float Lifetime { get; private set; } = 3f;
        [field: SerializeField] public bool DestroyOnHit { get; private set; } = true;
        [field: SerializeField, Min(0f)] public float HitRadius { get; private set; } = 0.05f;
    }
}

