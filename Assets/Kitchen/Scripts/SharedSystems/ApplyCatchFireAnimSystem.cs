using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyCatchFireAnimSystem: SystemBase
    {
     
        protected override void OnUpdate()
        {

            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                   in CatchFirePresentation presentation,
                   in CatchFirePredictedState state,
                   in SlotSetting slotSetting,
                   in LocalToWorld localToWorld) =>
               {

                   if (state.IsCatchFire)
                   {
                       if (presentation.Value == null)
                           presentation.Value = Object.Instantiate(Resources.Load("CatchFire")) as GameObject;

                       presentation.Value.SetActive(true);
                       presentation.Value.transform.position =
                           localToWorld.Position + math.mul(localToWorld.Rotation, slotSetting.Pos);
                   }
                   else
                   {
                       if (presentation.Value != null)

                           presentation.Value.SetActive(false);
                   }

               }).Run();
        }
    }
}