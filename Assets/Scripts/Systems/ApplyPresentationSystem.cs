using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class ApplyPresentationSystem : FSComponentSystem
    {
 

        protected override void OnCreate()
        {          
           
        }

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Player>().ForEach((Entity entity, ref PlayerPredictData predictData,
                ref Translation translation) =>
            {
                translation.Value = predictData.pos;
            });

        }
    }
}
