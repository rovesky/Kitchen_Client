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
                    if (characterPresentation.Object == null)
                        return;

                    characterPresentation.Object.SetActive(true);

                    //显示动作
                    var presentPos = characterPresentation.Object.transform;
                    //   var oy = presentPos.position.y;
                    var cPos = localToWorld.Position;
                    cPos.y = cPos.y - 1.5f;
                    presentPos.position = cPos;
                    presentPos.rotation = state.Rotation;
                    var anim = characterPresentation.Object.GetComponent<Animator>();

                    anim.SetFloat("Velocity", state.Velocity);
                    anim.SetBool("IsTake", state.IsTake);
                    anim.SetBool("IsSlice", state.IsSlice);
                    anim.SetBool("IsClean", state.IsClean);
                    anim.SetBool("IsThrow", state.IsThrow);

                    var knife1 = SearchChild.FindChildFast(characterPresentation.Object.transform,"Knife1");
                    if (knife1 != null)
                        knife1.gameObject.SetActive(state.IsSlice);
                    var knife2 = SearchChild.FindChildFast(characterPresentation.Object.transform,"Knife2");
                    if (knife2 != null)
                        knife2.gameObject.SetActive(state.IsSlice);

                }).Run();
        }
    }
}