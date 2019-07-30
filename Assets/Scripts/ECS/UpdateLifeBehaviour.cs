using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct UpdateLife : IComponentData { }

    [RequiresEntityConversion]
    public class UpdateLifeBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Transform LifeText;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(
            entity,
            new UpdateLife());
        }
    }

    public class UpdateLifeSystem : ComponentSystem
    {
        EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

        protected override void OnCreate()
        {
            m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();

            Entities.WithAllReadOnly<UpdateLife>().ForEach((Entity entity, ref Life life) =>
            {
            //    var str = string.Format("生命 {0}", life.lifeValue);
              //  Debug.Log("updateLife:" + str);
                GameManager.Instance.ChangeLife(life.lifeValue);
              //  updateLife.LifeText.text = str;
            });
        }
    }

}
