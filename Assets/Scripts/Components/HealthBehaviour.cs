using System;
using System.Threading;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [Serializable]
    public struct Health : IComponentData
    {
        public int Value;
    }

    public class HealthBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public int Value;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Health() { Value = Value });
        }
    }   

}