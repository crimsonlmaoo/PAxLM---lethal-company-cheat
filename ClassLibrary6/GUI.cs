using ClassLibrary6.Helpers;
using ClassLibrary6.Toggles;
using DigitalRuby.ThunderAndLightning;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.InputSystem;

namespace ClassLibrary4
{
    public class gui : MonoBehaviour
    {
        //gui stuff
        Rect recttington = new Rect(100, 100, 800, 550);
        int selected = 0;
        string[] tabs = { "Home", "Self", "Visuals", "Players", "Enemies", "Spawners", "Shop/Company", "Server", "Logging", "Settings" };

        //- style stuff -
        GUIStyle window;
        GUIStyle tabst;
        GUIStyle label;
        GUIStyle header;
        Texture2D black;
        Texture2D tab;
        Texture2D tab2;
        Texture2D tab3;
        Texture2D pixel;

        //enums
        public enum Enemies
        {
            Unknown,
            Centipede,
            SandSpider,
            HoarderBug,
            Flowerman,
            Crawler,
            Blob,
            DressGirl,
            Puffer,
            Nutcracker,
            RedLocustBees,
            Doublewing,
            DocileLocustBees,
            MouthDog,
            ForestGiant,
            SandWorm,
            BaboonHawk,
            SpringMan,
            Jester,
            LassoMan,
            MaskedPlayerEnemy,
            Butler,
            ButlerBees,
            RadMech,
            FlowerSnake,
            BushWolf,
            ClaySurgeon,
            CaveDweller
        }

        //for gui
        bool show = false;

        //for scrolling
        Vector2 scrolling = Vector2.zero;
        Vector3 scrollingplayers = Vector2.zero;

        //indexes and more
        EnemyAI[] enemies = Array.Empty<EnemyAI>();
        int selectedEnemyIndex = -1;
        int selectedPlayerIndex = -1;
        int selecteEnemySpawnIndex = -1;
        PlayerControllerB[] players = Array.Empty<PlayerControllerB>();

        //instances/classes
        RoundManager roundManager => RoundManager.Instance;
        StartOfRound start => StartOfRound.Instance;
        PlayerControllerB localplayer => GameNetworkManager.Instance?.localPlayerController;

        //list of things and things (some aren't used)
        List<UnlockableItem> unlockables => StartOfRound.Instance != null ? StartOfRound.Instance.unlockablesList.unlockables.ToList() : new List<UnlockableItem>();
        List<GrabbableObject> grabbables => FindObjectsOfType<GrabbableObject>().ToList();
        List<DepositItemsDesk> desks => FindObjectsOfType<DepositItemsDesk>().ToList();
        List<Terminal> terminals => FindObjectsOfType<Terminal>().ToList();
        List<EnemyAI> enemyais => FindObjectsOfType<EnemyAI>().ToList();
        List<PlayerControllerB> playercontrollers => FindObjectsOfType<PlayerControllerB>().ToList();
        List<ShipAlarmCord> horns => FindObjectsOfType<ShipAlarmCord>().ToList();
        List<ShipBuildModeManager> shipbuilders => FindObjectsOfType<ShipBuildModeManager>().ToList();
        List<NetworkObject> netobjects => FindObjectsOfType<NetworkObject>().ToList();
        List<DepositItemsDesk> depositdesks => FindObjectsOfType<DepositItemsDesk>().ToList();
        List<BaboonBirdAI> baboonbirds => FindObjectsOfType<BaboonBirdAI>().ToList();
        List<RoundManager> roundmanagers => FindObjectsOfType<RoundManager>().ToList();
        List<GlobalEffects> globaleffects => FindObjectsOfType<GlobalEffects>().ToList();
        List<AudioSource> audiosources => FindObjectsOfType<AudioSource>().ToList();
        List<HUDManager> hudmanagers => FindObjectsOfType<HUDManager>().ToList();
        List<SelectableLevel> selectlevels => FindObjectsOfType<SelectableLevel>().ToList();
        List<TimeOfDay> times => FindObjectsOfType<TimeOfDay>().ToList();
        List<ShotgunItem> shotguns => FindObjectsOfType<ShotgunItem>().ToList();
        List<Camera> cameras => FindObjectsOfType<Camera>().ToList();

        //harmony
        Harmony harm = new Harmony("com.ClassLibrary4.patching");

        //variables/fields
        float hue = 0f;
        float speed;
        bool init;
        string hello = "damage amount";
        string healamt = "heal amount";
        string message = "message";
        float cooldown = 0.3f;
        float lasttoggle = -1f;

        //for force emotes but it didn't work
        UnityEngine.InputSystem.InputAction.CallbackContext context;

        //caching for optimization - probably doesn't work but was an attempt at it
        static Dictionary<string, Vector2> caching = new Dictionary<string, Vector2>();

        //fps stuff
        WaitForSecondsRealtime waitfor;
        public int fpsbr { get; private set; }
        float refresh = 1f;

        private IEnumerator fps()
        {
            waitfor = new WaitForSecondsRealtime(refresh);
            while (true)
            {
                fpsbr = (int)(1 / Time.unscaledDeltaTime);
                yield return waitfor;
            }
        }

        void Start()
        {
            StartCoroutine(fps());
        }

        void Update()
        {
            if (Keyboard.current.insertKey.isPressed)
            {
                if (Time.time - lasttoggle > cooldown)
                {
                    show = !show;
                    lasttoggle = Time.time;
                }
            }

            if (Toggles.inst.t_fly)
            {
                Rigidbody rb = localplayer.gameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 force = new Vector3(10f, 20f, 10f);
                    rb.AddForce(force, ForceMode.Impulse);
                }
            }

            if (Toggles.inst.t_infstam)
            {
                localplayer.sprintMeter = 1f;
                if (localplayer.sprintMeterUI != null)
                    localplayer.sprintMeterUI.fillAmount = 1f;
            }

            if (Toggles.inst.t_infhealth)
            {
                localplayer.health = 9999999;
            }

            if (Toggles.inst.t_infcharge && localplayer.currentlyHeldObjectServer?.insertedBattery != null)
            {
                localplayer.currentlyHeldObjectServer.insertedBattery.charge = 1;
                localplayer.currentlyHeldObjectServer.insertedBattery.empty = false;
            }

            if (Toggles.inst.t_chargeany)
            {
                localplayer.currentlyHeldObject.itemProperties.requiresBattery = true;
            }

            if (Toggles.inst.t_fastheal)
            {
                localplayer.healthRegenerateTimer = 0.1f;
            }

            if (Toggles.inst.t_infreach)
            {
                localplayer.grabDistance = 999999999f;
            }

            if (Toggles.inst.t_noweight)
            {
                localplayer.currentlyHeldObjectServer.itemProperties.weight = 0f;
                localplayer.carryWeight = 0f;
            }

            if (Toggles.inst.t_strong)
            {
                localplayer.currentlyHeldObjectServer.itemProperties.twoHanded = false;
                localplayer.currentlyHeldObjectServer.itemProperties.twoHandedAnimation = false;
            }

            if (Toggles.inst.t_infammo)
            {
                foreach (ShotgunItem gun in FindObjectsOfType<ShotgunItem>())
                {
                    gun.shellsLoaded = 9999;
                }
            }

            if (Toggles.inst.t_nodark)
            {
                localplayer.nightVision.enabled = true;
                localplayer.nightVision.intensity = 30000f;
                localplayer.nightVision.range = 10000f;
            }
        }

        void OnGUI()
        {
            if (!init)
            {
                header = new GUIStyle(GUI.skin.label);
                header.normal.textColor = new Color(255.0f, 0f, 255.0f);
                header.fontSize = 16;
                header.fontStyle = FontStyle.Bold;
            }
            if (Toggles.inst.t_lesp && localplayer != null && localplayer.gameplayCamera != null)
            {
                Draw(
                    GameObject.FindObjectsOfType<Landmine>().Select(e => e.gameObject),
                    localplayer,
                    obj => "Landmine",
                    obj => Color.red,
                    true
                );
            }
            if (Toggles.inst.t_ttesp && localplayer != null && localplayer.gameplayCamera != null)
            {
                Draw(
                    GameObject.FindObjectsOfType<Turret>().Select(e => e.gameObject),
                    localplayer,
                    obj => "Turret",
                    obj => Color.red,
                    true
                );
            }
            if (Toggles.inst.t_tesp && localplayer != null && localplayer.gameplayCamera != null)
            {
                Draw(
                    GameObject.FindObjectsOfType<GrabbableObject>().Select(e => e.gameObject),
                    localplayer,
                    obj => obj.GetComponent<GrabbableObject>().itemProperties.itemName,
                    obj => Color.magenta,
                    true
                );
            }
            if (Toggles.inst.t_pesp && localplayer != null && localplayer.gameplayCamera != null)
            {
                Draw(
                    GameObject.FindObjectsOfType<PlayerControllerB>().Select(e => e.gameObject),
                    localplayer,
                    obj => obj.GetComponent<PlayerControllerB>().playerUsername,
                    obj => Color.blue,
                    true
                );
            }
            if (Toggles.inst.t_esp && localplayer != null && localplayer.gameplayCamera != null)
            {
                Draw(
                    GameObject.FindObjectsOfType<EnemyAI>().Select(e => e.gameObject),
                    localplayer,
                    obj => obj.GetComponent<EnemyAI>().enemyType.enemyName,
                    obj => Color.red,
                    true
                );
            }
            string[] mods = new string[]
            {
                    Toggles.inst.spamhorn ? "Spam horn" : null
            };

            string enableds = string.Join("\n", mods.Where(name => name != null));
            GUI.Label(new Rect(0, 10, UnityEngine.Screen.width, 80), $"PAxLM - created by crimsonh - version 1.0.0 - {fpsbr} fps\n\n{enableds}", header);
            if (!show) return;
            RGB(recttington, 4);
            recttington = GUI.Window(0, recttington, ActualWindow, "PAxLM", window);
        }
        //suprisingly actually works
        [ServerRpc(RequireOwnership = false)]
        public void BuyItemsServerRpc(int[] boughtItems)
        {
            foreach (Terminal term in FindObjectsOfType<Terminal>())
            {
                term.orderedItemsFromTerminal.AddRange(boughtItems.ToList());
                term.groupCredits = 999999999;
                term.SyncGroupCreditsClientRpc(999999999, boughtItems.Length);
            }
        }
        void ActualWindow(int winid)
        {
            if (!init)
            {
                black = Tex(2, 2, Color.black);
                black = Tex(2, 2, Color.black);
                tab = Tex(2, 2, new Color(0.15f, 0.15f, 0.15f));
                tab2 = Tex(2, 2, new Color(0.25f, 0.25f, 0.25f));
                tab3 = Tex(2, 2, new Color(0.35f, 0.35f, 0.35f));

                window = new GUIStyle(GUI.skin.window);
                window.normal.background = black;
                //window.hover.background = black;
                window.active.background = black;
                window.focused.background = black;
                window.onNormal.background = black;
                //window.onHover.background = black;
                window.onActive.background = black;
                window.onFocused.background = black;
                window.padding = new RectOffset(10, 10, 30, 10);
                window.border = new RectOffset(0, 0, 0, 0);

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

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            for (int i = 0; i < tabs.Length; i++)
            {
                Color highlight = (selected == i) ? Color.Lerp(Color.white, new Color(0.3f, 0.6f, 1f), 0.5f) : Color.white;
                GUI.backgroundColor = (selected == i) ? new Color(0.25f, 0.25f, 0.25f) : new Color(0.15f, 0.15f, 0.15f);
                GUI.contentColor = highlight;

                if (GUILayout.Toggle(selected == i, tabs[i], tabst, GUILayout.Height(30)))
                    selected = i;
            }
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            switch (selected)
            {
                case 0: //home
                    GUILayout.Label("Welcome to PAxLM", label);

                    GUILayout.Space(20);
                    GUILayout.Label("Changelog", header);
                    GUILayout.Label("- first release!\n- no idea what works and what doesn't, but some might be host just yadada find out what works yourself.\nif any bugs or such found just reply on the thread", label);
                    break;
                case 1: //self
                    Toggles.inst.t_fly = GUILayout.Toggle(Toggles.inst.t_fly, "Velocity fly");
                    Toggles.inst.t_nodark = GUILayout.Toggle(Toggles.inst.t_nodark, "No dark/nightvision");
                    Toggles.inst.t_infammo = GUILayout.Toggle(Toggles.inst.t_infammo, "Inf shotgun ammo");
                    Toggles.inst.t_infhealth = GUILayout.Toggle(Toggles.inst.t_infhealth, "Inf health");
                    Toggles.inst.t_infstam = GUILayout.Toggle(Toggles.inst.t_infstam, "Inf stamina");
                    Toggles.inst.t_infcharge = GUILayout.Toggle(Toggles.inst.t_infcharge, "Inf battery charge");
                    Toggles.inst.t_chargeany = GUILayout.Toggle(Toggles.inst.t_chargeany, "Charge anything");
                    Toggles.inst.t_noweight = GUILayout.Toggle(Toggles.inst.t_noweight, "No item weight");
                    Toggles.inst.t_strong = GUILayout.Toggle(Toggles.inst.t_strong, "Strong");
                    Toggles.inst.t_fastheal = GUILayout.Toggle(Toggles.inst.t_fastheal, "Fast heal");
                    Toggles.inst.t_infreach = GUILayout.Toggle(Toggles.inst.t_infreach, "Inf reach");
                    break;
                case 2: //visuals
                    Toggles.inst.t_esp = GUILayout.Toggle(Toggles.inst.t_esp, "Enemy ESP");
                    Toggles.inst.t_pesp = GUILayout.Toggle(Toggles.inst.t_pesp, "Player ESP");
                    Toggles.inst.t_tesp = GUILayout.Toggle(Toggles.inst.t_tesp, "Item ESP");
                    Toggles.inst.t_ttesp = GUILayout.Toggle(Toggles.inst.t_ttesp, "Turret ESP");
                    Toggles.inst.t_lesp = GUILayout.Toggle(Toggles.inst.t_lesp, "Landmine ESP");
                    break;
                case 3: //players
                    GUILayout.BeginHorizontal();

                    try
                    {
                        scrollingplayers = GUILayout.BeginScrollView(scrollingplayers, GUILayout.Width(300));
                        players = FindObjectsOfType<PlayerControllerB>();

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

                            if (GUILayout.Button("Force emote", tabst))
                            {
                                player.PerformEmote(context, 1);
                            }

                            if (GUILayout.Button("Delete items <color=red>(HOST)</color>", tabst))
                            {
                                player.DestroyItemInSlotAndSync(0);
                                player.DestroyItemInSlotAndSync(1);
                                player.DestroyItemInSlotAndSync(2);
                                player.DestroyItemInSlotAndSync(3);
                            }

                            if (GUILayout.Button("Tp to", tabst))
                            {
                                var self = Stuff.inst.GetSelf();
                                if (self != null)
                                    self.transform.position = player.transform.position + new Vector3(2f, 0f, 2f);
                            }

                            if (GUILayout.Button("Strike", tabst))
                            {
                                foreach (var roundman in FindObjectsOfType<RoundManager>())
                                {
                                    roundman.LightningStrikeServerRpc(player.transform.position);
                                }
                            }
                            GUILayout.Label("Msg to send");
                            message = GUILayout.TextField(message);

                            if (GUILayout.Button("Send message"))
                            {
                                foreach (HUDManager hud in FindObjectsOfType<HUDManager>())
                                {
                                    hud.AddTextToChatOnServer(message, playerId: (int)player.actualClientId);
                                }
                            }
                        }
                        else
                        {
                            GUILayout.Label("whoever you selected is null! something went wrong.", label);
                        }
                    }
                    else
                    {
                        GUILayout.Label("select someone", label);
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    break;
                case 4: //enemies
                    GUILayout.BeginHorizontal();

                    try
                    {
                        scrolling = GUILayout.BeginScrollView(scrolling, GUILayout.Width(300));
                        enemies = FindObjectsOfType<EnemyAI>();

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
                                var self = Stuff.inst.GetSelf();
                                if (self != null)
                                    self.transform.position = selected.transform.position + new Vector3(2f, 0f, 2f);
                            }

                            if (GUILayout.Button("Tp enemy", tabst))
                            {
                                var self = Stuff.inst.GetSelf();
                                if (self != null)
                                {
                                    selected.transform.position = self.transform.position + new Vector3(2f, 0f, 2f);
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
                    break;
                case 5:
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
                            if (GUILayout.Button("Spawn", tabst, GUILayout.Width(80f)))
                            {
                                var roundman = RoundManager.Instance;
                                if (roundman != null)
                                {
                                    roundman.SpawnEnemyOnServer(
                                        GameNetworkManager.Instance.localPlayerController.transform.position + new Vector3(2f, 0f, 2f),
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
                    break;
                case 6: //shop
                    bool noprice = GUILayout.Button("No shop item cost", tabst);
                    if (noprice)
                    {
                        foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
                        {
                            item.creditsWorth = 0;
                        }
                    }

                    bool noshotdelay = GUILayout.Button("Set scrap value", tabst);
                    if (noshotdelay)
                    {
                        foreach (GrabbableObject grabbable in FindObjectsOfType<GrabbableObject>())
                        {
                            grabbable.SetScrapValue(9999);
                        }
                    }

                    bool noshotdelay2 = GUILayout.Button("All items scrap", tabst);
                    if (noshotdelay2)
                    {
                        foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
                        {
                            item.isScrap = true;
                            item.minValue = 999999;
                            item.maxValue = 999999;
                        }
                    }

                    bool buyall = GUILayout.Button("Buy all <color=red>(HOST)</color>", tabst);
                    if (buyall)
                    {
                        BuyItemsServerRpc(new int[]
                        {
                            0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20
                        });
                    }

                    bool sellal = GUILayout.Button("Add all to selling desk", tabst);
                    if (sellal)
                    {
                        foreach (DepositItemsDesk term in FindObjectsOfType<DepositItemsDesk>())
                        {
                            foreach (NetworkObject netobj in FindObjectsOfType<NetworkObject>())
                            {
                                term.AddObjectToDeskServerRpc(netobj);
                            }
                        }
                    }

                    bool makesellguyattack = GUILayout.Button("Make sell guy attack", tabst);
                    if (makesellguyattack)
                    {
                        foreach (DepositItemsDesk shop in FindObjectsOfType<DepositItemsDesk>())
                        {
                            shop.AttackPlayersServerRpc();
                        }
                    }

                    bool sellall = GUILayout.Button("Sell all", tabst);
                    if (sellall)
                    {
                        foreach (DepositItemsDesk shop in FindObjectsOfType<DepositItemsDesk>())
                        {
                            foreach (NetworkObject netobj in FindObjectsOfType<NetworkObject>())
                            {
                                shop.itemsOnCounterNetworkObjects.Add(netobj);
                            }
                            shop.SellItemsOnServer();
                        }
                    }
                    break;
                case 7: //server
                    scrolling = GUILayout.BeginScrollView(scrolling);

                    Toggles.inst.spamhorn = GUILayout.Toggle(Toggles.inst.spamhorn, "Spam horn");
                    if (Toggles.inst.spamhorn)
                    {
                        foreach (ShipAlarmCord horn in FindObjectsOfType<ShipAlarmCord>())
                        {
                            horn.HoldCordDown();
                        }
                    }
                    bool tpallthings = GUILayout.Button("Teleport all items", tabst);
                    if (tpallthings)
                    {
                        foreach (GrabbableObject grabbable in FindObjectsOfType<GrabbableObject>())
                        {
                            grabbable.transform.position = GameNetworkManager.Instance.localPlayerController.gameObject.transform.position + new Vector3(2f, 0f, 2f);
                            grabbable.parentObject.gameObject.transform.position = GameNetworkManager.Instance.localPlayerController.gameObject.transform.position + new Vector3(2f, 0f, 2f);
                        }
                    }

                    bool upgrade = GUILayout.Button("Buy all unlockables", tabst);
                    if (upgrade)
                    {
                        foreach (StartOfRound startof in FindObjectsOfType<StartOfRound>())
                        {
                            for (int i = 0; i < 30; i++)
                            {
                                startof.BuyShipUnlockableServerRpc(i, 1);
                            }
                        }
                    }

                    bool makeallmessage = GUILayout.Button("Make all message", tabst);
                    if (makeallmessage)
                    {
                        foreach (HUDManager hud in FindObjectsOfType<HUDManager>())
                        {
                            for (int i = 0; i < 60; i++)
                            {
                                hud.AddTextToChatOnServer("<color=red>IM FUCKING GEEKED HOLY SHIT</color>", playerId: i);
                            }
                        }
                    }

                    bool signaltranslator = GUILayout.Button("Send signaltranslator thing", tabst);
                    if (signaltranslator)
                    {
                        foreach (HUDManager hud in FindObjectsOfType<HUDManager>())
                        {
                            hud.UseSignalTranslatorServerRpc("Hi");
                        }
                    }

                    bool test12 = GUILayout.Button("Kill all enemies", tabst);
                    if (test12)
                    {
                        foreach (EnemyAI enemy in FindObjectsOfType<EnemyAI>())
                        {
                            enemy.KillEnemyServerRpc(true);
                        }
                    }

                    bool findalljournal = GUILayout.Button("Find all journal entries", tabst);
                    if (findalljournal)
                    {
                        foreach (HUDManager hud in FindObjectsOfType<HUDManager>())
                        {
                            for (int i = 0; i < 27; i++)
                            {
                                hud.GetNewStoryLogServerRpc(i);
                            }
                        }
                    }

                    bool spam = GUILayout.Button("Scan all enemies", tabst);
                    if (spam)
                    {
                        foreach (HUDManager hud in FindObjectsOfType<HUDManager>())
                        {
                            for (int i = 0; i < 27; i++)
                            {
                                hud.ScanNewCreatureServerRpc(i);
                            }
                        }
                    }

                    bool setalllevel = GUILayout.Button("Set all levels", tabst);
                    if (setalllevel)
                    {
                        foreach (HUDManager hud in FindObjectsOfType<HUDManager>())
                        {
                            foreach (PlayerControllerB player in FindObjectsOfType<PlayerControllerB>())
                            {
                                hud.SyncAllPlayerLevelsServerRpc(0, (int)player.actualClientId);
                            }
                        }
                    }

                    bool hostserver = GUILayout.Button("Host game", tabst);
                    if (hostserver)
                    {
                        foreach (MenuManager manager in FindObjectsOfType<MenuManager>())
                        {
                            manager.StartHosting();
                        }
                    }

                    bool storeall = GUILayout.Button("Store all", tabst);
                    if (storeall)
                    {
                        foreach (ShipBuildModeManager shipBuild in FindObjectsOfType<ShipBuildModeManager>())
                        {
                            foreach (NetworkObject netobj in FindObjectsOfType<NetworkObject>())
                            {
                                shipBuild.StoreObjectServerRpc(netobj, 0);
                            }
                        }
                    }

                    bool deleteallshipobjs = GUILayout.Button("Delete all ship objects", tabst);
                    if (deleteallshipobjs)
                    {
                        foreach (ShipBuildModeManager shipBuild in FindObjectsOfType<ShipBuildModeManager>())
                        {
                            foreach (NetworkObject netobj in FindObjectsOfType<NetworkObject>())
                            {
                                shipBuild.PlaceShipObjectServerRpc(new Vector3(1000000f, 100000f, 100000f), new Vector3(0f, 0f, 0f), netobj, 0);
                            }
                        }
                    }

                    bool equiopall = GUILayout.Button("Equip all", tabst);
                    if (equiopall)
                    {
                        foreach (PlayerControllerB player in FindObjectsOfType<PlayerControllerB>())
                        {
                            foreach (GrabbableObject obj in FindObjectsOfType<GrabbableObject>())
                            {
                                obj.EquipItem();
                            }
                        }
                    }

                    bool reviveall = GUILayout.Button("Revive all", tabst);
                    if (reviveall)
                    {
                        foreach (StartOfRound startof in FindObjectsOfType<StartOfRound>())
                        {
                            foreach (PlayerControllerB player in FindObjectsOfType<PlayerControllerB>())
                            {
                                startof.ReviveDeadPlayers();
                            }
                        }
                    }

                    bool reviveall2 = GUILayout.Button("Revive all 2 <color=red>(HOST)</color>", tabst);
                    if (reviveall2)
                    {
                        foreach (StartOfRound startof in FindObjectsOfType<StartOfRound>())
                        {
                            startof.Debug_ReviveAllPlayersServerRpc();
                        }
                    }

                    bool makeshipleave = GUILayout.Button("Make ship leave early", tabst);
                    if (makeshipleave)
                    {
                        foreach (TimeOfDay weather in FindObjectsOfType<TimeOfDay>())
                        {
                            weather.SetShipLeaveEarlyServerRpc();
                        }
                    }

                    bool makeallemote = GUILayout.Button("Make all emote", tabst);
                    if (makeallemote)
                    {
                        foreach (PlayerControllerB player in FindObjectsOfType<PlayerControllerB>())
                        {
                            context.performed.Equals(true);
                            for (int i = 0; i < 5; i++)
                                player.PerformEmote(context, i);
                        }
                    }

                    bool strikeall = GUILayout.Button("Strike all <color=red>(HOST)</color>", tabst);
                    if (strikeall)
                    {
                        foreach (PlayerControllerB player in FindObjectsOfType<PlayerControllerB>())
                        {
                            foreach (RoundManager roundman in FindObjectsOfType<RoundManager>())
                            {
                                roundman.LightningStrikeServerRpc(player.gameObject.transform.position);
                            }
                        }
                    }

                    bool deleteallitems = GUILayout.Button("Delete all items", tabst);
                    if (deleteallitems)
                    {
                        foreach (BaboonBirdAI item in FindObjectsOfType<BaboonBirdAI>())
                        {
                            foreach (NetworkObject netobj in FindObjectsOfType<NetworkObject>())
                            {
                                item.GrabScrapServerRpc(netobj, 0);
                            }
                        }
                    }

                    bool spawnallenemies = GUILayout.Button("Spawn all enemies <color=red>(HOST)</color>", tabst);
                    if (spawnallenemies)
                    {
                        foreach (RoundManager spawn in FindObjectsOfType<RoundManager>())
                        {
                            for (int i = 0; i < 27; i++)
                                spawn.SpawnEnemyOnServer(GameNetworkManager.Instance.localPlayerController.transform.position, 0, i);
                        }
                    }

                    bool tpallenemies = GUILayout.Button("Teleport all enemies <color=red>(HOST)</color>", tabst);
                    if (tpallenemies)
                    {
                        foreach (EnemyAI enemy in FindObjectsOfType<EnemyAI>())
                        {
                            enemy.gameObject.transform.position = GameNetworkManager.Instance.localPlayerController.transform.position + new Vector3(2f, 0f, 2f);
                            enemy.serverPosition = GameNetworkManager.Instance.localPlayerController.transform.position;
                            enemy.SyncPositionToClients();
                        }
                    }

                    bool playaudio = GUILayout.Button("SS Audio test", tabst);
                    if (playaudio)
                    {
                        foreach (GlobalEffects speaker in FindObjectsOfType<GlobalEffects>())
                        {
                            foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
                            {
                                audio.volume = 10000;
                                ServerAudio sa = new ServerAudio
                                {
                                    audioObj = audio.gameObject
                                };
                                speaker.PlayAudioServerFromSenderObject(sa);
                            }
                        }
                    }

                    bool test2 = GUILayout.Button("Break everyones game <color=red>(HOST)</color>", tabst);
                    if (test2)
                    {
                        foreach (Unity.Netcode.NetworkObject netObj in FindObjectsOfType<NetworkObject>())
                        {
                            netObj.Despawn();
                        }
                    }

                    bool test = GUILayout.Button("Destroy everyones game <color=red>(HOST)</color>", tabst);
                    if (test)
                    {
                        foreach (NetworkObject objects in Stuff.inst.GetOwnedObjects())
                        {
                            objects.Despawn(true);
                        }
                    }

                    GUILayout.EndScrollView();
                    break;
                case 8: //logging
                    bool getids = GUILayout.Button("Log all players", tabst);
                    if (getids)
                    {
                        foreach (PlayerControllerB player in FindObjectsOfType<PlayerControllerB>())
                        {
                            Debug.Log($"player objecto: {player.name} | id: {player.actualClientId} | owner: {player.OwnerClientId}" + " more info: " + player.IsOwner + " " + player.playerClientId);
                        }
                    }

                    bool getenemies = GUILayout.Button("Log spawned enemies", tabst);
                    if (getenemies)
                    {
                        foreach (EnemyAI enemy in FindObjectsOfType<EnemyAI>())
                        {
                            Debug.Log($"enemy: {enemy.enemyType.enemyName} | can die: {enemy.enemyType.canDie} | max amount: {enemy.enemyType.MaxCount}");
                        }
                    }

                    bool getcams = GUILayout.Button("Log all cams", tabst);
                    if (getcams)
                    {
                        foreach (Camera cam in FindObjectsOfType<Camera>())
                        {
                            Debug.Log($"cam: {cam.name}");
                        }
                    }

                    bool getscrap = GUILayout.Button("Log all scrap", tabst);
                    if (getscrap) 
                    {
                        foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
                        {
                            if (item.isScrap)
                            {
                                Debug.Log($"scrap item: {item.itemName} | min: {item.minValue} | max: {item.maxValue}");
                            }
                        }
                    }

                    bool getitems = GUILayout.Button("Log shop items", tabst);
                    if (getitems)
                    {
                        foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
                        {
                            if (!item.isScrap)
                            {
                                Debug.Log($"item: {item.itemName} | value: {item.creditsWorth}");
                            }
                        }
                    }

                    bool log = GUILayout.Button("Log all unlockables", tabst);
                    if (log)
                    {
                        foreach (StartOfRound startof in FindObjectsOfType<StartOfRound>())
                        {
                            foreach (var unlockable in startof.unlockablesList.unlockables)
                            {
                                Debug.Log($"unlockable: {unlockable.unlockableName} | type: {unlockable.unlockableType} | already unlocked: {unlockable.alreadyUnlocked} | unlocked by you: {unlockable.hasBeenUnlockedByPlayer}");
                            }
                        }
                    }

                    bool logslevels = GUILayout.Button("Log levels", tabst);
                    if (logslevels)
                    {
                        foreach (HUDManager level in FindObjectsOfType<HUDManager>())
                        {
                            foreach (var levelbl in level.playerLevels)
                            {
                                Debug.Log($"level: {levelbl.levelName} | xp: {levelbl.XPMax} | min: {levelbl.XPMin}");
                            }
                        }
                    }
                    break;
                default:
                    GUILayout.Label($"{tabs[selected]}", label);
                    break;
            }

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }

        private Texture2D Tex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void RGB(Rect rect, int thickness)
        {
            if (pixel == null)
            {
                pixel = new Texture2D(1, 1);
                pixel.wrapMode = TextureWrapMode.Clamp;
                pixel.SetPixel(0, 0, Color.white);
                pixel.Apply();
            }

            hue += Time.deltaTime * 0.1f;
            if (hue > 1f) hue -= 1f;

            int width = Mathf.CeilToInt(rect.width);
            int height = Mathf.CeilToInt(rect.height);
            float perimeter = 2f * (width + height);
            float pos = 0f;

            for (int x = 0; x < width + thickness; x++, pos++)
            {
                GUI.color = Color.HSVToRGB((pos / perimeter + hue) % 1f, 1f, 1f);
                GUI.DrawTexture(new Rect(rect.x + x, rect.y - thickness, 1, thickness), pixel);
            }

            for (int y = 0; y < height + thickness; y++, pos++)
            {
                GUI.color = Color.HSVToRGB((pos / perimeter + hue) % 1f, 1f, 1f);
                GUI.DrawTexture(new Rect(rect.x + width, rect.y + y, thickness, 1), pixel);
            }

            for (int x = width + thickness - 1; x >= 0; x--, pos++)
            {
                GUI.color = Color.HSVToRGB((pos / perimeter + hue) % 1f, 1f, 1f);
                GUI.DrawTexture(new Rect(rect.x + x, rect.y + height, 1, thickness), pixel);
            }

            for (int y = height + thickness - 1; y >= 0; y--, pos++)
            {
                GUI.color = Color.HSVToRGB((pos / perimeter + hue) % 1f, 1f, 1f);
                GUI.DrawTexture(new Rect(rect.x - thickness, rect.y + y, thickness, 1), pixel);
            }

            GUI.color = Color.white;
        }
        public static void labelesp(GUIStyle Style, float X, float Y, float W, float H, string str, Color col, bool centerx = false, bool centery = false)
        {
            GUIContent content = new GUIContent(str);

            Vector2 size = Style.CalcSize(content);
            float fX = centerx ? (X - size.x / 2f) : X,
                fY = centery ? (Y - size.y / 2f) : Y;

            Style.normal.textColor = Color.black;
            GUI.Label(new Rect(fX, fY, size.x, H), str, Style);

            Style.normal.textColor = col;
            GUI.Label(new Rect(fX + 1f, fY + 1f, size.x, H), str, Style);
        }

        public static void Draw(IEnumerable<GameObject> objects, PlayerControllerB localPlayer,
            System.Func<GameObject, string> labelSelector,
            System.Func<GameObject, Color> colorSelector,
            bool displayDistance = true)
        {
            if (localPlayer == null || localPlayer.gameplayCamera == null) return;
            Camera cam = localPlayer.gameplayCamera;

            GUIStyle style = GUI.skin.label;

            foreach (var obj in objects)
            {
                if (obj == null || !obj.activeSelf) continue;

                Vector3 worldPos = obj.transform.position + Vector3.up * 2f;
                Vector3 screenPos;

                if (!WorldToScreen(cam, worldPos, out screenPos) || screenPos.z <= 0f)
                    continue;

                float distance = Vector3.Distance(localPlayer.transform.position, obj.transform.position);

                string label = labelSelector(obj);
                if (string.IsNullOrEmpty(label)) continue;

                label = char.ToUpper(label[0]) + label.Substring(1);
                if (displayDistance)
                    label += $" - {distance:F1}m -";
                if (!caching.TryGetValue(label, out Vector2 size))
                {
                    size = style.CalcSize(new GUIContent(label));
                    caching[label] = size;
                }

                labelesp(style, screenPos.x, screenPos.y, size.x, size.y, label, colorSelector(obj), true, true);
            }
        }
        public static bool WorldToScreen(Camera cam, Vector3 worldpos, out Vector3 screenpos)
        {
            screenpos = cam.WorldToViewportPoint(worldpos);
            screenpos.x *= (float)UnityEngine.Screen.width;
            screenpos.y *= (float)UnityEngine.Screen.height;
            screenpos.y = (float)UnityEngine.Screen.height - screenpos.y;
            return screenpos.z > 0f;
        }
    }
}
