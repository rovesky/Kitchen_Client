using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyCharAnimSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,
                ref CharacterInterpolatedState state,
                ref Character character,
                ref LocalToWorld localToWorld) =>
            {
                if(character.PresentationEntity == Entity.Null)
                    return;
                //显示动作
                var presentPos = EntityManager.GetComponentObject<Transform>(character.PresentationEntity);
             //   var oy = presentPos.position.y;
                var cPos = localToWorld.Position;
                cPos.y = cPos.y - 1.2f;
                presentPos.position = cPos;
                presentPos.rotation = state.Rotation;

                if(state.SqrMagnitude > 0)
                    FSLog.Info($"ApplyCharAnimSystem,entity:{entity},state.SqrMagnitude:{state.SqrMagnitude}");
                var anim = EntityManager.GetComponentObject<Animator>(character.PresentationEntity);
                anim.SetFloat("Blend", state.SqrMagnitude, state.SqrMagnitude > 0.1f ? 0.3f : 0.15f, Time.DeltaTime);

                var skinController =
                    EntityManager.GetComponentObject<CharacterSkinController>(character.PresentationEntity);
                if (skinController != null)
                    skinController.ChangeMaterialSettings(state.MaterialId);

                //anim.SetFloat("Blend", speed, 0.3f, Time.deltaTime);
                //   EntityManager.SetComponentData(character.PresentationEntity, presentPos);
                //  FSLog.Info($"ApplyCharPresentationSystem,x:{predictData.Position.x},z:{predictData.Position.z}," +
                //       $"translation.Value.x:{ translation.Value.x},translation.Value.z:{ translation.Value.z}");
            });
        }
    }

}