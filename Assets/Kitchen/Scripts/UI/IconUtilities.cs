using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootStone.Kitchen;
using UnityEngine;

namespace Assets.Kitchen.Scripts.UI
{
    public static class  IconUtilities
    {
        private static readonly Dictionary<EntityType, Sprite> sprites = new Dictionary<EntityType, Sprite>();

        public static void Init()
        {
            sprites[EntityType.ShrimpSlice] = Resources.Load<Sprite>("UI/Icon/demo_icon_food_Ingredients5");
            sprites[EntityType.CucumberSlice] = Resources.Load<Sprite>("UI/Icon/demo_icon_food_Ingredients6");
            sprites[EntityType.KelpSlice] = Resources.Load<Sprite>("UI/Icon/demo_icon_food_Ingredients7");
            sprites[EntityType.RiceCooked] = Resources.Load<Sprite>("UI/Icon/demo_icon_food_Ingredients1");
            sprites[EntityType.Rice] = Resources.Load<Sprite>("UI/Icon/demo_icon_food_Ingredients1");
        }
       

        public static Sprite GetIconSprite(EntityType entityType)
        {
            return !sprites.ContainsKey(entityType) ? null : sprites[entityType];
        }

    }
}
