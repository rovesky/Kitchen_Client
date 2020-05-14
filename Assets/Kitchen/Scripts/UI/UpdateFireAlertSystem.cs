using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateFireAlertSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    in FireAlertSetting setting,
                    in FireAlertPredictedState state,
                    in LocalToWorld translation,
                    in UIObject uiObject) =>
                {
                    var percentage = (float) state.CurTick / setting.TotalTick;

                    var visible = percentage > 0.28f && percentage < 0.36f ||
                                   percentage > 0.49f && percentage < 0.55f ||
                                   percentage > 0.64f && percentage < 0.68f ||
                                   percentage > 0.74f && percentage < 0.78f ||
                                   percentage > 0.82f && percentage < 0.84f ||
                                   percentage > 0.86f && percentage < 0.88f ||
                                   percentage > 0.90f && percentage < 0.92f ||
                                   percentage > 0.94f && percentage < 0.96f ||
                                   percentage > 0.98f && percentage < 1.0f;

                    UpdateFireAlert(uiObject, visible, translation.Position + new float3(0.5f, -1f, 0));

                }).Run();
        }

        private void UpdateFireAlert(UIObject uiObject, bool isVisible, Vector3 pos)
        {
            if (uiObject.Info == null)
                uiObject.Info = UIManager.Instance.CreateUIFromPrefabs("FireAlert");

            var sliceAlert = uiObject.Info;
            sliceAlert.SetActive(isVisible);

            var screenPos = Camera.main.WorldToScreenPoint(pos);
            var rectTransform = sliceAlert.GetComponent<RectTransform>();
            rectTransform.position = screenPos;
        }
    }
}