using UnityEngine;

namespace Towers
{
    [CreateAssetMenu(menuName = "Tower Defence/Tower Stats", fileName = "TowerStats")]
    public class TowerStats : ScriptableObject
    {
        [field: SerializeField, Min(0.1f)] public float Range { get; private set; } = 2f;
        [field: SerializeField, Min(0.01f)] public float Cooldown { get; private set; } = 0.75f;
        [field: SerializeField, Min(0.1f)] public float Damage { get; private set; } = 1f;
        [field: SerializeField] public bool CanTargetPhaseEnemies { get; private set; } = true;
        
        [field: SerializeField] public Projectiles.Projectile ProjectilePrefab { get; private set; }
        [field: SerializeField] public Projectiles.ProjectileStats ProjectileStats { get; private set; }
    }
}

