using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct UpdateHealthUI : IComponentData { }

    [RequiresEntityConversion]
    public class UpdateHealthUIBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Transform LifeText;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(
            entity,
            new UpdateHealthUI());
        }
    }

}
