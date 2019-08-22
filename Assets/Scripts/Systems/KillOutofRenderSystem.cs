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

            Entities.WithAllReadOnly<Player>().ForEach((Entity entity,KillOutofRenderBehaviour behaviour)  =>
            {
            //    var killOutofRend
             //   if (behaviour.)
              //  {
                 //   PostUpdateCommands.DestroyEntity(entity);
                    Debug.Log($"KillOutofRenderSystem  kill entity {entity.Index}");
              //  }
            });

            //var AnimatorEntities = AnimatorGroup.ToEntityArray(Allocator.Persistent);
            //if(AnimatorEntities.Length > 0)
            //{
            //    Debug.Log($"KillOutofRenderSystem  AnimatorEntities > 0");
            //}
            //  Debug.Log($"KillOutofRenderSystem OnUpdate");
            // Entities.WithAllReadOnly<Player>().ForEach((Entity entity) =>
            // {
            ////     var entity = gameObjectEntity.Entity;
            //     if (EntityManager.HasComponent<Transform>(entity))
            //     {
            //       Debug.Log($"KillOutofRenderSystem {entity.Index}");

            //         //var renderer = EntityManager.GetComponentObject<MeshRenderer>(entity);
            //         //if (!renderer.isVisible)
            //         //{
            //         //    Debug.Log($"KillOutofRenderSystem,KillOutofRender:{entity.Index}");
            //         //    PostUpdateCommands.DestroyEntity(entity);
            //         //}
            //     }
            // });

            //Entities.WithNone<Attack>().ForEach((Entity entity, Transform transform) =>
            //{
            //   // transform.position = FAR_AWAY;
            //    // var entity = this.GetPr

            //});

            // Entities.WithNone<Attack>().ForEach((Entity entity,MeshRenderer renderer) =>
            // {
            //     //  Debug.Log("renderer.gameObject.SetActive(false)");
            //     ////  renderer.gameObject.SetActive(false);
            //     //if (EntityManager.HasComponent<RenderMesh>(entity))
            //     //{
            //     //    //   renderer.enabled = false;
            //     //    PostUpdateCommands.RemoveComponent<RenderMesh>(entity);

            //     //    Debug.Log($"KillOutofRenderSystem {entity.Index}");
            //     //}
            //     // var entity = this.GetPr

            // });

            // Entities.WithAllReadOnly<Player>().ForEach((Animator renderer) =>
            // {
            //   //  var entity = GetPrimaryEntity(meshRenderer);

            //    // if (EntityManager.HasComponent<MeshRenderer> (entity))
            //    // {
            //  //       var renderer = EntityManager.GetComponentObject<MeshRenderer>(entity);
            //         Debug.Log($"EntityManager.HasComponent<MeshRenderer>({renderer})");
            //         if (renderer!= null)
            //         {
            //             Debug.Log(" EntityManager.DestroyEntity(entity)");
            //        //     PostUpdateCommands.DestroyEntity(entity);
            //         }
            ////     }

            // });
        }
    }
}
