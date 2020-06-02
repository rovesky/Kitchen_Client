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
        protected override void OnUpdate()
        {
            Entities.WithStructuralChanges()
                .WithNone<NewServerEntity>()
                .ForEach((Entity entity,
                    in CharacterInterpolatedState state,
                    in CharacterPresentation characterPresentation,
                    in LocalToWorld localToWorld) =>
                {

                    if (localToWorld.Position.Equals(float3.zero))
                        return;
                    if (characterPresentation.CharacterObject == null)
                        return;

                    characterPresentation.CharacterObject.SetActive(true);

                    //显示动作
                    var presentPos = characterPresentation.CharacterObject.transform;
                    //   var oy = presentPos.position.y;
                    var cPos = localToWorld.Position;
                    cPos.y = cPos.y - 1.5f;
                    presentPos.position = cPos;
                    presentPos.rotation = state.Rotation;
                    var anim = characterPresentation.CharacterObject.GetComponent<Animator>();

                    anim.SetFloat("Velocity", state.Velocity);
                    anim.SetBool("IsTake", state.IsTake);
                    anim.SetBool("IsSlice", state.IsSlice);
                    anim.SetBool("IsClean", state.IsClean);
                    anim.SetBool("IsThrow", state.IsThrow);


                    //显示菜刀
                    if (characterPresentation.KnifeObject1 == null)
                        characterPresentation.KnifeObject1 = SearchChild
                            .FindChild(characterPresentation.CharacterObject.transform, "Knife1").gameObject;
                    characterPresentation.KnifeObject1.SetActive(state.IsSlice);

                    if (characterPresentation.KnifeObject2 == null)
                        characterPresentation.KnifeObject2 = SearchChild
                            .FindChild(characterPresentation.CharacterObject.transform, "Knife2").gameObject;
                    characterPresentation.KnifeObject2.SetActive(state.IsSlice);

                    var effectPos = presentPos.position;
                    effectPos.y = 0.39f;

                    //走路的时候冒烟
                    if (characterPresentation.FootSmoke == null)
                        characterPresentation.FootSmoke =
                            Object.Instantiate(Resources.Load("Effect/FootSmoke") as GameObject);
                    characterPresentation.FootSmoke.transform.position =
                        effectPos; //+ ((Vector3)math.mul(presentPos.rotation,Vector3.back)).normalized;
                    characterPresentation.FootSmoke.SetActive(state.Velocity > 0);

                    //主角的脚下光圈
                    if (!HasComponent<LocalCharacter>(entity))
                        return;

                    if (characterPresentation.ChooseEffect == null)
                        characterPresentation.ChooseEffect =
                            Object.Instantiate(Resources.Load("Effect/ChooseEffect") as GameObject);
                    characterPresentation.ChooseEffect.transform.position = effectPos;
            
                }).Run();
        }
    }
}