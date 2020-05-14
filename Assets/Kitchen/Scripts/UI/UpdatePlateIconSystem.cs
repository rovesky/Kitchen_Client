using Assets.Kitchen.Scripts.UI;
using FootStone.ECS;
using Unity.Entities;
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
                    in UIObject uiObject) =>
                {
                    UpdateIcon(uiObject, localToWorld.Position, plateState, slotState);
                }).Run();
        }

        private void UpdateIcon(UIObject uiObject, Vector3 pos, PlatePredictedState plateState,
            MultiSlotPredictedState slotState)
        {
            if (uiObject.Icon == null)
                uiObject.Icon = UIManager.Instance.CreateUIFromPrefabs("PlateIcon");

            var platePannel = uiObject.Icon;
            var screenPos = Camera.main.WorldToScreenPoint(pos) + new Vector3(0, 55, 0);

            var rectTransform = platePannel.GetComponent<RectTransform>();
            rectTransform.position = screenPos;

            if (plateState.Product == Entity.Null)
            {
                SetImage(1, slotState.Value.FilledIn1, platePannel);
                SetImage(2, slotState.Value.FilledIn2, platePannel);
                SetImage(3, slotState.Value.FilledIn3, platePannel);
                SetImage(4, slotState.Value.FilledIn4, platePannel);

                platePannel.SetActive(!slotState.Value.IsEmpty());

             //   FSLog.Info($"UpdatePlateIconSystem:plateState.Product == Entity.Null");

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

               // FSLog.Info($"UpdatePlateIconSystem:plateState.Product != Entity.Null");
            }
        }

        private void SetImage(int index, Entity entity, GameObject platePannel)
        {
            var icon1 = platePannel.transform.Find("Image" + index).GetComponent<Image>();
            if (entity == Entity.Null)
            {
                icon1.gameObject.SetActive(false);
            }
            else
            {
                var food = EntityManager.GetComponentData<GameEntity>(entity);
                icon1.sprite = IconUtilities.GetIconSprite(food.Type);
                icon1.gameObject.SetActive(true);
            }
        }

        private void SetImage1(int index, EntityType type, GameObject platePannel)
        {
            var icon1 = platePannel.transform.Find("Image" + index).GetComponent<Image>();
            if (type == EntityType.None)
            {
                icon1.gameObject.SetActive(false);
            }
            else
            {
                icon1.sprite = IconUtilities.GetIconSprite(type);
                icon1.gameObject.SetActive(true);
            }
        }
    }
}