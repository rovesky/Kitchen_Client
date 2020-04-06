using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateItemUISystem :SystemBase
    {

        protected override void OnUpdate()
        {
            Entities
                .ForEach((Entity entity,
                    ref ItemSliceState sliceState,
                    in ItemSliceSetting sliceSetting,
                    in LocalToWorld translation) =>
            {
                if (sliceState.CurSliceTick == 0)
                {
                    UIManager.Instance.UpdateSlider(false, float3.zero, 0);
                }
                else
                {
                    float percentage = (float)sliceState.CurSliceTick / sliceSetting.TotalSliceTick;

                   // if(percentage > 0)
                      //  FSLog.Info($"UpdateItemUISystem,percentage:{percentage}");
                 //   var viewPos = Camera.main.WorldToViewportPoint(translation.Position); //得到视窗坐标
                    UIManager.Instance.UpdateSlider(true,translation.Position + sliceSetting.OffPos, percentage);
                }

            }).Run();
        
          
        }
    }
}