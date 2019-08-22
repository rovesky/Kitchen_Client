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

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Enemy>().ForEach((Entity entity,ref KillOutofRender killOutofRender)  =>
            {
                if (!killOutofRender.IsRenderEnable)
                {
                    PostUpdateCommands.DestroyEntity(entity);
                }
            });
        
        }
    }
}
