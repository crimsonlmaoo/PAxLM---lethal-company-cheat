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
        //DUDE these 2 are useless my dumbass made it loop causing a crash
        public PlayerControllerB GetPlayer()
        {
            return GetPlayer();
        }
        public PlayerLevel GetLevel()
        {
            return GetLevel();
        }
    }
}
