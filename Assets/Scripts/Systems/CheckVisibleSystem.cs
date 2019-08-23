using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public class CheckVisibleSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((CheckVisibleBehaviour behaviour) =>
            {
                if (behaviour.InVisible())
                {
                    Debug.Log("CheckVisibleSystem true");
                    EntityManager.SetComponentData(behaviour.entity,
                        new KillOutofRender() {IsVisible = false});
                }
            });
        }
    }
}
