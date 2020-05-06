using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyGasCookerAnimSystem : SystemBase
    {
        private RenderMesh mesh;

        protected override void OnUpdate()
        {

            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in CookFirePresentation presentation,
                    in SlotPredictedState slotState,
                    in SlotSetting slotSetting,
                    in LocalToWorld localToWorld ) =>
                {

                    var visible = false;
                    if (slotState.FilledIn != Entity.Null &&
                        EntityManager.HasComponent<Pot>(slotState.FilledIn) &&
                        EntityManager.HasComponent<SlotPredictedState>(slotState.FilledIn))
                    {

                        var potSlot = EntityManager.GetComponentData<SlotPredictedState>(slotState.FilledIn);
                        if (potSlot.FilledIn != Entity.Null)
                        {
                            if (presentation.Object == null)
                                presentation.Object = Object.Instantiate(Resources.Load("Effect/CookFire")) as GameObject;
                            
                            presentation.Object.transform.position =
                                localToWorld.Position + math.mul(localToWorld.Rotation, slotSetting.Pos);
                            presentation.Object.transform.rotation = localToWorld.Rotation;
                            visible = true;

                        }
                          
                    }

                    if (presentation.Object != null)
                        presentation.Object.SetActive(visible);

                }).Run();

        }
    }
}