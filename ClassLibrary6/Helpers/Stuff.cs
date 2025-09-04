using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

namespace ClassLibrary6.Helpers
{
    public class Stuff
    {
        public static Stuff inst = new Stuff();
        public NetworkManager GetNetworkManager()
        {
            return NetworkManager.Singleton; //net manager
        }

        public PlayerControllerB GetSelf()
        {
            return GameNetworkManager.Instance.localPlayerController;
        }

        public ulong GetId()
        {
            return NetworkManager.Singleton.LocalClientId; // gets like YOUR player id - useful for some things
        }
        public List<NetworkObject> GetOwnedObjects()
        {
            return GetMyClient().OwnedObjects.ToList<NetworkObject>();
        }
        //gets the client for getting the client objects - uses GetOwnedObjects for the break game 2
        public NetworkClient GetMyClient()
        {
            return NetworkManager.Singleton.LocalClient;
        }
        //useless idk why i added it
        public HUDManager GetHUDManager()
        {
            return HUDManager.Instance;
        }
        public PlayerControllerB GetPlayer()
        {
            return GetPlayer();
        }
        public PlayerLevel GetLevel()
        {
            return GetLevel();
        }

        public int ParseInt(string val)
        {
            int.TryParse(val, out int v);
            return v;
        }

        public float ParseFloat(string val)
        {
            float.TryParse(val, out float v);
            return v;
        }

        public double ParseDouble(string val)
        {
            double.TryParse(val, out double v);
            return v;
        }

        public bool ParseBool(string val)
        {
            bool.TryParse(val, out bool v);
            return v;
        }
    }
}
