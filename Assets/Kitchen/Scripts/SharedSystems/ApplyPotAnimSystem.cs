using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
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
                    in SlotPredictedState slotState,
                    in ProgressSetting cookedSetting,
                    in ProgressPredictState cookedState,
                    in BurntPredictedState burntState,
                    in LocalToWorld localToWorld) =>
                {
                    if (slotState.FilledIn == Entity.Null)
                    {
                        //空锅
                        if (EntityManager.HasComponent<RenderMesh>(presentation.Full))
                        {
                            fullMesh = EntityManager.GetSharedComponentData<RenderMesh>(presentation.Full);
                            EntityManager.RemoveComponent<RenderMesh>(presentation.Full);
                        }

                        if (!EntityManager.HasComponent<RenderMesh>(presentation.Empty))
                            EntityManager.AddSharedComponentData(presentation.Empty, emptyMesh);

                        if (presentation.Steam != null)
                            presentation.Steam.SetActive(false);

                    }
                    else
                    {
                        //满锅
                        if (EntityManager.HasComponent<RenderMesh>(presentation.Empty))
                        {
                            emptyMesh = EntityManager.GetSharedComponentData<RenderMesh>(presentation.Empty);
                            EntityManager.RemoveComponent<RenderMesh>(presentation.Empty);
                        }

                        if (!EntityManager.HasComponent<RenderMesh>(presentation.Full))
                            EntityManager.AddSharedComponentData(presentation.Full, fullMesh);


                        if (presentation.Steam == null)
                        {
                            presentation.Steam = Object.Instantiate(Resources.Load("Steam")) as GameObject;
                            presentation.Steam.SetActive(false);
                        }

                        if (cookedState.CurTick == cookedSetting.TotalTick)
                        {
                            presentation.Steam.SetActive(true);
                            presentation.Steam.transform.position = localToWorld.Position + new float3(0, 1.5f, 0);
                        }
                        //     FSLog.Info($"stream active true,pos:{presentation.Steam.transform.position}");

                    }

                    if (burntState.IsBurnt)
                    {
                        if (presentation.Steam != null)
                            presentation.Steam.SetActive(false);

                    }

                }).Run();
        }
    }
}