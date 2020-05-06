using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyExtinguisherAnimSystem : SystemBase
    {
        private RenderMesh fullMesh;
        private RenderMesh emptyMesh;

        protected override void OnUpdate()
        {

            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in ExtinguisherPresentation presentation,
                    in ExtinguisherPredictedState extinguisherState,
                    in LocalToWorld localToWorld) =>
                {
                    if (extinguisherState.Distance == 0)
                    {
                        if (presentation.Smog != null)
                            presentation.Smog.SetActive(false);
                        return;
                    }

                    if (presentation.Smog == null)
                    {
                        presentation.Smog = Object.Instantiate(Resources.Load("Effect/Smog")) as GameObject;
                    }

                    presentation.Smog.SetActive(true);
                    presentation.Smog.transform.position =
                        localToWorld.Position + math.mul(localToWorld.Rotation, presentation.SmogPos);
                    presentation.Smog.transform.rotation = localToWorld.Rotation;

                }).Run();
        }
    }
}