using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateProgressSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    ref ProgressPredictState sliceState,
                    in ProgressSetting sliceSetting,
                    in LocalToWorld translation,
                    in UIObject uiObject) =>
                {
                    if (sliceState.CurTick == 0
                        || sliceState.CurTick == sliceSetting.TotalTick
                        || EntityManager.HasComponent<Despawn>(entity))
                    {
                        UpdateProgress(uiObject, false, float3.zero, 0);
                    }
                    else
                    {
                        var percentage = (float) sliceState.CurTick / sliceSetting.TotalTick;
                        UpdateProgress(uiObject, true, translation.Position + sliceSetting.OffPos, percentage);
                    }

                }).Run();
        }

        private void UpdateProgress(UIObject uiObject, bool isVisible, Vector3 pos, float value)
        {

            if (uiObject.Progress == null)
                uiObject.Progress = UIManager.Instance.CreateUIFromPrefabs("Progress");

            var progress = uiObject.Progress;

            progress.SetActive(isVisible);

            var screenPos = Camera.main.WorldToScreenPoint(pos);
            var rectTransform = progress.GetComponent<RectTransform>();
            rectTransform.position = screenPos;

            var slider = progress.GetComponent<Slider>();
            slider.value = value;
        }
    }
}