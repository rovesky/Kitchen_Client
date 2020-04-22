using System.Collections.Generic;
using System.Linq;
using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateMenuItemSystem : SystemBase
    {
        private Dictionary<EntityType,int>  icons = new Dictionary<EntityType, int>();
        private Dictionary<EntityType,int>  productIcons = new Dictionary<EntityType, int>();

        private Dictionary<ushort, bool> menus = new Dictionary<ushort, bool>();
      
        protected override void OnCreate()
        {

            icons[EntityType.ShrimpSlice] = 5;
            icons[EntityType.CucumberSlice] = 6;
            icons[EntityType.KelpSlice] = 7;
            icons[EntityType.RiceCooked] = 1;

            productIcons[EntityType.ShrimpProduct] = 1;
            productIcons[EntityType.Sushi] = 2;
        }

        private int TypeToIcon(int type)
        {
            if (!icons.ContainsKey((EntityType)type))
                return 0;
            return icons[(EntityType) type];
        }

        private int ProductToIcon(int type)
        {
            if (!productIcons.ContainsKey((EntityType)type))
                return 0;
            return productIcons[(EntityType) type];
        }

        protected override void OnUpdate()
        {

            //   FSLog.Info($"UpdateMenuItemSystem OnUpdate1");
            var query = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(MenuItem)
                }
            });

            //   if (query.CalculateEntityCount() > 0)
            {
                var entities = query.ToEntityArray(Allocator.TempJob);

                for (var i = 0; i < entities.Length; ++i)
                {
                    var entity = entities[i];
                    var menu = EntityManager.GetComponentData<MenuItem>(entity);
                    if (!menus.ContainsKey(menu.Index))
                    {
                        UIManager.Instance.AddMenu(menu.Index, ProductToIcon(menu.ProductId),
                            TypeToIcon(menu.MaterialId1),
                            TypeToIcon(menu.MaterialId2),
                            TypeToIcon(menu.MaterialId3),
                            TypeToIcon(menu.MaterialId4));
                    }

                    menus[menu.Index] = true;
                }

                entities.Dispose();
            }

            var removes = new List<ushort>();
            foreach (var key in menus.Keys)
            {

                var menu = menus[key];
                //    FSLog.Info($"key:{key},{menu}");
                if (!menu)
                {
                    UIManager.Instance.RemoveMenu(key);
                    removes.Add(key);
                }
            }

            foreach (var index in removes)
            {
                menus.Remove(index);
                FSLog.Info($"RemoveMenu,{menus.Count}");
            }

            var list = menus.Keys.ToArray();
            foreach (var key in list)
            {
                menus[key] = false;
            }

        }
    }
}