using Assets.Kitchen.Scripts.UI;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdatePlateIconSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<Plate>()
                .WithoutBurst()
                .ForEach((Entity entity,
                    in PlatePredictedState plateState,
                    in MultiSlotPredictedState slotState,
                    in LocalToWorld localToWorld,
                  //  in OffsetSetting offsetting,
                    in UIObject uiObject) =>
                {
                    var pos = localToWorld.Position+ new float3(0,2.0f,0) ;

                    UpdateIcon(uiObject, pos, plateState, slotState);
                }).Run();
        }

        private void UpdateIcon(UIObject uiObject, Vector3 pos, PlatePredictedState plateState,
            MultiSlotPredictedState slotState)
        {
            if (uiObject.Icon == null)
                uiObject.Icon = UIManager.Instance.CreateUIFromPrefabs("PlateIcon");

            var platePannel = uiObject.Icon;
            var screenPos = Camera.main.WorldToScreenPoint(pos);

            var rectTransform = platePannel.GetComponent<RectTransform>();
            rectTransform.position = screenPos;

            if (plateState.Product == Entity.Null)
            {
                SetImage(1, slotState.Value.FilledIn1, platePannel);
                SetImage(2, slotState.Value.FilledIn2, platePannel);
                SetImage(3, slotState.Value.FilledIn3, platePannel);
                SetImage(4, slotState.Value.FilledIn4, platePannel);
                platePannel.SetActive(!slotState.Value.IsEmpty());
             //   if(!slotState.Value.IsEmpty())
                //    FSLog.Info($"UpdatePlateIconSystem:plateState.Product == Entity.Null");
            }
            else
            {
                var gameEntity = EntityManager.GetComponentData<GameEntity>(plateState.Product);
                var menuT = MenuUtilities.GetMenuTemplate(gameEntity.Type);
                SetImage1(1, menuT.Material1, platePannel);
                SetImage1(2, menuT.Material2, platePannel);
                SetImage1(3, menuT.Material3, platePannel);
                SetImage1(4, menuT.Material4, platePannel);
                platePannel.SetActive(true);
             //   FSLog.Info($"UpdatePlateIconSystem:plateState.Product = {plateState.Product}");
            }
        }

        private void SetImage(int index, Entity entity, GameObject platePannel)
        {
            var icon = platePannel.transform.Find("Image" + index).GetComponent<Image>();
            if (entity == Entity.Null)
            {
                icon.gameObject.SetActive(false);
            }
            else
            {
                var food = EntityManager.GetComponentData<GameEntity>(entity);
                icon.sprite = IconUtilities.GetIconSprite(food.Type);
                icon.gameObject.SetActive(true);
            }
        }

        private void SetImage1(int index, EntityType type, GameObject platePannel)
        {
            var icon = platePannel.transform.Find("Image" + index).GetComponent<Image>();
            if (type == EntityType.None)
            {
                icon.gameObject.SetActive(false);
            }
            else
            {
                icon.sprite = IconUtilities.GetIconSprite(type);
                icon.gameObject.SetActive(true);
            }
        }
    }
}