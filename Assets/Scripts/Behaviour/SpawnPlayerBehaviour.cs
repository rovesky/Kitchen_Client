using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{  

    public class SpawnPlayerBehaviour : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject prefabs;
    
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {

            //var entities = conversionSystem.GetEntities(prefabs);
            //foreach(var e in entities)
            //{
            //    Debug.Log($"SpawnPlayerBehaviour,prefabs {e.Index}");
            //}

            dstManager.AddComponentData(entity, new SpawnPlayer()
            {
                entity = conversionSystem.GetPrimaryEntity(prefabs),
                isSpawned = false,
            });

            

            //if (dstManager.HasComponent<Transform>(entity))
            //{
            //    Debug.Log("SpawnPlayerBehaviour has Transform");
            //}
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(prefabs);
        }
    }
   
}
