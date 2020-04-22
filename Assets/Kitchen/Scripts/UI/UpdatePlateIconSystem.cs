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
            sprites[EntityType.Sushi] = Resources.Load<Sprite>("demo_icon_food_Ingredients1");
            sprites[EntityType.ShrimpProduct] = Resources.Load<Sprite>("demo_icon_food_Ingredients5a");

        }

        protected override void OnUpdate()
        {
            Entities
                .WithAll<Plate>()
                .WithoutBurst()
                .ForEach((Entity entity,
                    in PlatePredictedState plateState,
                    in MultiSlotPredictedState slotState,
                    in LocalToWorld localToWorld) =>
                {
                    var pos = localToWorld.Position;
                    UpdateIcon(entity, pos, plateState,slotState);
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

        private void UpdateIcon(Entity entity, Vector3 pos, PlatePredictedState plateState,MultiSlotPredictedState slotState)
        {

            if (!icons.ContainsKey(entity))
                icons.Add(entity, UIManager.Instance.CreatePlateIcon());

            var platePannel = icons[entity];
            var screenPos = Camera.main.WorldToScreenPoint(pos) + new Vector3(0, 55, 0);

            var rectTransform = platePannel.GetComponent<RectTransform>();
            rectTransform.position = screenPos;

            if (plateState.Product == Entity.Null)
            {

                SetImage(1, slotState.FilledIn1, platePannel);
                SetImage(2, slotState.FilledIn2, platePannel);
                SetImage(3, slotState.FilledIn3, platePannel);
                SetImage(4, slotState.FilledIn4, platePannel);

                platePannel.SetActive(!slotState.IsEmpty());
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
            }
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

        private void SetImage1(int index, EntityType type,GameObject platePannel)
        {
            var icon1 = platePannel.transform.Find("Image"+index).GetComponent<Image>();
            if (type == EntityType.None)
            {
                icon1.gameObject.SetActive(false);
            }
            else
            {
                icon1.sprite = sprites[type];
                icon1.gameObject.SetActive(true);
            }
        }
    }
}