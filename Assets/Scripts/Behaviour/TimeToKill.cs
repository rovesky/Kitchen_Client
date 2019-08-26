using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Behaviour
{
    public class TimeToKill : MonoBehaviour
    {
        public int Time;


        void Update()
        {
            Time--;

            if (Time <= 0)
            {
                Destroy(gameObject);
            }

        }

    }
}
