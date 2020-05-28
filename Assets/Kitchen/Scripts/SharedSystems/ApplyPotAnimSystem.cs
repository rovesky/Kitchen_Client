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
                    in ProgressSetting cookedSetting,
                    in ProgressPredictState cookedState,
                    in PotPredictedState potState,
                    in LocalToWorld localToWorld) =>
                {
                    //空锅
                    if (potState.State == PotState.Empty)
                    {
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
                    //满锅
                    else if (potState.State == PotState.Full)
                    {
                        if (EntityManager.HasComponent<RenderMesh>(presentation.Empty))
                        {
                            emptyMesh = EntityManager.GetSharedComponentData<RenderMesh>(presentation.Empty);
                            EntityManager.RemoveComponent<RenderMesh>(presentation.Empty);
                        }

                        if (!EntityManager.HasComponent<RenderMesh>(presentation.Full))
                            EntityManager.AddSharedComponentData(presentation.Full, fullMesh);
                    }
                    //饭已煮好的锅
                    else if (potState.State == PotState.Cooked)
                    {
                        if (EntityManager.HasComponent<RenderMesh>(presentation.Empty))
                        {
                            emptyMesh = EntityManager.GetSharedComponentData<RenderMesh>(presentation.Empty);
                            EntityManager.RemoveComponent<RenderMesh>(presentation.Empty);
                        }

                        if (!EntityManager.HasComponent<RenderMesh>(presentation.Full))
                            EntityManager.AddSharedComponentData(presentation.Full, fullMesh);


                        if (presentation.Steam == null)
                            presentation.Steam = Object.Instantiate(Resources.Load("Effect/Steam")) as GameObject;
                       
                        presentation.Steam.transform.position = localToWorld.Position + new float3(0, 1.5f, 0);
                        presentation.Steam.SetActive(true);
                    }
                    //被烧糊的锅
                    else if (potState.State == PotState.Burnt)
                    {
                        if (presentation.Steam != null)
                            presentation.Steam.SetActive(false);

                    }

                }).Run();
        }
    }
}