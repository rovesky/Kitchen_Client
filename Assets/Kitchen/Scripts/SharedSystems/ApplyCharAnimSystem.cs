using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyCharAnimSystem : SystemBase
    {
        private void SetAction(UnityEngine.Animator anim, byte ActionId)
        {
            switch (ActionId)
            {
                case 0:
                    anim.SetTrigger("normal");
                    break;
                case 1:
                    anim.SetTrigger("angry");
                    break;
                case 2:
                    anim.SetTrigger("happy");
                    break;
                case 3:
                    anim.SetTrigger("dead");
                    break;
                default:
                    anim.SetTrigger("normal");
                    break;
            }

        }

        protected override void OnUpdate()
        {

            Entities.WithStructuralChanges()
                .WithNone<NewClientEntity>()
                .ForEach((Entity entity,
                    in CharacterInterpolatedState state,
                    in CharacterPresentation characterPresentation,
                    in LocalToWorld localToWorld) =>
                {
                    if (characterPresentation.Object == null)
                        return;
                    
                    characterPresentation.Object.SetActive(true);

                    //显示动作
                    var presentPos = characterPresentation.Object.transform;
                    //   var oy = presentPos.position.y;
                    var cPos = localToWorld.Position;
                    cPos.y = cPos.y - 1.4f;
                    presentPos.position = cPos;
                    presentPos.rotation = state.Rotation;

                    //  if(state.SqrMagnitude > 0)
                    //   FSLog.Info($"ApplyCharAnimSystem,entity:{entity},state.SqrMagnitude:{state.SqrMagnitude}");
                    var anim = characterPresentation.Object.GetComponent<Animator>();
                    anim.SetFloat("Blend", state.SqrMagnitude, state.SqrMagnitude > 0.1f ? 0.3f : 0.15f,
                        Time.DeltaTime);

                    if (state.SqrMagnitude < 0.1f)
                        SetAction(anim, state.ActionId);

                    var skinController = characterPresentation.Object.GetComponent<CharacterSkinController>();
                    //    EntityManager.GetComponentObject<CharacterSkinController>(character.PresentationEntity);
                    if (skinController != null)
                        skinController.ChangeMaterialSettings(state.MaterialId);

                    //anim.SetFloat("Blend", speed, 0.3f, Time.deltaTime);
                    //   EntityManager.SetComponentData(character.PresentationEntity, presentPos);
                    //  FSLog.Info($"ApplyCharPresentationSystem,x:{predictData.Position.x},z:{predictData.Position.z}," +
                    //       $"translation.Value.x:{ translation.Value.x},translation.Value.z:{ translation.Value.z}");
                }).Run();
        }
    }

}