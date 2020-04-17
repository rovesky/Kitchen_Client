using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateMenuItemSystem : SystemBase
    {
        private Dictionary<EntityType,int>  icons = new Dictionary<EntityType, int>();

        private Dictionary<ushort, bool> menus = new Dictionary<ushort, bool>();
      
        protected override void OnCreate()
        {
            icons[EntityType.ShrimpSlice] = 5;
            icons[EntityType.CucumberSlice] = 6;
            icons[EntityType.KelpSlice] = 7;
            icons[EntityType.RiceCooked] = 1;
        }

        private int TypeToIcon(int type)
        {
            if (!icons.ContainsKey((EntityType)type))
                return 0;
            return icons[(EntityType) type];
        }

        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in Menu menu) =>
                {
                    if (!menus.ContainsKey(menu.Index))
                    {
                        UIManager.Instance.AddMenu(menu.ProductId,
                            TypeToIcon(menu.MaterialId1),
                            TypeToIcon(menu.MaterialId2),
                            TypeToIcon(menu.MaterialId3),
                            TypeToIcon(menu.MaterialId4));
                    }

                    menus[menu.Index] = true;
                 
                 
                }).Run();

            var removes = new List<ushort>();
            foreach(var key in menus.Keys)
            {
                var menu = menus[key];
                if (!menu)
                {
                    UIManager.Instance.RemoveMenu();
                    removes.Add(key);
                }
            }

            foreach (var index in removes)
            {
                menus.Remove(index);
            }

            var list = menus.Keys.ToArray();
            foreach (var key in list)
            {
                menus[key] = false;
            }
           
        }
    }
}