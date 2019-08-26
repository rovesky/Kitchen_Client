using System.Collections.Generic;
using Unity.Entities;
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
            var entityPrefabs = conversionSystem.GetPrimaryEntity(prefabs);
            dstManager.AddComponentData<SpawnEnemy>(entity, new SpawnEnemy()
            {
          //      entity = entityPrefabs,
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
