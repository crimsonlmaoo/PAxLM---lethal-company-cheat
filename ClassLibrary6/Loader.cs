using UnityEngine;

namespace ClassLibrary4
{
    public class Loader
    {
        public static GameObject gui;
        public static void Load()
        {
            gui = new GameObject(System.Guid.NewGuid().ToString("N").Substring(0, 8));
            gui.AddComponent<gui>();
            GameObject.DontDestroyOnLoad(gui);
        }
        public static void Unload()
        {
            GameObject.Destroy(gui);
        }
    }
}
