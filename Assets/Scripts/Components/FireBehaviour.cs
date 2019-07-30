using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct FireRocket : IComponentData
    {
        public Entity rocket;    
        public float  minRocketTimer;
        public float rocketTimer;
    }
    
    public class FireBehaviour : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        public GameObject bullet;
        public float minRocketTimer;

        // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
        public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
        {
            gameObjects.Add(bullet);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {     
            dstManager.AddComponentData(
                entity,
                new FireRocket()
                {
                    rocket = conversionSystem.GetPrimaryEntity(bullet), 
                    minRocketTimer = minRocketTimer,
                    rocketTimer = 0,
                });
        }
    }   
}