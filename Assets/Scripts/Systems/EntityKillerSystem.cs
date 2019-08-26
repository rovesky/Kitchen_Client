//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Unity.Entities;

//namespace Assets.Scripts.ECS
//{
//    public class EntityKillerSystem : ComponentSystem
//    {

//        protected override void OnUpdate()
//        {
//            Entities.ForEach((Entity entity, ref EntityKiller killer) =>
//            {
//                killer.TimeToDie--;
//                if (killer.TimeToDie <= 0)
//                {

//                    PostUpdateCommands.DestroyEntity(entity);
//                }
//            });
//        }
//    }
//}
