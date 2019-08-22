using UnityEngine;
using System.Collections.Generic;
using Unity.Entities;
using System;


namespace FootStone.ECS
{
    public class EnumeratedArrayAttribute : PropertyAttribute
    {
        public readonly string[] names;
        public EnumeratedArrayAttribute(Type enumtype)
        {
            names = Enum.GetNames(enumtype);
        }
    }


    [DefaultExecutionOrder(-1000)]
    public class Game : MonoBehaviour
    {       

        public interface IGameLoop
        {
            bool Init(string[] args);
            void Shutdown();

            void Update();
            void FixedUpdate();
            void LateUpdate();
        }

        public static Game game;
      
        public static double frameTime;

        public void Awake()
        {
            GameDebug.Assert(game == null);
            DontDestroyOnLoad(gameObject);
            game = this;
            var commandLineArgs = new List<string>(System.Environment.GetCommandLineArgs());


            Application.targetFrameRate = 30;

            m_gameLoops.Add(new ClientGameLoop());

        }



        public void FixedUpdate()
        {
            foreach (var gameLoop in m_gameLoops)
            {
                gameLoop.FixedUpdate();
            }

        }

        public void LateUpdate()
        {
            try
            {
                if (!m_ErrorState)
                {
                    foreach (var gameLoop in m_gameLoops)
                    {
                        gameLoop.LateUpdate();
                    }
                    //  Console.ConsoleLateUpdate();
                }
            }
            catch (System.Exception e)
            {
                // HandleGameloopException(e);
                throw;
            }
        }

        public void Update()
        {
            foreach (var gameLoop in m_gameLoops)
            {
                gameLoop.Update();
            }
        }

        void OnApplicationQuit()
        {
           
        }

        bool m_ErrorState = false;
        List<IGameLoop> m_gameLoops = new List<IGameLoop>();
    }

}