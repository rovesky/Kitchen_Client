using System.Collections.Generic;
using System.Linq;

namespace FootStone.Kitchen
{
    public static class UIInput
    {
        private static readonly Dictionary<string, bool> Events = new Dictionary<string, bool>();

        public static void AddButtonClickEvent(string buttonName)
        {
            if (!Events.ContainsKey(buttonName))
                Events.Add(buttonName, true);

            Events[buttonName] = true;
        }


        public static bool GetButtonClick(string buttonName)
        {
            if (!Events.ContainsKey(buttonName))
                return false;

            var ret = Events[buttonName];
            //  events[buttonName] = false;
            return ret;
        }

        public static void ResetButtonClick()
        {
            foreach (var id in Events.Keys.ToArray()) Events[id] = false;
        }

        public static void ReleaseButtonClick(string buttonName)
        {
            if (!Events.ContainsKey(buttonName))
                return;

            Events[buttonName] = false;
        }
    }
}