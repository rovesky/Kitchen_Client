using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [RequiresEntityConversion]
    public class LifeTimeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float LifeTime;

        // The MonoBehaviour data is converted to ComponentData on the entity.
        // We are specifically transforming from a good editor representation of the data (Represented in degrees)
        // To a good runtime representation (Represented in radians)
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new LifeTime { Value = LifeTime };
            dstManager.AddComponentData(entity, data);
        }
    }
}
