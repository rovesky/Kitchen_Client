using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    public class UpdateHealthUISystem : ComponentSystem
    {
      //  EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

        //protected override void OnCreate()
        //{
        //    m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        //}

        protected override void OnUpdate()
        {
         //   EntityCommandBuffer CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();

            Entities.WithAllReadOnly<UpdateHealthUI>().ForEach((Entity entity, ref Health health) =>
            {
                //    var str = string.Format("生命 {0}", life.lifeValue);
                //  Debug.Log("updateLife:" + str);
                GameManager.Instance.ChangeLife(health.Value);
                //  updateLife.LifeText.text = str;
            });
        }
    }
}
