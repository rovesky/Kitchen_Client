using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
    public enum UIInfoType
    {
        FireAlert,
        Cooked
    }

    [DisableAutoCreation]
    public class UpdatePotInfoSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    in FireAlertSetting setting,
                    in FireAlertPredictedState state,
                    in PotPredictedState potState,
                    in LocalToWorld translation,
                    in UIObject uiObject) =>
                {
                    if(potState.State != PotState.Cooked)
                        return;

                    var percentage = (float) state.CurTick / setting.TotalTick;

                    var visible =  percentage > 0 && percentage < 0.15f ||
                                   percentage > 0.28f && percentage < 0.36f ||
                                   percentage > 0.49f && percentage < 0.55f ||
                                   percentage > 0.64f && percentage < 0.68f ||
                                   percentage > 0.74f && percentage < 0.78f ||
                                   percentage > 0.82f && percentage < 0.84f ||
                                   percentage > 0.86f && percentage < 0.88f ||
                                   percentage > 0.90f && percentage < 0.92f ||
                                   percentage > 0.94f && percentage < 0.96f ||
                                   percentage > 0.98f && percentage < 1.0f;

                    var type = percentage < 0.2f ? UIInfoType.Cooked : UIInfoType.FireAlert;

                    UpdateInfo(uiObject, visible, translation.Position + new float3(0.5f, -1.5f, 0),type);

                }).Run();
        }

        private void UpdateInfo(UIObject uiObject, bool isVisible, Vector3 pos,UIInfoType type)
        {
            if (uiObject.Info == null)
                uiObject.Info = UIManager.Instance.CreateUIFromPrefabs("Info");

            var info = uiObject.Info;
            info.SetActive(isVisible);

            var screenPos = Camera.main.WorldToScreenPoint(pos);
            var rectTransform = info.GetComponent<RectTransform>();
            rectTransform.position = screenPos;
            var image = info.GetComponent<Image>();
            if (type == UIInfoType.FireAlert)
            {
                rectTransform.sizeDelta = new Vector2(60, 60);
                image.sprite = Resources.Load<Sprite>("UI/demo_cookzone_btn_warning");
            }
            else if (type == UIInfoType.Cooked)
            {
                rectTransform.sizeDelta = new Vector2(20, 20);
                image.sprite = Resources.Load<Sprite>("UI/demo_cookzone_state_get");

            }
        }
    }
}