//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Rendering;
//using UnityEngine;

//namespace Assets.Scripts.ECS
//{
//    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
//    public class KillOutofRenderSystem : ComponentSystem
//    {

//        protected override void OnUpdate()
//        {
//            Entities.ForEach((Entity entity,ref KillOutofRender killOutofRender)  =>
//            {
//                if(!EntityManager.HasComponent<Despawn>(entity))
//                    EntityManager.AddComponentData(entity, new Despawn() { Frame = 0 });
  
//            });
        
//        }
//    }
//}
