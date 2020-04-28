using FootStone.ECS;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyPotAnimSystem : SystemBase
    {
        private RenderMesh fullMesh;
        private RenderMesh emptyMesh;
        protected override void OnUpdate()
        {

            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in PotPresentation presentation,
                    in SlotPredictedState slotState) =>
                {
                    if (slotState.FilledIn == Entity.Null)
                    {
                        if (EntityManager.HasComponent<RenderMesh>(presentation.Full))
                        {
                            fullMesh = EntityManager.GetSharedComponentData<RenderMesh>(presentation.Full);
                            EntityManager.RemoveComponent<RenderMesh>(presentation.Full);
                        }


                        if (!EntityManager.HasComponent<RenderMesh>(presentation.Empty))
                            EntityManager.AddSharedComponentData(presentation.Full, emptyMesh);

                    }
                    else
                    {
                        if (EntityManager.HasComponent<RenderMesh>(presentation.Empty))
                        {
                            emptyMesh = EntityManager.GetSharedComponentData<RenderMesh>(presentation.Empty);
                            EntityManager.RemoveComponent<RenderMesh>(presentation.Empty);
                        }

                        if (!EntityManager.HasComponent<RenderMesh>(presentation.Full))
                            EntityManager.AddSharedComponentData(presentation.Full, fullMesh);


                    }

                }).Run();
            
        }
    }
}