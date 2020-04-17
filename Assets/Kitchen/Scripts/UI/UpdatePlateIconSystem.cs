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
    public class UpdatePlateIconSystem :SystemBase
    {

        private Dictionary<Entity, GameObject> icons = new  Dictionary<Entity, GameObject>();
    

        private Dictionary<EntityType,Sprite>  sprites = new Dictionary<EntityType, Sprite>();
        protected override void OnCreate()
        {
            sprites[EntityType.ShrimpSlice] = Resources.Load<Sprite>("demo_icon_food_Ingredients5");
            sprites[EntityType.CucumberSlice] = Resources.Load<Sprite>("demo_icon_food_Ingredients6");
            sprites[EntityType.KelpSlice] = Resources.Load<Sprite>("demo_icon_food_Ingredients7");
            sprites[EntityType.RiceCooked] = Resources.Load<Sprite>("demo_icon_food_Ingredients1");
        }

        protected override void OnUpdate()
        {
            Entities
                .WithAll<Plate>()
                .WithoutBurst()
                .ForEach((Entity entity,
                    in PlatePredictedState plateState,
                   // in ItemPredictedState itemState,
                    in LocalToWorld localToWorld) =>
                {
                    //if (itemState.Owner != Entity.Null && EntityManager.HasComponent<Plate>(itemState.Owner))
                    //{
                    //    UpdateIcon(entity, false,localToWorld.Position, food.Type);
                    //    return;
                    //}
                    //   var pos = localToWorld.Position + math.mul(localToWorld.Rotation, new float3(0, 0.2f, 1.3f));
                    var pos = localToWorld.Position;
                    UpdateIcon(entity, pos, plateState);
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

        private void UpdateIcon(Entity entity, Vector3 pos, PlatePredictedState plateState)
        {

            if (!icons.ContainsKey(entity))
                icons.Add(entity, UIManager.Instance.CreatePlateIcon());

            var platePannel = icons[entity];
            var screenPos = Camera.main.WorldToScreenPoint(pos) + new Vector3(0, 55, 0);

            var rectTransform = platePannel.GetComponent<RectTransform>();
            rectTransform.position = screenPos;
         

            SetImage(1, plateState.Material1, platePannel);
            SetImage(2, plateState.Material2, platePannel);
            SetImage(3, plateState.Material3, platePannel);
            SetImage(4, plateState.Material4, platePannel);

            platePannel.SetActive(!plateState.IsEmpty());
        }

        private void SetImage(int index, Entity entity,GameObject platePannel)
        {
            var icon1 = platePannel.transform.Find("Image"+index).GetComponent<Image>();
            if (entity == Entity.Null)
            {
                icon1.gameObject.SetActive(false);
            }
            else
            {
                var food = EntityManager.GetComponentData<GameEntity>(entity);
                icon1.sprite = sprites[food.Type];
                icon1.gameObject.SetActive(true);
            }
        }
    }
}