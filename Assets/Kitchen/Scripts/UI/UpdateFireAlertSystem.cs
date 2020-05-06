using System.CodeDom;
using System.Collections.Generic;
using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateFireAlertSystem : SystemBase
    {
        private Dictionary<Entity, GameObject> alerts = new Dictionary<Entity, GameObject>();

        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    in FireAlertSetting setting,
                    in FireAlertPredictedState state,
                    in LocalToWorld translation) =>
                {
                    var percentage = (float) state.CurTick / setting.TotalTick;

                    bool visible = percentage > 0.28f && percentage < 0.36f ||
                                   percentage > 0.49f && percentage < 0.55f ||
                                   percentage > 0.64f && percentage < 0.68f ||
                                   percentage > 0.74f && percentage < 0.78f ||
                                   percentage > 0.82f && percentage < 0.84f ||
                                   percentage > 0.86f && percentage < 0.88f ||
                                   percentage > 0.90f && percentage < 0.92f ||
                                   percentage > 0.94f && percentage < 0.96f ||
                                   percentage > 0.98f && percentage < 1.0f;

                    UpdateFireAlert(entity, visible, translation.Position + new float3(0.5f,-1f,0));


                }).Run();

            var removes = new List<Entity>();
            foreach (var entity in alerts.Keys)
            {
                if (!EntityManager.Exists(entity))
                {
                    removes.Add(entity);
                }
            }

            foreach (var entity in removes)
            {
                var slider = alerts[entity];
                Object.Destroy(slider);
                alerts.Remove(entity);
            }

        }

        private void UpdateFireAlert(Entity entity, bool isVisible, Vector3 pos)
        {
            if (!alerts.ContainsKey(entity))
                alerts.Add(entity, UIManager.Instance.CreateUIFromPrefabs("FireAlert"));


            var sliceAlert = alerts[entity];
            sliceAlert.SetActive(isVisible);

            var screenPos = Camera.main.WorldToScreenPoint(pos);
            var rectTransform = sliceAlert.GetComponent<RectTransform>();
            rectTransform.position = screenPos;
        }
    }
}