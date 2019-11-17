using System.Collections.Generic;
using Assets.Scripts.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class SpawnPreviewClientSystem : ComponentSystem
    {  
        private Entity player;       

        protected override void OnCreate()
        {        
            EntityManager.CreateEntity(typeof(LocalPlayer));
            SetSingleton(new LocalPlayer(){ playerId = -1, playerEntity = Entity.Null });        

            player = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Player1") as GameObject, World.Active);

            FSLog.Info($" spwan entity OnCreate2");
        }

        protected override void OnUpdate()
        {
            var localPalyer = GetSingleton<LocalPlayer>();

            if (localPalyer.playerEntity != Entity.Null)
                return;
            

            var e = EntityManager.Instantiate(player);
            Translation position = new Translation() { Value = { x = 0, y = 1, z = -5 } };
            Rotation rotation = new Rotation() { Value = Quaternion.identity };

            EntityManager.SetComponentData(e, position);

            EntityManager.AddComponentData(e, new Player() { playerId = 0, id = e.Index });
            //EntityManager.AddComponentData(e, new Attack() { Power = 10000 });
            //EntityManager.AddComponentData(e, new Damage());
            //EntityManager.AddComponentData(e, new Health() { Value = 30 });
            //EntityManager.AddComponentData(e, new Score() { ScoreValue = 0, MaxScoreValue = 0 });
            EntityManager.AddComponentData(e, new UpdateUI());
            EntityManager.AddComponentData(e, new CharacterDataComponent() { SkinWidth = 0.02f, Entity = e });

            EntityManager.AddComponentData(e, new CharacterInterpolateState()
            {
                position = position.Value,
                rotation = rotation.Value
            });    

            EntityManager.AddComponentData(e, new UserCommand());
            EntityManager.AddComponentData(e, new MoveInput()
            {
                Speed = 6,
            });

            EntityManager.AddComponentData(e, new PickupItem()
            {
              //  pickupEntity = Entity.Null
            });

            EntityManager.AddComponentData(e, new ThrowItem()
            {
                speed = 10
                //  pickupEntity = Entity.Null
            });

            localPalyer.playerEntity = e;
            SetSingleton(localPalyer);

        }
    }
}
