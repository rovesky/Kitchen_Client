using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FootStone.ECS
{
    public struct GameTick
    {

        public GameTick(int tickRate)
        {
            _tickRate = tickRate;
            TickInterval = 1.0f / _tickRate;
            Tick = 1;
            TickDuration = 0;
        }

        public void SetTick(int tick, float tickDuration)
        {
            this.Tick = tick;
            this.TickDuration = tickDuration;
        }

        public float DurationSinceTick(int tick)
        {
            return (this.Tick - tick) * TickInterval + TickDuration;
        }

        public void AddDuration(float duration)
        {
            TickDuration += duration;
            var deltaTicks = Mathf.FloorToInt(TickDuration * (float) TickRate);
            Tick += deltaTicks;
            TickDuration = TickDuration % TickInterval;
        }

        public static float GetDuration(GameTick start, GameTick end)
        {
            if (start.TickRate != end.TickRate)
            {
                GameDebug.LogError("Trying to compare time with different Tick rates (" + start.TickRate + " and " +
                                   end.TickRate + ")");
                return 0;
            }

            float result = (end.Tick - start.Tick) * start.TickInterval + end.TickDuration - start.TickDuration;
            return result;
        }

        /// <summary>Number of ticks per second.</summary>
        public int TickRate
        {
            get => _tickRate;
            set
            {
                _tickRate = value;
                TickInterval = 1.0f / _tickRate;
            }
        }

        ///<summary>Duration of current Tick as fraction.</summary>
        public float TickDurationAsFraction
        {
            get => TickDuration / TickInterval;
        }

        /// <summary>Length of each world Tick at current tickrate, e.g. 0.0166s if ticking at 60fps.</summary>
        public float TickInterval { get; private set; } 

        /// <summary>Current Tick  </summary>
        public int Tick { get; set; }

        /// <summary>Duration of current Tick </summary>
        public float TickDuration { get; set; } 


        private int _tickRate;
    }
}
