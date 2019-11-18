using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public static class UIInput
    {
        private static Dictionary<string, bool> events = new Dictionary<string, bool>();

        public static void AddButtonClickEvent(string buttonName)
        {
            if (!events.ContainsKey(buttonName))
                events.Add(buttonName, true);

            events[buttonName] = true;

        }


        public static bool  GetButtonClick(string buttonName)
        {
            if (!events.ContainsKey(buttonName))
                return false;

            var ret = events[buttonName];
          //  events[buttonName] = false;
            return ret;
        }

        public static void ResetButtonClick()
        {        
            foreach(var id in events.Keys.ToArray())
            {               
                events[id] = false;
            }

        }

        public static void ReleaseButtonClick(string buttonName)
        {
            if (!events.ContainsKey(buttonName))
                return;

            events[buttonName] = false;
           
        }
    }
}
