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
    

        private Dictionary<EntityType,Sprite>  sprites = new Dictionary<EntityType, Sprite>();
        protected override void OnCreate()
        {
            sprites[EntityType.None] = Resources.Load<Sprite>("demo_icon_food_Ingredients4");
            sprites[EntityType.Rice] = Resources.Load<Sprite>("demo_icon_food_Ingredients1");

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
                 
                 //   if()
                    //   var pos = localToWorld.Position + math.mul(localToWorld.Rotation, new float3(0, 0.2f, 1.3f));
                    var pos = localToWorld.Position;
                    var type = slotState.FilledIn == Entity.Null?EntityType.None:EntityType.Rice;
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

        private void UpdateIcon(Entity entity, bool isVisible, Vector3 pos, EntityType type)
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