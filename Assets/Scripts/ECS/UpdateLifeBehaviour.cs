using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ECS
{
    public struct UpdateLife : IComponentData
    {
        public Entity LifeText;
    }

    [RequiresEntityConversion]
    public class UpdateLifeBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Transform LifeText;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {

            Debug.Log("Convert:" + LifeText.GetComponent<Text>().text);
            dstManager.AddComponentData(
            entity,
            new UpdateLife()
            {
                LifeText = conversionSystem.GetPrimaryEntity(LifeText.GetComponent<Text>()),
            });
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

            Entities.ForEach((Entity entity, ref Life life,ref UpdateLife a) =>
            {
            //    var str = string.Format("生命 {0}", life.lifeValue);
              //  Debug.Log("updateLife:" + str);
                GameManager.Instance.ChangeLife(life.lifeValue);
              //  updateLife.LifeText.text = str;
            });
        }
    }

}
