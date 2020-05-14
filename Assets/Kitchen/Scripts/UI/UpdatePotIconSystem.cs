using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdatePotIconSystem :SystemBase
    {
        private Dictionary<Entity, GameObject> icons = new  Dictionary<Entity, GameObject>();
        private Dictionary<PotState,Sprite>  sprites = new Dictionary<PotState, Sprite>();

        protected override void OnCreate()
        {
            sprites[PotState.Empty] = Resources.Load<Sprite>("UI/Icon/demo_cookzone_btn_addition");
            sprites[PotState.Full] = Resources.Load<Sprite>("UI/Icon/demo_icon_food_Ingredients1");
            sprites[PotState.Cooked] = Resources.Load<Sprite>("UI/Icon/demo_icon_food_Ingredients1");
            sprites[PotState.Burnt] = Resources.Load<Sprite>("UI/Icon/demo_cookzone_btn_fire");
        }

        protected override void OnUpdate()
        {
            Entities
                .WithAll<Pot>()
                .WithNone<NewServerEntity>()
                .WithoutBurst()
                .ForEach((Entity entity,
                    in SlotPredictedState slotState,
                    in PotPredictedState burntState,
                    in LocalToWorld localToWorld,
                    in UIObject uiObject) =>
                {
                    UpdateIcon(uiObject, true, localToWorld.Position, burntState.State);
                }).Run();
           

        }

        private void UpdateIcon(UIObject uiObject, bool isVisible, Vector3 pos, PotState type)
        {
            if (uiObject.Icon == null)
                uiObject.Icon = UIManager.Instance.CreateUIFromPrefabs("ItemIcon");
        
            var potIcon = uiObject.Icon;
         
            var screenPos = Camera.main.WorldToScreenPoint(pos) + new Vector3(0,50,0);

            var rectTransform = potIcon.GetComponent<RectTransform>();
            rectTransform.position = screenPos;
           // FSLog.Info($"UpdateIcon,rectTransform.position:{rectTransform.position}");
            var image = potIcon.GetComponent<Image>();
            image.sprite = sprites[type];

            potIcon.SetActive(isVisible);
        }
    }
}