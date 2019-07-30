using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct SpawnEntity : IComponentData
    {
        public Entity entity;
        public float spawnIntervalMax;
        public float spawnIntervalMin;        

        public float spawnTimer;
    }

    public class SpawnEntityBehaviour : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject prefabs;
        public float spawnIntervalMin = 5;
        public float spawnIntervalMax = 15;
        public float spawnTimer = 5;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<SpawnEntity>(entity, new SpawnEntity()
            {
                entity = conversionSystem.GetPrimaryEntity(prefabs),
                spawnIntervalMin = spawnIntervalMin,
                spawnIntervalMax = spawnIntervalMax,
                spawnTimer = spawnTimer
            });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(prefabs);
        }
    }
   
}
