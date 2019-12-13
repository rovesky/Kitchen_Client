using Unity.Entities;
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
                ref Character character) =>
            {
                //显示动作
                var presentPos = EntityManager.GetComponentObject<Transform>(character.PresentationEntity);
                // var oy = presentPos.position.y;
                var cPos = state.Position;
                cPos.y = 0;
                presentPos.position = cPos;
                presentPos.rotation = state.Rotation;

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