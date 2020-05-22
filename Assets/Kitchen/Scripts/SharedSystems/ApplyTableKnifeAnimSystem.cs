using Unity.Entities;
using Unity.Rendering;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyTableKnifeAnimSystem : SystemBase
    {
        private RenderMesh knifeMesh;

        protected override void OnUpdate()
        {

            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in TableSlice tableSlice,
                    in SlotPredictedState slotState) =>
                {
                    if (slotState.FilledIn != Entity.Null)
                    {
                        if (EntityManager.HasComponent<RenderMesh>(tableSlice.Knife))
                        {
                            knifeMesh = EntityManager.GetSharedComponentData<RenderMesh>(tableSlice.Knife);
                            EntityManager.RemoveComponent<RenderMesh>(tableSlice.Knife);
                        }
                    }
                    else
                    {
                        if (!EntityManager.HasComponent<RenderMesh>(tableSlice.Knife))
                            EntityManager.AddSharedComponentData(tableSlice.Knife, knifeMesh);
                    }

                }).Run();
        }
    }
}