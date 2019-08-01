using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
  

    public class SpawnEnemyBehaviour : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject prefabs;
        public float spawnIntervalMin = 5;
        public float spawnIntervalMax = 15;
        public float spawnTimer = 5;
        public EnemyType enemyType = EnemyType.Normal;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<SpawnEnemy>(entity, new SpawnEnemy()
            {
                entity = conversionSystem.GetPrimaryEntity(prefabs),
                spawnIntervalMin = spawnIntervalMin,
                spawnIntervalMax = spawnIntervalMax,
                spawnTimer = spawnTimer,
                enemyType = enemyType
            });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(prefabs);
        }
    }
   
}
