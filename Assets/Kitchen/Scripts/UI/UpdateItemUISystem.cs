using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateItemUISystem :SystemBase
    {

        private Dictionary<int, GameObject> sliders = new  Dictionary<int, GameObject>();
       // private GameObject sliderPrefab;

        protected override void OnCreate()
        {
           // sliderPrefab = Resources.Load("Progress") as GameObject;
        }

        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    ref ItemSliceState sliceState,
                    in ItemSliceSetting sliceSetting,
                    in LocalToWorld translation) =>
            {
                if (sliceState.CurSliceTick == 0)
                {
                    UpdateSlider(entity.Index,false, float3.zero, 0);
                }
                else
                {
                    var percentage = (float) sliceState.CurSliceTick / sliceSetting.TotalSliceTick;

                    // if(percentage > 0)
                    //  FSLog.Info($"UpdateItemUISystem,percentage:{percentage}");
                    //   var viewPos = Camera.main.WorldToViewportPoint(translation.Position); //得到视窗坐标
                    UpdateSlider(entity.Index, true, translation.Position + sliceSetting.OffPos, percentage);
                }

            }).Run();
          
        }

        private void UpdateSlider(int id,bool isVisible,Vector3 pos, float value)
        {
            if(!sliders.ContainsKey(id))
                sliders.Add(id, UIManager.Instance.CreateProgess());


            var sliceSlider = sliders[id];

            sliceSlider.SetActive(isVisible);

            var screenPos = Camera.main.WorldToScreenPoint(pos);
            var rectTransform = sliceSlider.GetComponent<RectTransform>();
            rectTransform.position = screenPos;

            var slider = sliceSlider.GetComponent<Slider>();
            slider.value = value;
        }
    }
}