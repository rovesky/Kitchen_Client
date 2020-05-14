using Assets.Kitchen.Scripts.UI;
using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateFoodIconSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAny<Sliced, Uncooked>()
                .WithNone<NewServerEntity>()
                .WithoutBurst()
                .ForEach((Entity entity,
                    in GameEntity food,
                    in OwnerPredictedState itemState,
                    in LocalToWorld localToWorld,
                    in IconUI iconUI) =>
                {
                    var isVisible = !(itemState.Owner != Entity.Null &&
                                     (EntityManager.HasComponent<Plate>(itemState.Owner) ||
                                      EntityManager.HasComponent<Pot>(itemState.Owner)));

                //    if(!isVisible)
                    //    FSLog.Info($"UpdateIcon,entity:{entity}");

                    //if(food.Type == EntityType.ShrimpSlice)
                       // FSLog.Info($"UpdateIcon,entity:{entity},isVisible:{isVisible},pos:{localToWorld.Position}");
                    UpdateIcon(iconUI, isVisible, localToWorld.Position, food.Type);
                }).Run();
        }

        private void UpdateIcon(IconUI iconUI, bool isVisible, Vector3 pos, EntityType type)
        {
            if (iconUI.Icon == null)
                iconUI.Icon = UIManager.Instance.CreateUIFromPrefabs("ItemIcon");


        
            var sliceIcon = iconUI.Icon;
            var screenPos = Camera.main.WorldToScreenPoint(pos) + new Vector3(0, 50, 0);

            var rectTransform = sliceIcon.GetComponent<RectTransform>();
            rectTransform.position = screenPos;
            // FSLog.Info($"UpdateIcon,rectTransform.position:{rectTransform.position}");
            var image = sliceIcon.GetComponent<Image>();
            image.sprite = IconUtilities.GetIconSprite(type);
          
            sliceIcon.SetActive(isVisible);
        }
    }
}