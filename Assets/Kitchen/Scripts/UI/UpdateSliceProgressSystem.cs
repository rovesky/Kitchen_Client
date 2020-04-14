using System.CodeDom;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateSliceProgressSystem :SystemBase
    {

        private Dictionary<Entity, GameObject> sliders = new  Dictionary<Entity, GameObject>();
  
        protected override void OnCreate()
        {
          
        }

        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    ref FoodSlicedState sliceState,
                    in FoodSlicedSetting sliceSetting,
                    in LocalToWorld translation) =>
            {
                if (sliceState.CurSliceTick == 0 || sliceState.CurSliceTick == sliceSetting.TotalSliceTick)
                {
                    UpdateSlider(entity,false, float3.zero, 0);
                }
                else
                {
                    var percentage = (float) sliceState.CurSliceTick / sliceSetting.TotalSliceTick;

                    // if(percentage > 0)
                    //  FSLog.Info($"UpdateItemUISystem,percentage:{percentage}");
                    //   var viewPos = Camera.main.WorldToViewportPoint(translation.Position); //得到视窗坐标
                    UpdateSlider(entity, true, translation.Position + sliceSetting.OffPos, percentage);
                }

            }).Run();

            var removes = new List<Entity>();
            foreach (var entity in sliders.Keys)
            {
                if(!EntityManager.Exists(entity))
                {
                    removes.Add(entity);
                }
            }

            foreach (var entity in removes)
            {
                var slider = sliders[entity];
                Object.Destroy(slider);
                sliders.Remove(entity);
            }
          
        }

        private void UpdateSlider(Entity entity,bool isVisible,Vector3 pos, float value)
        {
            if(!sliders.ContainsKey(entity))
                sliders.Add(entity, UIManager.Instance.CreateProgess());


            var sliceSlider = sliders[entity];

            sliceSlider.SetActive(isVisible);

            var screenPos = Camera.main.WorldToScreenPoint(pos);
            var rectTransform = sliceSlider.GetComponent<RectTransform>();
            rectTransform.position = screenPos;

            var slider = sliceSlider.GetComponent<Slider>();
            slider.value = value;
        }
    }
}