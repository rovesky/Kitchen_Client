using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct TriggerDestroy : IComponentData {  }

    public class TriggerDestroyBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        void OnEnable() { }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (enabled)
            {      
                dstManager.AddComponentData(entity, new TriggerDestroy());
            }
        }
    }

}