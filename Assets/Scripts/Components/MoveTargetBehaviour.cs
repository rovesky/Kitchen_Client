using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [Serializable]
    public struct MoveTarget : IComponentData
    {
        public int Speed;      
    }

    public class MoveTargetBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public int Speed; 

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (enabled)
            {
                dstManager.AddComponentData(
                    entity,
                    new MoveTarget()
                    {
                        Speed = Speed,
                    });
            }
        }
    }


   

}