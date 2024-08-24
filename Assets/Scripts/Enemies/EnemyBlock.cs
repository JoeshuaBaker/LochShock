using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBlock", menuName = "NemesisShock/EnemyBlock")]
public class EnemyBlock : ScriptableObject
{
    public enum EnemyType
    {
        Nothing,
        Urchin,
        Eye,
        Mine,
        Ring
    }

    [Serializable]
    public class EnemyBlockEntry
    {
        public EnemyType enemyType;
        [Range(0f, 1f)] public float spawnRate;

        public Type GetEnemyType()
        {
            switch (enemyType)
            {
                case EnemyType.Urchin:
                    return typeof(Urchin);

                case EnemyType.Eye:
                    return typeof(Eye);

                case EnemyType.Mine:
                    return typeof(Mine);

                case EnemyType.Ring:
                    return typeof(Ring);

                default:
                case EnemyType.Nothing:
                    return null;
            }
        }
    }

    public float duration = 30f;
    public List<EnemyBlockEntry> enemyBlocks;
}
