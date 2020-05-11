using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateCharPresentationClientSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref CharacterInterpolatedState interpolateData,
                    in TransformPredictedState transformPredictData,
                    in VelocityPredictedState velocityPredictData,
                    in TriggerPredictedState triggerPredictedData,
                    in SlicePredictedState slicePredictedState,
                    in WashPredictedState washPredictedState,
                    in ReplicatedEntityData replicatedEntityData) =>
                {
                     //   FSLog.Info($"velocityPredictData.SqrMagnitude:{velocityPredictData.SqrMagnitude}");

                    if (EntityManager.HasComponent<LocalCharacter>(entity))
                    {
                        interpolateData.Position = transformPredictData.Position;
                        interpolateData.Rotation = transformPredictData.Rotation;

                        var dir = Vector3.SqrMagnitude(velocityPredictData.Linear) < 0.001f
                            ? Vector3.zero
                            : (Vector3) math.normalize(velocityPredictData.Linear);
                        //   if(!dir.Equals(Vector3.zero))
                        // FSLog.Info($"UpdateCharPresentationSystem,entity:{entity},velocityPredictData:{velocityPredictData.Linear}");
                        interpolateData.SqrMagnitude = new Vector2(dir.x, dir.z).sqrMagnitude;

                        interpolateData.MaterialId = (byte) (replicatedEntityData.Id % 4);

                        interpolateData.ActionId =
                            (byte) (slicePredictedState.IsSlicing || washPredictedState.IsWashing ? 1 : 0);

                        //setup trigger entity 
                        if (triggerPredictedData.TriggeredEntity == Entity.Null)
                            return;

                        var triggerEntity = triggerPredictedData.TriggeredEntity;
                        var triggerState = EntityManager.GetComponentData<TriggeredState>(triggerEntity);
                        triggerState.IsTriggered = true;
                        EntityManager.SetComponentData(triggerEntity, triggerState);

                    }
                    else
                    {
                        if (velocityPredictData.SqrMagnitude > 0)
                        {
                            interpolateData.SqrMagnitude = velocityPredictData.SqrMagnitude;
                        }
                        else
                        {
                            var dir = Vector3.SqrMagnitude(velocityPredictData.Linear) < 0.001f
                                ? Vector3.zero
                                : (Vector3) math.normalize(velocityPredictData.Linear);
                            //   if(!dir.Equals(Vector3.zero))
                            // FSLog.Info($"UpdateCharPresentationSystem,entity:{entity},velocityPredictData:{velocityPredictData.Linear}");
                            interpolateData.SqrMagnitude = new Vector2(dir.x, dir.z).sqrMagnitude;
                        }
                    }

                
                }).Run();
        }
    }
}