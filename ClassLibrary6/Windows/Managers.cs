using ClassLibrary6.Helpers;
using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.InputSystem.InputRemoting;
using UnityEngine;
using System.Net.NetworkInformation;
using DunGen.Tags;
using System.Collections;
using UnityEngine.Assertions.Must;
using static ClassLibrary4.gui;
using Unity.Netcode;
using System.Xml.Linq;

namespace ClassLibrary6.Windows
{
    public class Managers
    {
        //ugly and unorganized code incoming
        static bool init = false;
        static GUIStyle tabst;
        static List<Item> itemsList = null;
        static GUIStyle label;
        static GUIStyle header;
        static Texture2D black;
        static Texture2D tab;
        static Texture2D tab2;
        static Texture2D tab3;
        static Vector2 scrollingplayers = Vector2.zero;
        static Vector2 scrollingenemies = Vector2.zero;
        static Vector2 scrollingitems = Vector2.zero;
        static Vector2 scrollingexplorer = Vector2.zero;
        static Vector2 scrollingexplorer2 = Vector2.zero;
        static Vector2 scrollingprefabs = Vector2.zero;
        static int selectedItemIndex = -1;
        static int selectedObjectIndex = -1;
        static int selectedPrefabIndex = -1;
        static string val = "item value";
        static NetworkObject[] prefabs = Array.Empty<NetworkObject>();
        static NetworkObject[] objects = Array.Empty<NetworkObject>();
        static GrabbableObject[] items = Array.Empty<GrabbableObject>();
        static PlayerControllerB[] players = Array.Empty<PlayerControllerB>();
        static EnemyAI[] enemies = Array.Empty<EnemyAI>();
        static Landmine[] landmines = Array.Empty<Landmine>();
        static Vector2 scrollinglandmines = Vector2.zero;
        static int selectedLandmineIndex = -1;
        static int selectedEnemyIndex = -1;
        static int selectedPlayerIndex = -1;
        static int selectedItemIndex2 = -1;
        static Vector2 scrolling = Vector2.zero;
        static string hello = "dmg amount";
        static string healamt = "heal amount";
        static string message = "hello";
        static UnityEngine.InputSystem.InputAction.CallbackContext context = new UnityEngine.InputSystem.InputAction.CallbackContext();

        public static void InitMyTexsBruh()
        {
            if (!init)
            {
                black = Tex(2, 2, Color.black);
                black = Tex(2, 2, Color.black);
                tab = Tex(2, 2, new Color(0.15f, 0.15f, 0.15f));
                tab2 = Tex(2, 2, new Color(0.25f, 0.25f, 0.25f));
                tab3 = Tex(2, 2, new Color(0.35f, 0.35f, 0.35f));

                tabst = new GUIStyle(GUI.skin.button);
                tabst.normal.background = tab;
                tabst.hover.background = tab2;
                tabst.active.background = tab2;
                tabst.focused.background = tab;
                tabst.onNormal.background = tab3;
                tabst.onHover.background = tab3;
                tabst.onActive.background = tab3;
                tabst.onFocused.background = tab3;

                tabst.normal.textColor = Color.white;
                tabst.onNormal.textColor = Color.white;
                tabst.hover.textColor = Color.white;
                tabst.onHover.textColor = Color.white;
                tabst.active.textColor = Color.white;
                tabst.onActive.textColor = Color.white;
                tabst.focused.textColor = Color.white;
                tabst.onFocused.textColor = Color.white;

                tabst.fontSize = 14;
                tabst.alignment = TextAnchor.MiddleCenter;
                tabst.margin = new RectOffset(2, 2, 2, 2);

                label = new GUIStyle(GUI.skin.label);
                label.normal.textColor = Color.white;
                label.wordWrap = true;
                label.fontSize = 14;
                init = true;
            }
        }

        public static void Players(int windowid)
        {
            GUILayout.BeginHorizontal();

            try
            {
                scrollingplayers = GUILayout.BeginScrollView(scrollingplayers, GUILayout.Width(300));
                players = GameObject.FindObjectsOfType<PlayerControllerB>();

                for (int i = 0; i < players.Length; i++)
                {
                    var player = players[i];
                    if (player == null) continue;

                    if (GUILayout.Button($"player: {player.playerUsername} | hp: {player.health}", tabst))
                    {
                        selectedPlayerIndex = i;
                    }
                }
            }
            finally
            {
                GUILayout.EndScrollView();
            }

            GUILayout.BeginVertical();

            if (selectedPlayerIndex >= 0 && selectedPlayerIndex < players.Length)
            {
                scrolling = GUILayout.BeginScrollView(scrolling);
                var player = players[selectedPlayerIndex];
                if (player != null)
                {
                    GUILayout.Label($"selected: {player.playerUsername} | id: {player.actualClientId} | health: {player.health}", label);

                    if (player.ItemSlots != null)
                    {
                        foreach (var item in player.ItemSlots)
                        {
                            if (item == null) continue;
                            GUILayout.Label($"value: {item.scrapValue} | item: {item.name}", label);
                            Debug.Log($"value: {item.scrapValue} | item: {item.name}");
                        }
                    }

                    if (GUILayout.Button("Kill", tabst))
                    {
                        player.KillPlayer(new Vector3(100f, 100f, 100f));
                    }
                    GUILayout.Label("Dmg amount");
                    hello = GUILayout.TextField(hello);
                    if (GUILayout.Button("Damage", tabst))
                    {
                        if (int.TryParse(hello, out int dmg))
                            player.DamagePlayer(dmg);
                    }
                    GUILayout.Label("Heal amt");
                    healamt = GUILayout.TextField(healamt);
                    if (GUILayout.Button("Heal", tabst))
                    {
                        player.HealServerRpc();
                    }

                    if (GUILayout.Button("Drop items", tabst))
                    {
                        player.DropAllHeldItemsServerRpc();
                    }

                    if (GUILayout.Button("Break game/delete", tabst))
                    {
                        player.NetworkObject.Despawn();
                    }

                    if (GUILayout.Button("Delete items <color=red>(HOST)</color>", tabst))
                    {
                        player.DestroyItemInSlotServerRpc(0);
                        player.DestroyItemInSlotServerRpc(1);
                        player.DestroyItemInSlotServerRpc(2);
                        player.DestroyItemInSlotServerRpc(3);
                    }

                    if (GUILayout.Button("Tp to", tabst))
                    {
                        var self = Stuff.inst.GetSelf();
                        if (self != null)
                            self.transform.position = player.transform.position + new Vector3(2f, 0f, 2f);
                    }

                    if (GUILayout.Button("Strike", tabst))
                    {
                        foreach (var roundman in GameObject.FindObjectsOfType<RoundManager>())
                        {
                            roundman.LightningStrikeServerRpc(player.transform.position);
                        }
                    }
                    GUILayout.Label("Msg to send");
                    message = GUILayout.TextField(message);

                    if (GUILayout.Button("Send message"))
                    {
                        foreach (HUDManager hud in GameObject.FindObjectsOfType<HUDManager>())
                        {
                            hud.AddTextToChatOnServer(message, playerId: (int)player.playerClientId);
                        }
                    }
                }
                else
                {
                    GUILayout.Label("whoever you selected is null! something went wrong.", label);
                }
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("select someone", label);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }
        public static void Landmine(int windowid)
        {
            GUILayout.BeginHorizontal();

            try
            {
                scrollinglandmines = GUILayout.BeginScrollView(scrollinglandmines, GUILayout.Width(300));
                landmines = GameObject.FindObjectsOfType<Landmine>();

                for (int i = 0; i < landmines.Length; i++)
                {
                    Landmine enemy = landmines[i];
                    if (enemy == null) continue;

                    if (GUILayout.Button($"{enemy.gameObject.name}", tabst))
                    {
                        selectedLandmineIndex = i;
                    }
                }
            }
            finally
            {
                GUILayout.EndScrollView();
            }
            GUILayout.BeginVertical();

            if (selectedLandmineIndex >= 0 && selectedLandmineIndex < landmines.Length)
            {
                Landmine selected = landmines[selectedLandmineIndex];
                if (selected != null)
                {
                    GUILayout.Label($"selected: {selected.gameObject.name}", label);

                    if (GUILayout.Button("Detonate", tabst))
                    {
                        selected.ExplodeMineServerRpc();
                    }
                }
                else
                {
                    GUILayout.Label("something went wrong! (something was null actually lol)", label);
                }
            }
            else
            {
                GUILayout.Label("select an enemy", label);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }
        public static void EnemiesWindow(int windowid)
        {
            GUILayout.BeginHorizontal();

            try
            {
                scrollingenemies = GUILayout.BeginScrollView(scrollingenemies, GUILayout.Width(300));
                enemies = GameObject.FindObjectsOfType<EnemyAI>();

                for (int i = 0; i < enemies.Length; i++)
                {
                    EnemyAI enemy = enemies[i];
                    if (enemy == null) continue;

                    if (GUILayout.Button($"{enemy.enemyType.enemyName}", tabst))
                    {
                        selectedEnemyIndex = i;
                    }
                }
            }
            finally
            {
                GUILayout.EndScrollView();
            }
            GUILayout.BeginVertical();

            if (selectedEnemyIndex >= 0 && selectedEnemyIndex < enemies.Length)
            {
                EnemyAI selected = enemies[selectedEnemyIndex];
                if (selected != null)
                {
                    GUILayout.Label($"selected: {selected.enemyType.enemyName}", label);

                    if (GUILayout.Button("Kill", tabst))
                    {
                        selected.KillEnemyServerRpc(true);
                    }

                    if (GUILayout.Button("Tp to", tabst))
                    {
                        var self = GameNetworkManager.Instance.localPlayerController;
                        if (self != null)
                            self.gameObject.transform.position = selected.transform.position + new Vector3(2f, 0f, 2f);
                    }

                    if (GUILayout.Button("Tp enemy", tabst))
                    {
                        var self = GameNetworkManager.Instance.localPlayerController;
                        if (self != null)
                        {
                            selected.gameObject.transform.position = self.transform.position + new Vector3(2f, 0f, 2f);
                            selected.serverPosition = self.transform.position;
                            selected.SyncPositionToClients();
                        }
                    }
                }
                else
                {
                    GUILayout.Label("something went wrong! (something was null actually lol)", label);
                }
            }
            else
            {
                GUILayout.Label("select an enemy", label);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }

        public static void Items(int windowid)
        {
            GUILayout.BeginHorizontal();

            try
            {
                scrollingitems = GUILayout.BeginScrollView(scrollingitems, GUILayout.Width(300));
                items = GameObject.FindObjectsOfType<GrabbableObject>();

                for (int i = 0; i < items.Length; i++)
                {
                    GrabbableObject enemy = items[i];
                    if (enemy == null) continue;

                    if (GUILayout.Button($"{enemy.itemProperties.itemName}", tabst))
                    {
                        selectedItemIndex = i;
                    }
                }
            }
            finally
            {
                GUILayout.EndScrollView();
            }

            GUILayout.BeginVertical();

            if (selectedItemIndex >= 0 && selectedItemIndex < items.Length)
            {
                scrolling = GUILayout.BeginScrollView(scrolling, GUILayout.Width(300));
                GrabbableObject selected = items[selectedItemIndex];
                if (selected != null)
                {
                    GUILayout.Label($"selected: {selected.itemProperties.itemName}", label);

                    if (GUILayout.Button("Activate item", tabst))
                    {
                        Notifications.Noti("Activated item!");
                        selected.ItemActivate(false);
                    }

                    if (GUILayout.Button("Set value max", tabst))
                    {
                        Notifications.Noti("Set scrap value to max!");
                        selected.SetScrapValue(int.MaxValue);
                    }

                    if (GUILayout.Button("Remove charge", tabst))
                    {
                        Notifications.Noti("Removed charge!");
                        selected.UseUpItemBatteriesServerRpc();
                    }

                    if (GUILayout.Button("Tp", tabst))
                    {
                        Notifications.Noti("Teleported! Look down");
                        selected.targetFloorPosition = GameNetworkManager.Instance.localPlayerController.gameObject.transform.position;;
                    }

                    if (GUILayout.Button("Tp to", tabst))
                    {
                        Notifications.Noti("Telported to item");
                        GameNetworkManager.Instance.localPlayerController.gameObject.transform.position = selected.gameObject.transform.position;
                    }

                    if (GUILayout.Button("Delete <color=red>(HOST)</color>", tabst))
                    {
                        Notifications.Noti("Deleted!");
                        selected.NetworkObject.Despawn();
                    }
                }
                else
                {
                    GUILayout.Label("something went wrong! (something was null actually lol)", label);
                }
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("select an item", label);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }
        public static void ItemSpawner(int windowid)
        {
            if (itemsList == null)
            {
                itemsList = new List<Item>(Resources.FindObjectsOfTypeAll<Item>());
            }

            GUILayout.BeginVertical();

            GUILayout.Label("<b>Item spawner</b>", GUI.skin.label);

            scrollingexplorer2 = GUILayout.BeginScrollView(scrollingexplorer2);

            int selectedIndex = selectedItemIndex2;

            foreach (Item item in itemsList)
            {
                GUILayout.BeginHorizontal();

                int itemIndex = itemsList.IndexOf(item);

                if (GUILayout.Button(item.itemName, tabst, GUILayout.Width(200f)))
                {
                    selectedItemIndex2 = itemIndex;
                }
                if (selectedItemIndex2 == itemIndex)
                {
                    if (GUILayout.Button("Spawn <color=red>HOST</color>", tabst, GUILayout.Width(110f)))
                    {
                        Notifications.Noti("Spawned!");
                        var obj = GameObject.Instantiate(item.spawnPrefab, GameNetworkManager.Instance.localPlayerController.transform.position, Quaternion.identity);
                        obj.GetComponent<GrabbableObject>().SetScrapValue(Stuff.inst.ParseInt(val));
                        var netobj = obj.GetComponent<NetworkObject>();
                        if (netobj != null)
                        {
                            netobj.Spawn();
                        }
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.Label("item value", label);
            val = GUILayout.TextField(val);

            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }

        public static void Spawners(int windowid)
        {
            if (itemsList == null)
            {
                itemsList = new List<Item>(Resources.FindObjectsOfTypeAll<Item>());
            }

            GUILayout.BeginVertical();
            GUILayout.Label("<b>Enemy spawner</b>", GUI.skin.label);

            scrolling = GUILayout.BeginScrollView(scrolling);

            foreach (Enemies enemyType in Enum.GetValues(typeof(Enemies)))
            {
                if (enemyType == Enemies.Unknown) continue;

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(enemyType.ToString(), tabst, GUILayout.Width(200f)))
                {
                    selectedEnemyIndex = (int)enemyType;
                }

                if (selectedEnemyIndex == (int)enemyType)
                {
                    Notifications.Noti("Spawned!");
                    if (GUILayout.Button("Spawn", tabst, GUILayout.Width(80f)))
                    {
                        var roundman = RoundManager.Instance;
                        if (roundman != null)
                        {
                            roundman.SpawnEnemyOnServer(
                                GameNetworkManager.Instance.localPlayerController.transform.position,
                                0,
                                (int)enemyType - 1
                            );
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }

        public static void Moons(int windowid)
        {
            var terminals = GameObject.FindObjectOfType<Terminal>();
            if (StartOfRound.Instance != null)
                GUILayout.Label($"current moon stuff:\nselected moon: {StartOfRound.Instance.currentLevel.name}\nmoon's weather: {StartOfRound.Instance.currentLevel.currentWeather}\nrisk: {StartOfRound.Instance.currentLevel.riskLevel}");
                for (int i = 0; i < StartOfRound.Instance.levels.Length; i++)
                {
                    var moon = StartOfRound.Instance.levels[i];

                    if (GUILayout.Button($"switch to {moon.PlanetName}, weather: {moon.currentWeather}", tabst))
                    {
                        StartOfRound.Instance.ChangeLevelServerRpc(i, terminals.groupCredits);
                    }
                }

            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }

        public static void Explorer(int windowid)
        {
            GUILayout.BeginHorizontal();

            try
            {
                scrollingexplorer = GUILayout.BeginScrollView(scrollingexplorer, GUILayout.Width(300));
                objects = GameObject.FindObjectsOfType<NetworkObject>();

                for (int i = 0; i < objects.Length; i++)
                {
                    var player = objects[i];
                    if (player == null) continue;

                    if (GUILayout.Button($"object: {player.name}", tabst))
                    {
                        selectedObjectIndex = i;
                    }
                }
            }
            finally
            {
                GUILayout.EndScrollView();
            }

            GUILayout.BeginVertical();

            if (selectedObjectIndex >= 0 && selectedObjectIndex < objects.Length)
            {
                var player = objects[selectedObjectIndex];
                if (player != null)
                {
                    GUILayout.Label($"selected: {player.name}", label);

                    if (GUILayout.Button("Delete <color=red>(HOST)</color>", tabst))
                    {
                        player.Despawn();
                    }
                }
                else
                {
                    GUILayout.Label("whatever you selected is null! something went wrong.", label);
                }
            }
            else
            {
                GUILayout.Label("select something", label);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }

        public static void PrefabExplorer(int windowid)
        {
            GUILayout.BeginHorizontal();

            try
            {
                scrollingprefabs = GUILayout.BeginScrollView(scrollingprefabs, GUILayout.Width(300));
                prefabs = Resources.FindObjectsOfTypeAll<NetworkObject>();

                for (int i = 0; i < prefabs.Length; i++)
                {
                    var player = prefabs[i];
                    if (player == null) continue;

                    if (GUILayout.Button($"object: {player.name}", tabst))
                    {
                        selectedPrefabIndex = i;
                    }
                }
            }
            finally
            {
                GUILayout.EndScrollView();
            }

            GUILayout.BeginVertical();

            if (selectedPrefabIndex >= 0 && selectedPrefabIndex < prefabs.Length)
            {
                var player = prefabs[selectedPrefabIndex];
                if (player != null)
                {
                    GUILayout.Label($"selected: {player.name}", label);

                    if (GUILayout.Button("Spawn <color=red>(HOST)</color>", tabst))
                    {
                        var obj = GameObject.Instantiate(player.gameObject, GameNetworkManager.Instance.localPlayerController.gameObject.transform.position, Quaternion.identity);
                        obj.GetComponent<NetworkObject>().Spawn();
                        Notifications.Noti("Spawned!");
                    }
                }
                else
                {
                    GUILayout.Label("whatever you selected is null! something went wrong.", label);
                }
            }
            else
            {
                GUILayout.Label("select something", label);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }

        private static Texture2D Tex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
