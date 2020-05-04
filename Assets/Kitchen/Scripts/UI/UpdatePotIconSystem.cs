using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace FootStone.Kitchen
{
    public enum PotState
    {
        Empty,
        Full,
        Burnt
    }

    [DisableAutoCreation]
    public class UpdatePotIconSystem :SystemBase
    {

        private Dictionary<Entity, GameObject> icons = new  Dictionary<Entity, GameObject>();
    

        private Dictionary<PotState,Sprite>  sprites = new Dictionary<PotState, Sprite>();
        protected override void OnCreate()
        {
            sprites[PotState.Empty] = Resources.Load<Sprite>("demo_cookzone_btn_addition");
            sprites[PotState.Full] = Resources.Load<Sprite>("demo_icon_food_Ingredients1");
            sprites[PotState.Burnt] = Resources.Load<Sprite>("demo_cookzone_btn_fire");

        }

        protected override void OnUpdate()
        {
            Entities
                .WithAll<Pot>()
                .WithoutBurst()
                .ForEach((Entity entity,
                    in SlotPredictedState slotState,
                    in LocalToWorld localToWorld,
                    in OffsetSetting offset) =>
                {
                 
                    var pos = localToWorld.Position;
                    var type = slotState.FilledIn == Entity.Null?PotState.Empty:PotState.Full;
                    UpdateIcon(entity, true, pos, type);
                }).Run();

            var removes = new List<Entity>();
            foreach (var entity in icons.Keys)
            {
                if (!EntityManager.Exists(entity))
                {
                    removes.Add(entity);
                }
            }

            foreach (var entity in removes)
            {
                var slider = icons[entity];
                Object.Destroy(slider);
                icons.Remove(entity);
            }

        }

        private void UpdateIcon(Entity entity, bool isVisible, Vector3 pos, PotState type)
        {
            
            if(!icons.ContainsKey(entity))
                icons.Add(entity, UIManager.Instance.CreateIcon());

            var sliceIcon = icons[entity];
         
            var screenPos = Camera.main.WorldToScreenPoint(pos) + new Vector3(0,50,0);

            var rectTransform = sliceIcon.GetComponent<RectTransform>();
            rectTransform.position = screenPos;
           // FSLog.Info($"UpdateIcon,rectTransform.position:{rectTransform.position}");
            var image = sliceIcon.GetComponent<Image>();
            image.sprite = sprites[type];

            sliceIcon.SetActive(isVisible);
        }
    }
}