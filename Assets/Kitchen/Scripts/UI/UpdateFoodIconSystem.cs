﻿using Assets.Kitchen.Scripts.UI;
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
                    in UIObject uiObject) =>
                {
                    var isVisible = !(itemState.Owner != Entity.Null &&
                                     (EntityManager.HasComponent<Plate>(itemState.Owner) ||
                                      EntityManager.HasComponent<Pot>(itemState.Owner)));
                  //  FSLog.Info($"UpdateFoodIconSystem,isVisible:{isVisible}");

                    //if(food.Type == EntityType.ShrimpSlice)
                       // FSLog.Info($"UpdateIcon,entity:{entity},isVisible:{isVisible},pos:{localToWorld.Position}");
                    UpdateIcon(uiObject, isVisible, localToWorld.Position, food.Type);
                }).Run();
        }

        private void UpdateIcon(UIObject uiObject, bool isVisible, Vector3 pos, EntityType type)
        {
            if (uiObject.Icon == null)
                uiObject.Icon = UIManager.Instance.CreateUIFromPrefabs("ItemIcon");
        
            var foodIcon = uiObject.Icon;
            var screenPos = Camera.main.WorldToScreenPoint(pos) + new Vector3(0, 50, 0);

            var rectTransform = foodIcon.GetComponent<RectTransform>();
            rectTransform.position = screenPos;
             var image = foodIcon.GetComponent<Image>();
            image.sprite = IconUtilities.GetIconSprite(type);
          
            foodIcon.SetActive(isVisible);
        }
    }
}