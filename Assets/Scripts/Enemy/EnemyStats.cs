using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Tower Defence/Enemy Stats", fileName = "EnemyStats")]
    public class EnemyStats : ScriptableObject
    {
        [field: SerializeField] public EnemyType Type { get; private set; } = EnemyType.Fast;
        [field: SerializeField, Min(1f)] public float MaxHp { get; private set; } = 10f;
        [field: SerializeField, Min(0.1f)] public float MoveSpeed { get; private set; } = 1f;
        [field: SerializeField, Min(0)] public int CoinsOnKill { get; private set; } = 1;
    }
}

