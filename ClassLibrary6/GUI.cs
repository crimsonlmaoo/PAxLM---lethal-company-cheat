using ClassLibrary6.Helpers;
using ClassLibrary6.Toggles;
using ClassLibrary6.Windows;
using DigitalRuby.ThunderAndLightning;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.InputSystem;
using static IngamePlayerSettings;

namespace ClassLibrary4
{
    public class gui : MonoBehaviour
    {
        //gui stuff
        Rect recttington2 = new Rect(100, 100, 800, 550);
        Rect recttington1 = new Rect(100, 100, 800, 550);
        Rect recttington3 = new Rect(100, 100, 800, 550);
        Rect recttington4 = new Rect(100, 100, 800, 550);
        Rect recttington5 = new Rect(100, 100, 800, 550);
        Rect recttington6 = new Rect(100, 100, 800, 550);
        Rect recttington7 = new Rect(100, 100, 800, 550);
        Rect recttington8 = new Rect(100, 100, 800, 550);
        Rect recttington9 = new Rect(100, 100, 800, 550);
        Rect recttington10 = new Rect(100, 100, 800, 550);
        Rect recttington11 = new Rect(100, 100, 800, 550);
        Rect recttington = new Rect(100, 100, 800, 550);
        int selected = 0;
        string[] tabs = { "Home", "Self", "Visuals", "Misc", "Server", "Settings", "Managers" };

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
        //thx lethal menu/icyrelic for the enemies struct!
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
        GrabbableObject[] items = Array.Empty<GrabbableObject>();
        int selectedEnemyIndex = -1;
        int selectedPlayerIndex = -1;
        int selectedItemIndex = -1;
        int selectedObjectIndex = -1;
        PlayerControllerB[] players = Array.Empty<PlayerControllerB>();
        NetworkObject[] objects = Array.Empty<NetworkObject>();

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

        //caching
        List<Landmine> landmines = null;
        List<Turret> turrets = null;
        List<GrabbableObject> grabbabless = null;
        List<PlayerControllerB> playerss = null;
        List<EnemyAI> enemiess = null;

        //harmony
        Harmony harm = new Harmony("com.ClassLibrary4.patching");

        //variables/fields
        float hue = 0f;
        float speed;
        bool init;
        string hello = "damage amount";
        string healamt = "heal amount";
        string messagetosay = "message";
        string signalmessage = "signal message thing";
        string message = "message";
        float cooldown = 0.3f;
        float lasttoggle = -1f;
        float speedd = 1f;

        //for force emotes but it didn't work
        UnityEngine.InputSystem.InputAction.CallbackContext context;

        //caching for optimization - probably doesn't work but was an attempt at it
        static Dictionary<string, Vector2> caching = new Dictionary<string, Vector2>();

        //noti stuff
        private HashSet<string> alreadynotid = new HashSet<string>();

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
            foreach (var field in typeof(Toggles).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                if (field.FieldType == typeof(bool))
                {
                    bool toggleValue = (bool)field.GetValue(Toggles.inst);
                    string toggleName = field.Name;

                    if (toggleValue && !alreadynotid.Contains(toggleName))
                    {
                        Notifications.Noti("Enabled/executed!");
                        alreadynotid.Add(toggleName);
                    }
                    else if (!toggleValue && alreadynotid.Contains(toggleName))
                    {
                        alreadynotid.Remove(toggleName);
                    }
                }
            }

            if (Keyboard.current.insertKey.isPressed)
            {
                if (Time.time - lasttoggle > cooldown)
                {
                    show = !show;
                    lasttoggle = Time.time;
                }
            }

            if (Toggles.inst.t_hearall)
            {
                foreach (PlayerControllerB players in FindObjectsOfType<PlayerControllerB>())
                {
                    players.currentVoiceChatAudioSource.minDistance = 0f;
                    players.currentVoiceChatAudioSource.maxDistance = float.MaxValue;
                }
            }

            if (Toggles.inst.t_spazlights)
            {
                ShipLights[] lights = FindObjectsOfType<ShipLights>();
                foreach (ShipLights light in lights)
                {
                    light.SetShipLightsServerRpc(false);
                    Task.Delay(1000);
                    light.SetShipLightsServerRpc(true);
                }
            }

            if (Toggles.inst.t_tvspz)
            {
                foreach (TVScript tVScript in FindObjectsOfType<TVScript>())
                {
                    tVScript.TurnOffTVServerRpc();
                    tVScript.TurnOnTVServerRpc();
                }
            }

            if (Toggles.inst.t_fly)
            {
                if (Keyboard.current.wKey.isPressed)
                {
                    localplayer.playerRigidbody.AddForce(localplayer.gameplayCamera.transform.forward * 10f, ForceMode.VelocityChange);
                }
            }

            if (Toggles.inst.t_fastclimb)
            {
                localplayer.climbSpeed = 99f;
            }

            if (Toggles.inst.t_infstam)
            {
                localplayer.sprintMeter = 1f;
                if (localplayer.sprintMeterUI != null)
                    localplayer.sprintMeterUI.fillAmount = 1f;
            }

            if (Toggles.inst.t_infhealth)
            {
                localplayer.health = int.MaxValue;
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
                localplayer.grabDistance = int.MaxValue;
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
                localplayer.currentlyHeldObjectServer.GetComponent<ShotgunItem>().shellsLoaded = int.MaxValue;
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
                Managers.InitMyTexsBruh();
                header = new GUIStyle(GUI.skin.label);
                header.normal.textColor = new Color(255.0f, 0f, 255.0f);
                header.fontSize = 16;
                header.fontStyle = FontStyle.Bold;

                GUIStyle slide = new GUIStyle(GUI.skin.horizontalSlider);
                slide.normal.background = Tex(20, 20, Color.black);

                GUIStyle thhumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
                thhumb.normal.background = Tex(10, 10, Color.red);
                thhumb.fixedWidth = 20;

                GUI.skin.horizontalSlider = slide;
                GUI.skin.horizontalSliderThumb = thhumb;
            }

            if (Toggles.inst.t_speed)
            {
                 localplayer.movementSpeed = speedd;
            }

            if (Toggles.inst.t_lesp && localplayer != null && localplayer.gameplayCamera != null)
            {
                if (Time.frameCount % 10 == 0)
                { landmines = new List<Landmine>(FindObjectsOfType<Landmine>()); }
                Draw(
                    landmines.Select(e => e.gameObject),
                    localplayer,
                    obj => "Landmine",
                    obj => Color.red,
                    true
                );
            }
            if (Toggles.inst.t_ttesp && localplayer != null && localplayer.gameplayCamera != null)
            {
                if (Time.frameCount % 10 == 0)
                { turrets = new List<Turret>(FindObjectsOfType<Turret>()); }
                Draw(
                    turrets.Select(e => e.gameObject),
                    localplayer,
                    obj => "Turret",
                    obj => Color.red,
                    true
                );
            }
            if (Toggles.inst.t_tesp && localplayer != null && localplayer.gameplayCamera != null)
            {
                if (Time.frameCount % 10 == 0)
                {
                    grabbabless = new List<GrabbableObject>(FindObjectsOfType<GrabbableObject>());
                }
                Draw(
                    grabbabless.Select(g => g.gameObject),
                    localplayer,
                    obj =>
                    {
                        var grab = obj.GetComponent<GrabbableObject>();
                        return grab != null ? grab.itemProperties.itemName : null;
                    },
                    obj => Color.magenta,
                    true
                );
            }
            if (Toggles.inst.t_pesp && localplayer != null && localplayer.gameplayCamera != null)
            {
                if (Time.frameCount % 10 == 0) { 
                    playerss = new List<PlayerControllerB>(FindObjectsOfType<PlayerControllerB>());
                    }
                Draw(
                    playerss.Select(e => e.gameObject),
                    localplayer,
                    obj => obj.GetComponent<PlayerControllerB>().playerUsername,
                    obj => Color.blue,
                    true
                );
            }
            if (Toggles.inst.t_esp && localplayer != null && localplayer.gameplayCamera != null)
            {
                if (Time.frameCount % 10 == 0)
                {
                    enemiess = new List<EnemyAI>(FindObjectsOfType<EnemyAI>());
                }
                Draw(
                    enemiess.Select(e => e.gameObject),
                    localplayer,
                    obj => obj.GetComponent<EnemyAI>().enemyType.enemyName,
                    obj => Color.red,
                    true
                );
            }
            string[] mods = new string[]
            {
                    Toggles.inst.spamhorn ? "Spam horn" : null,
                    Toggles.inst.t_fly ? "Velocity fly" : null,
                    Toggles.inst.t_nodark ? "No dark/nightvision" : null,
                    Toggles.inst.t_infammo ? "Inf shotgun ammo" : null,
                    Toggles.inst.t_infhealth ? "Inf health" : null,
                    Toggles.inst.t_infstam ? "Inf stamina" : null,
                    Toggles.inst.t_infcharge ? "Inf battery charge" : null,
                    Toggles.inst.t_chargeany ? "Charge anything" : null,
                    Toggles.inst.t_noweight ? "No item weight" : null,
                    Toggles.inst.t_strong ? "Strong" : null,
                    Toggles.inst.t_fastheal ? "Fast heal" : null,
                    Toggles.inst.t_infreach ? "Inf reach" : null,
                    Toggles.inst.t_esp ? "Enemy ESP" : null,
                    Toggles.inst.t_pesp ? "Player ESP" : null,
                    Toggles.inst.t_tesp ? "Item ESP" : null,
                    Toggles.inst.t_ttesp ? "Turret ESP" : null,
                    Toggles.inst.t_lesp ? "Landmine ESP" : null,
                    Toggles.inst.t_time ? "Better timer" : null,
            };

            string enableds = string.Join("\n", mods.Where(name => name != null));
            if (!Toggles.inst.t_time)
            {
                GUI.Label(new Rect(0, 10, UnityEngine.Screen.width, 200), $"PAxLM - created by crimsonh - version 1.0.2\n - {fpsbr} fps\n\n{enableds}", header);
            }
            else
            {
                //currentDayTime
                GUI.Label(new Rect(0, 10, UnityEngine.Screen.width, 200), $"PAxLM - created by crimsonh - version 1.0.2\n{TimeOfDay.Instance.currentDayTime}\n - {fpsbr} fps\n\n{enableds}", header);
            }

            if (!show)
                return;
            if (Toggles.inst.w_spawners)
            {
                recttington1 = GUI.Window(1, recttington1, Managers.Spawners, "Enemy spawner", window);
            }
            if (Toggles.inst.w_explorer)
            {
                recttington2 = GUI.Window(2, recttington2, Managers.Explorer, "Scene explorer", window);
            }
            if (Toggles.inst.w_pexplorer)
            {
                recttington3 = GUI.Window(3, recttington3, Managers.PrefabExplorer, "Prefab explorer", window);
            }
            if (Toggles.inst.w_enemies)
            {
                recttington4 = GUI.Window(4, recttington4, Managers.EnemiesWindow, "Enemies", window);
            }
            if (Toggles.inst.w_items)
            {
                recttington5 = GUI.Window(5, recttington5, Managers.Items, "Items", window);
            }
            if (Toggles.inst.w_players)
            {
                recttington6 = GUI.Window(6, recttington6, Managers.Players, "Players", window);
            }
            if (Toggles.inst.w_moons)
            {
                recttington7 = GUI.Window(7, recttington7, Managers.Moons, "Moons", window);
            }
            if  (Toggles.inst.w_ispawner)
            {
                recttington8 = GUI.Window(8, recttington8, Managers.ItemSpawner, "Item spawner", window);
            }

            if (Toggles.inst.w_landmine)
            {
                recttington9 = GUI.Window(9, recttington9, Managers.Landmine, "Landmines", window);
            }

            RGB(recttington, 4);
            recttington = GUI.Window(0, recttington, ActualWindow, "PAxLM", window);
        }
        //suprisingly actually works
        [ServerRpc(RequireOwnership = false)]
        public void BuyItemsServerRpc(int[] boughtItems)
        {
            foreach (Terminal term in FindObjectsOfType<Terminal>())
            {
                term.groupCredits = int.MaxValue;
                term.orderedItemsFromTerminal.AddRange(boughtItems.ToList());
                term.SyncGroupCreditsClientRpc(int.MaxValue, boughtItems.Length);
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
                    GUILayout.Label("- second release!\n- a LOT was added\n- no idea what works and what doesn't, but some might be host just yadada find out what works yourself.\nif any bugs or such found just reply on the thread", label);
                    break;
                case 1: //self
                    speedd = GUILayout.HorizontalSlider(speedd, 1f, 20f);
                    Toggles.inst.t_speed = GUILayout.Toggle(Toggles.inst.t_speed, $"Change speed ({speedd})");
                    Toggles.inst.t_fastclimb = GUILayout.Toggle(Toggles.inst.t_fastclimb, "Fast climb");
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
                    Toggles.inst.t_hearall = GUILayout.Toggle(Toggles.inst.t_hearall, "Hear all");
                    break;
                case 2: //visuals
                    Toggles.inst.t_esp = GUILayout.Toggle(Toggles.inst.t_esp, "Enemy ESP");
                    Toggles.inst.t_pesp = GUILayout.Toggle(Toggles.inst.t_pesp, "Player ESP");
                    Toggles.inst.t_tesp = GUILayout.Toggle(Toggles.inst.t_tesp, "Item ESP");
                    Toggles.inst.t_ttesp = GUILayout.Toggle(Toggles.inst.t_ttesp, "Turret ESP");
                    Toggles.inst.t_lesp = GUILayout.Toggle(Toggles.inst.t_lesp, "Landmine ESP");
                    break;
                case 3: //misc
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
                            grabbable.SetScrapValue(int.MaxValue);
                        }
                    }

                    bool noshotdelay2 = GUILayout.Button("All items scrap", tabst);
                    if (noshotdelay2)
                    {
                        foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
                        {
                            item.isScrap = true;
                            item.minValue = int.MaxValue;
                            item.maxValue = int.MaxValue;
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
                case 4: //server
                    scrolling = GUILayout.BeginScrollView(scrolling);
                    Toggles.inst.t_tvspz = GUILayout.Toggle(Toggles.inst.t_tvspz, "TV spaz");
                    Toggles.inst.t_spazlights = GUILayout.Toggle(Toggles.inst.t_spazlights, "Spaz lights");
                    Toggles.inst.spamhorn = GUILayout.Toggle(Toggles.inst.spamhorn, "Spam horn");
                    if (Toggles.inst.spamhorn)
                    {
                        foreach (ShipAlarmCord horn in FindObjectsOfType<ShipAlarmCord>())
                        {
                            horn.HoldCordDown();
                        }
                    }
                    //no work :cry:
                    /*Toggles.inst.t_rigspam = GUILayout.Toggle(Toggles.inst.t_rigspam, "Rig/player spam");
                    if (Toggles.inst.t_rigspam)
                    {
                        foreach (NetworkObject obj in Resources.FindObjectsOfTypeAll<NetworkObject>())
                        {
                            if (obj.name.Contains("Player") && !obj.IsSpawned)
                            {
                                obj.Spawn();
                            }
                        }
                    }*/

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

                    bool spawnallenemies = GUILayout.Button("Spawn all enemies <color=red>(HOST)</color>", tabst);
                    if (spawnallenemies)
                    {
                        foreach (EnemyAI enemy in Resources.FindObjectsOfTypeAll<EnemyAI>())
                        {
                            GameObject obj = Instantiate(enemy.gameObject, GameNetworkManager.Instance.localPlayerController.transform.position + new Vector3(2f, 0f, 2f), Quaternion.identity);
                            obj.GetComponent<NetworkObject>().Spawn();
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

                    bool endgame = GUILayout.Button("End game <color=red>(HOST)</color>", tabst);
                    if (endgame)
                    {
                        StartOfRound.Instance.inShipPhase = true;
                        StartOfRound.Instance.firingPlayersCutsceneRunning = false;
                        StartOfRound.Instance.ManuallyEjectPlayersServerRpc();
                    }

                    bool maybwork2 = GUILayout.Button("Break server <color=red>(HOST)</color>", tabst);
                    if (maybwork2)
                    {
                        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
                        {
                            var netObj = obj.GetComponent<NetworkObject>();
                            if (netObj == null)
                            {
                                netObj = obj.AddComponent<NetworkObject>();
                            }

                            if (!netObj.IsSpawned)
                            {
                                try
                                {
                                    netObj.Spawn();
                                }
                                catch (Exception e)
                                {
                                }
                            }
                        }
                    }

                    if (GUILayout.Button("Spawn meteor <color=red>(HOST)</color>", tabst))
                    {
                        MeteorShowers meteor = FindObjectOfType<MeteorShowers>();
                        GameObject obj = Instantiate(meteor.meteorPrefab.gameObject, localplayer.gameObject.transform.position, Quaternion.identity);
                        obj.GetComponent<NetworkObject>().Spawn();
                    }

                    bool tpallthings = GUILayout.Button("Teleport all items", tabst);
                    if (tpallthings)
                    {
                        foreach (GrabbableObject grabbable in FindObjectsOfType<GrabbableObject>())
                        {
                            grabbable.targetFloorPosition = GameNetworkManager.Instance.localPlayerController.gameObject.transform.position;
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

                    messagetosay = GUILayout.TextField(messagetosay);
                    bool makeallmessage = GUILayout.Button("Make all message", tabst);
                    if (makeallmessage)
                    {
                        foreach (HUDManager hud in FindObjectsOfType<HUDManager>())
                        {
                            for (int i = 0; i < 60; i++)
                            {
                                hud.AddTextToChatOnServer(messagetosay, playerId: i);
                            }
                        }
                    }

                    signalmessage = GUILayout.TextField(signalmessage);

                    bool signaltranslator = GUILayout.Button("Send signal", tabst);
                    if (signaltranslator)
                    {
                        foreach (HUDManager hud in FindObjectsOfType<HUDManager>())
                        {
                            hud.UseSignalTranslatorServerRpc(signalmessage);
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

                    bool setalllevel = GUILayout.Button("Set everyones level boss", tabst);
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

                    bool endgame2 = GUILayout.Button("End game 2", tabst);
                    if (endgame2)
                    {
                        StartOfRound.Instance.EndOfGameClientRpc(40, 50, StartOfRound.Instance.connectedPlayersAmount, StartOfRound.Instance.scrapCollectedLastRound);
                    }

                    bool endgame3 = GUILayout.Button("End game 3", tabst);
                    if (endgame3)
                    {
                        StartOfRound.Instance.EndGameServerRpc(0);
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

                    if (GUILayout.Button("Unlock all doors", tabst))
                    {
                        foreach (DoorLock land in FindObjectsOfType<DoorLock>())
                        {
                            land.UnlockDoorServerRpc();
                        }
                    }


                    if (GUILayout.Button("Open all doors", tabst))
                    {
                        foreach (DoorLock land in FindObjectsOfType<DoorLock>())
                        {
                            land.OpenDoorAsEnemyServerRpc();
                        }
                    }

                    if (GUILayout.Button("Close all doors", tabst))
                    {
                        foreach (DoorLock land in FindObjectsOfType<DoorLock>())
                        {
                            land.CloseDoorNonPlayerServerRpc();
                        }
                    }

                    if (GUILayout.Button("Explode all landmines", tabst))
                    {
                        foreach (Landmine land in FindObjectsOfType<Landmine>())
                        {
                            land.ExplodeMineServerRpc();
                        }
                    }

                    if (GUILayout.Button("Open all giftboxes", tabst))
                    {
                        foreach (GiftBoxItem gift in FindObjectsOfType<GiftBoxItem>())
                        {
                            gift.OpenGiftBoxServerRpc();
                        }
                    }

                    if (GUILayout.Button("Create mimics <color=red>(MASK NEEDED + HOST)</color>", tabst))
                    {
                        foreach (HauntedMaskItem mask in FindObjectsOfType<HauntedMaskItem>())
                        {
                            mask.CreateMimicServerRpc(false, GameNetworkManager.Instance.localPlayerController.gameObject.transform.position);
                        }
                    }
                    GUILayout.EndScrollView();
                    break;
                case 5: //settings
                    Toggles.inst.t_time = GUILayout.Toggle(Toggles.inst.t_time, "Better time");
                    GUILayout.Label("DEBUG:");
                    bool maybwork22 = GUILayout.Button("Break lobby/break everyones game <color=red>(LEAVE AND REJOIN TO DO IT, working???)</color>", tabst);
                    if (maybwork22)
                    {
                        HUDManager.Instance.DisplayTip("PAxLM", "THIS IS DEBUG!!!! probably won't work.");
                        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
                        {
                            var netObj = obj.GetComponent<NetworkObject>();
                            if (netObj == null)
                            {
                                netObj = obj.AddComponent<NetworkObject>();
                            }

                            if (!netObj.IsSpawned)
                            {
                                try
                                {
                                    netObj.Spawn();
                                }
                                catch (Exception e)
                                {
                                }
                            }
                        }
                    }
                    break;
                case 6: //managers
                    Toggles.inst.w_enemies = GUILayout.Toggle(Toggles.inst.w_enemies, "Enemy manager");
                    Toggles.inst.w_items = GUILayout.Toggle(Toggles.inst.w_items, "Item manager");
                    Toggles.inst.w_players = GUILayout.Toggle(Toggles.inst.w_players, "Player manager");
                    Toggles.inst.w_explorer = GUILayout.Toggle(Toggles.inst.w_explorer, "Scene explorer");
                    Toggles.inst.w_moons = GUILayout.Toggle(Toggles.inst.w_moons, "Moons manager");
                    Toggles.inst.w_pexplorer = GUILayout.Toggle(Toggles.inst.w_pexplorer, "Prefab explorer");
                    Toggles.inst.w_spawners = GUILayout.Toggle(Toggles.inst.w_spawners, "Enemy spawner");
                    Toggles.inst.w_ispawner = GUILayout.Toggle(Toggles.inst.w_ispawner, "Item spawner");
                    Toggles.inst.w_landmine = GUILayout.Toggle(Toggles.inst.w_landmine, "Landmine manager");
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

        public static void Draw(
            IEnumerable<GameObject> objects,
            PlayerControllerB localPlayer,
            System.Func<GameObject, string> labelSelector,
            System.Func<GameObject, Color> colorSelector,
            bool displayDistance = true)
        {
            if (localPlayer == null || localPlayer.gameplayCamera == null) return;

            Camera cam = localPlayer.gameplayCamera;
            Vector3 playerPos = localPlayer.transform.position;

            GUIStyle style = GUI.skin.label;
            GUIContent guiContent = new GUIContent();

            foreach (var obj in objects)
            {
                if (obj == null || !obj.activeInHierarchy) continue;

                Transform objTransform = obj.transform;

                const float maxdist = 250f;
                float distance = Vector3.Distance(playerPos, objTransform.position);
                if (distance > maxdist) continue;

                Vector3 worldPos = objTransform.position + Vector3.up * 2f;
                Vector3 toObj = objTransform.position - cam.transform.position;
                float dotProduct = Vector3.Dot(cam.transform.forward, toObj.normalized);
                if (dotProduct <= 0f) continue;

                if (!WorldToScreen(cam, worldPos, out Vector3 screenPos) || screenPos.z <= 0f)
                    continue;

                string label = labelSelector(obj);
                if (string.IsNullOrEmpty(label)) continue;
                if (char.IsLower(label[0]))
                    label = char.ToUpperInvariant(label[0]) + label.Substring(1);

                if (displayDistance)
                    label += $" - {distance:F1}m -";

                guiContent.text = label;
                Vector2 size = style.CalcSize(guiContent);
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
