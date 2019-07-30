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

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<SpawnEntity>(entity, new SpawnEntity()
            {
                entity = conversionSystem.GetPrimaryEntity(prefabs),
                spawnIntervalMin = spawnIntervalMin,
                spawnIntervalMax = spawnIntervalMax,
                spawnTimer = spawnIntervalMin
            });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(prefabs);
        }
    }

    public class SpawnEntitySystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.ForEach(
               (ref LocalToWorld gunTransform, ref Rotation gunRotation, ref SpawnEntity spawn) =>
               {
                   if (spawn.entity == null)
                       return;

                   spawn.spawnTimer -= Time.deltaTime;
                   if (spawn.spawnTimer > 0)
                       return;

                   spawn.spawnTimer = UnityEngine.Random.Range(spawn.spawnIntervalMin, spawn.spawnIntervalMax);

                   var e = PostUpdateCommands.Instantiate(spawn.entity);

                   Translation position = new Translation() { Value = gunTransform.Position };
                   Rotation rotation = new Rotation() { Value = Quaternion.identity };

                   PostUpdateCommands.SetComponent(e, position);
                   PostUpdateCommands.SetComponent(e, rotation);

               }
           );
        }
    }
}
