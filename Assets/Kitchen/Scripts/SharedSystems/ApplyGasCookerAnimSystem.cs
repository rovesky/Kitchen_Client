using FootStone.ECS;
using Unity.Entities;
using Unity.Rendering;
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
                    in FirePresentation presentation,
                    in SlotPredictedState slotState) =>
                {

                    var visible = false;
                    if (slotState.FilledIn != Entity.Null &&
                        EntityManager.HasComponent<Pot>(slotState.FilledIn) &&
                        EntityManager.HasComponent<SlotPredictedState>(slotState.FilledIn))
                    {

                        var potSlot = EntityManager.GetComponentData<SlotPredictedState>(slotState.FilledIn);
                        if (potSlot.FilledIn != Entity.Null)
                            visible = true;
                    }

                    presentation.Value.SetActive(visible);

                }).Run();

        }
    }
}