using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class DespawnSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,ref Despawn despawn) =>
            {
                if(despawn.Frame <=0)
                    EntityManager.DestroyEntity(entity);

                despawn.Frame--;
            });
        }
    }
}