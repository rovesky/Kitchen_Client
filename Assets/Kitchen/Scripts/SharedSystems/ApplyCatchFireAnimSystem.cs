using Unity.Entities;
using Unity.Mathematics;
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
                       if (presentation.Object == null)
                           presentation.Object = Object.Instantiate(Resources.Load("CatchFire")) as GameObject;

                       presentation.Object.SetActive(true);
                       presentation.Object.transform.position =
                           localToWorld.Position + math.mul(localToWorld.Rotation, slotSetting.Pos);

                       var pos = presentation.Object.transform.position;
                       pos.y = 1.02f;
                       presentation.Object.transform.position = pos;
                    //   presentation.Object.transform.rotation = localToWorld.Rotation;
                   }
                   else
                   {
                       if (presentation.Object != null)

                           presentation.Object.SetActive(false);
                   }

               }).Run();
        }
    }
}