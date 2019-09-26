using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public enum EnemyType
    {
        Normal,
        Super
    }

    [Serializable]
    public struct SpawnEnemy : IComponentData
    {
     //   public Entity entity;

        public EnemyType enemyType;

        public float spawnIntervalMax;
        public float spawnIntervalMin;

        public float spawnTimer;
    }

}