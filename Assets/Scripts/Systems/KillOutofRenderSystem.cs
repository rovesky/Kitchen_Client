using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public class KillOutofRenderSystem : ComponentSystem
    {
        //public EntityQuery AnimatorGroup;

        //protected override void OnCreate()
        //{
        //    AnimatorGroup = GetEntityQuery(new EntityQueryDesc
        //    {
        //        All = new ComponentType[]
        //        {
        //           typeof(Animator),
        //        }
        //    });
        //}


        protected override void OnUpdate()
        {
            //var AnimatorEntities = AnimatorGroup.ToEntityArray(Allocator.Persistent);
            //if(AnimatorEntities.Length > 0)
            //{
            //    Debug.Log($"KillOutofRenderSystem  AnimatorEntities > 0");
            //}
            //  Debug.Log($"KillOutofRenderSystem OnUpdate");
            Entities.WithAllReadOnly<KillOutofRender>().ForEach((Entity entity) =>
            {
                if (EntityManager.HasComponent<MeshRenderer>(entity))
                {
                  // Debug.Log($"KillOutofRenderSystem {entity.Index}");

                    var renderer = EntityManager.GetComponentObject<MeshRenderer>(entity);
                    if (!renderer.isVisible)
                    {
                        Debug.Log($"KillOutofRenderSystem,KillOutofRender:{entity.Index}");
                        PostUpdateCommands.DestroyEntity(entity);
                    }
                }
            });
        }
    }
}
