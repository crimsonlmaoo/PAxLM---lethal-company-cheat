using PAxLM.Helpers;
using PAxLM.Windows;
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
using PAxLM.Toggles;

namespace PAxLM
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
        List<DoorLock> doors = null;

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
        int value = 1;
        float updateint = 1f;
        float lastupdate;

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

        private string GetTime()
        {
            string newLine;
            int num = (int)(TimeOfDay.Instance.normalizedTimeOfDay * (60f * TimeOfDay.Instance.numberOfHours)) + 360;
            int num2 = (int)Mathf.Floor(num / 60);

            if (num2 > 12)
            {
                num2 %= 12;
            }

            int num3 = num % 60;
            string text = $"{num2:00}:{num3:00}".TrimStart('0');
            return text;
        }
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
            foreach (var field in typeof(Toggles.Toggles).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                if (field.FieldType == typeof(bool))
                {
                    bool toggleValue = (bool)field.GetValue(Toggles.Toggles.inst);
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

            if (Toggles.Toggles.inst.t_hearall)
            {
                foreach (PlayerControllerB players in FindObjectsOfType<PlayerControllerB>())
                {
                    players.currentVoiceChatAudioSource.minDistance = 0f;
                    players.currentVoiceChatAudioSource.maxDistance = float.MaxValue;
                }
            }

            if (Toggles.Toggles.inst.t_spazlights)
            {
                ShipLights[] lights = FindObjectsOfType<ShipLights>();
                foreach (ShipLights light in lights)
                {
                    light.SetShipLightsServerRpc(false);
                    Task.Delay(1000);
                    light.SetShipLightsServerRpc(true);
                }
            }

            if (Toggles.Toggles.inst.t_tvspz)
            {
                foreach (TVScript tVScript in FindObjectsOfType<TVScript>())
                {
                    tVScript.TurnOffTVServerRpc();
                    tVScript.TurnOnTVServerRpc();
                }
            }

            if (Toggles.Toggles.inst.t_fly)
            {
                if (Keyboard.current.wKey.isPressed)
                {
                    localplayer.playerRigidbody.AddForce(localplayer.gameplayCamera.transform.forward * 10f, ForceMode.VelocityChange);
                }
            }

            if (Toggles.Toggles.inst.t_fastclimb)
            {
                localplayer.climbSpeed = 99f;
            }

            if (Toggles.Toggles.inst.t_infstam)
            {
                localplayer.sprintMeter = 1f;
                if (localplayer.sprintMeterUI != null)
                    localplayer.sprintMeterUI.fillAmount = 1f;
            }

            if (Toggles.Toggles.inst.t_infhealth)
            {
                localplayer.health = int.MaxValue;
            }

            if (Toggles.Toggles.inst.t_infcharge && localplayer.currentlyHeldObjectServer?.insertedBattery != null)
            {
                localplayer.currentlyHeldObjectServer.insertedBattery.charge = 1;
                localplayer.currentlyHeldObjectServer.insertedBattery.empty = false;
            }

            if (Toggles.Toggles.inst.t_chargeany)
            {
                localplayer.currentlyHeldObject.itemProperties.requiresBattery = true;
            }

            if (Toggles.Toggles.inst.t_fastheal)
            {
                localplayer.healthRegenerateTimer = 0.1f;
            }

            if (Toggles.Toggles.inst.t_infreach)
            {
                localplayer.grabDistance = int.MaxValue;
            }

            if (Toggles.Toggles.inst.t_noweight)
            {
                localplayer.currentlyHeldObjectServer.itemProperties.weight = 0f;
                localplayer.carryWeight = 0f;
            }

            if (Toggles.Toggles.inst.t_strong)
            {
                localplayer.currentlyHeldObjectServer.itemProperties.twoHanded = false;
                localplayer.currentlyHeldObjectServer.itemProperties.twoHandedAnimation = false;
            }

            if (Toggles.Toggles.inst.t_infammo)
            {
                localplayer.currentlyHeldObjectServer.GetComponent<ShotgunItem>().shellsLoaded = int.MaxValue;
            }

            if (Toggles.Toggles.inst.t_nodark)
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
                thhumb.onActive.background = Tex(10, 10, Color.red);
                thhumb.fixedWidth = 20;

                GUI.skin.horizontalSlider = slide;
                GUI.skin.horizontalSliderThumb = thhumb;
            }

            if (Toggles.Toggles.inst.t_speed)
            {
                 localplayer.movementSpeed = speedd;
            }

            if (Time.time - lastupdate >= updateint)
            {
                lastupdate = Time.time;

                if (Toggles.Toggles.inst.t_lesp)
                    landmines = new List<Landmine>(FindObjectsOfType<Landmine>());

                if (Toggles.Toggles.inst.t_ttesp)
                    turrets = new List<Turret>(FindObjectsOfType<Turret>());

                if (Toggles.Toggles.inst.t_tesp)
                    grabbabless = new List<GrabbableObject>(FindObjectsOfType<GrabbableObject>());

                if (Toggles.Toggles.inst.t_pesp)
                    playerss = new List<PlayerControllerB>(FindObjectsOfType<PlayerControllerB>());

                if (Toggles.Toggles.inst.t_esp)
                    enemiess = new List<EnemyAI>(FindObjectsOfType<EnemyAI>());

                if (Toggles.Toggles.inst.t_desp)
                    doors = new List<DoorLock>(FindObjectsOfType<DoorLock>());
            }

            if (Toggles.Toggles.inst.t_lesp)
            {
                Draw(
                    landmines.Select(e => e.gameObject),
                    localplayer,
                    obj => "Landmine",
                    obj => Color.red,
                    true
                );
            }

            if (Toggles.Toggles.inst.t_ttesp)
            {
                Draw(
                    turrets.Select(e => e.gameObject),
                    localplayer,
                    obj => "Turret",
                    obj => Color.red,
                    true
                );
            }

            if (Toggles.Toggles.inst.t_tesp)
            {
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

            if (Toggles.Toggles.inst.t_pesp)
            {
                Draw(
                    playerss.Select(e => e.gameObject),
                    localplayer,
                    obj => obj.GetComponent<PlayerControllerB>().playerUsername,
                    obj => Color.blue,
                    true
                );
            }

            if (Toggles.Toggles.inst.t_esp)
            {
                Draw(
                    enemiess.Select(e => e.gameObject),
                    localplayer,
                    obj => obj.GetComponent<EnemyAI>().enemyType.enemyName,
                    obj => Color.red,
                    true
                );
            }

            if (Toggles.Toggles.inst.t_desp)
            {
                Draw(
                    doors.Select(e => e.gameObject),
                    localplayer,
                    obj => "Door",
                    obj => Color.blue,
                    true
                );
            }
            string[] mods = new string[]
            {
                    Toggles.Toggles.inst.spamhorn ? "Spam horn" : null,
                    Toggles.Toggles.inst.t_fly ? "Velocity fly" : null,
                    Toggles.Toggles.inst.t_nodark ? "No dark/nightvision" : null,
                    Toggles.Toggles.inst.t_infammo ? "Inf shotgun ammo" : null,
                    Toggles.Toggles.inst.t_infhealth ? "Inf health" : null,
                    Toggles.Toggles.inst.t_infstam ? "Inf stamina" : null,
                    Toggles.Toggles.inst.t_infcharge ? "Inf battery charge" : null,
                    Toggles.Toggles.inst.t_chargeany ? "Charge anything" : null,
                    Toggles.Toggles.inst.t_noweight ? "No item weight" : null,
                    Toggles.Toggles.inst.t_strong ? "Strong" : null,
                    Toggles.Toggles.inst.t_fastheal ? "Fast heal" : null,
                    Toggles.Toggles.inst.t_infreach ? "Inf reach" : null,
                    Toggles.Toggles.inst.t_esp ? "Enemy ESP" : null,
                    Toggles.Toggles.inst.t_pesp ? "Player ESP" : null,
                    Toggles.Toggles.inst.t_tesp ? "Item ESP" : null,
                    Toggles.Toggles.inst.t_ttesp ? "Turret ESP" : null,
                    Toggles.Toggles.inst.t_desp ? "Door ESP" : null,
                    Toggles.Toggles.inst.t_lesp ? "Landmine ESP" : null,
                    Toggles.Toggles.inst.t_time ? "Better timer" : null,
            };

            string enableds = string.Join("\n", mods.Where(name => name != null));
            if (!Toggles.Toggles.inst.t_time)
            {
                GUI.Label(new Rect(0, 10, UnityEngine.Screen.width, 200), $"PAxLM - created by crimsonh - version 1.0.3\n - {fpsbr} fps\n\n{enableds}", header);
            }
            else
            {
                //currentDayTime
                GUI.Label(new Rect(0, 10, UnityEngine.Screen.width, 200), $"PAxLM - created by crimsonh - version 1.0.3\n{GetTime()}\n - {fpsbr} fps\n\n{enableds}", header);
            }

            if (!show)
                return;
            if (Toggles.Toggles.inst.w_spawners)
            {
                recttington1 = GUI.Window(1, recttington1, Managers.Spawners, "Enemy spawner", window);
            }
            if (Toggles.Toggles.inst.w_explorer)
            {
                recttington2 = GUI.Window(2, recttington2, Managers.Explorer, "Scene explorer", window);
            }
            if (Toggles.Toggles.inst.w_pexplorer)
            {
                recttington3 = GUI.Window(3, recttington3, Managers.PrefabExplorer, "Prefab explorer", window);
            }
            if (Toggles.Toggles.inst.w_enemies)
            {
                recttington4 = GUI.Window(4, recttington4, Managers.EnemiesWindow, "Enemies", window);
            }
            if (Toggles.Toggles.inst.w_items)
            {
                recttington5 = GUI.Window(5, recttington5, Managers.Items, "Items", window);
            }
            if (Toggles.Toggles.inst.w_players)
            {
                recttington6 = GUI.Window(6, recttington6, Managers.Players, "Players", window);
            }
            if (Toggles.Toggles.inst.w_moons)
            {
                recttington7 = GUI.Window(7, recttington7, Managers.Moons, "Moons", window);
            }
            if  (Toggles.Toggles.inst.w_ispawner)
            {
                recttington8 = GUI.Window(8, recttington8, Managers.ItemSpawner, "Item spawner", window);
            }

            if (Toggles.Toggles.inst.w_landmine)
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
                    GUILayout.Label("- v1.0.3!\n- again, if things don't work PLEASE send them over to the thread and reply with the things!\n- if able to, please take a look at the source and give me some feedback on what to do better!", label);
                    break;
                case 1: //self
                    speedd = GUILayout.HorizontalSlider(speedd, 1f, 20f);
                    GUILayout.Label("speed: " + speedd);
                    Toggles.Toggles.inst.t_speed = GUILayout.Toggle(Toggles.Toggles.inst.t_speed, $"Change speed");
                    Toggles.Toggles.inst.t_fastclimb = GUILayout.Toggle(Toggles.Toggles.inst.t_fastclimb, "Fast climb");
                    //Toggles.Toggles.inst.t_fly = GUILayout.Toggle(Toggles.Toggles.inst.t_fly, "Velocity fly"); not gonna fix this
                    Toggles.Toggles.inst.t_nodark = GUILayout.Toggle(Toggles.Toggles.inst.t_nodark, "No dark/nightvision");
                    Toggles.Toggles.inst.t_infammo = GUILayout.Toggle(Toggles.Toggles.inst.t_infammo, "Inf shotgun ammo");
                    Toggles.Toggles.inst.t_infhealth = GUILayout.Toggle(Toggles.Toggles.inst.t_infhealth, "Inf health");
                    Toggles.Toggles.inst.t_infstam = GUILayout.Toggle(Toggles.Toggles.inst.t_infstam, "Inf stamina");
                    Toggles.Toggles.inst.t_infcharge = GUILayout.Toggle(Toggles.Toggles.inst.t_infcharge, "Inf battery charge");
                    Toggles.Toggles.inst.t_chargeany = GUILayout.Toggle(Toggles.Toggles.inst.t_chargeany, "Charge anything");
                    //Toggles.Toggles.inst.t_noweight = GUILayout.Toggle(Toggles.Toggles.inst.t_noweight, "No item weight"); breaks YOUR weight for some reason
                    //Toggles.Toggles.inst.t_strong = GUILayout.Toggle(Toggles.Toggles.inst.t_strong, "Strong"); why no work :(
                    Toggles.Toggles.inst.t_fastheal = GUILayout.Toggle(Toggles.Toggles.inst.t_fastheal, "Fast heal");
                    Toggles.Toggles.inst.t_infreach = GUILayout.Toggle(Toggles.Toggles.inst.t_infreach, "Inf reach");
                    Toggles.Toggles.inst.t_hearall = GUILayout.Toggle(Toggles.Toggles.inst.t_hearall, "Hear all");
                    break;
                case 2: //visuals
                    Toggles.Toggles.inst.t_esp = GUILayout.Toggle(Toggles.Toggles.inst.t_esp, "Enemy ESP");
                    Toggles.Toggles.inst.t_pesp = GUILayout.Toggle(Toggles.Toggles.inst.t_pesp, "Player ESP");
                    Toggles.Toggles.inst.t_tesp = GUILayout.Toggle(Toggles.Toggles.inst.t_tesp, "Item ESP");
                    Toggles.Toggles.inst.t_ttesp = GUILayout.Toggle(Toggles.Toggles.inst.t_ttesp, "Turret ESP");
                    Toggles.Toggles.inst.t_lesp = GUILayout.Toggle(Toggles.Toggles.inst.t_lesp, "Landmine ESP");
                    Toggles.Toggles.inst.t_desp = GUILayout.Toggle(Toggles.Toggles.inst.t_desp, "Door ESP");
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

                    value = (int)GUILayout.HorizontalSlider((float)value, 1f, 99999f);
                    GUILayout.Label("value: " + value);

                    bool noshotdelay = GUILayout.Button("Set every grabbables value", tabst);
                    if (noshotdelay)
                    {
                        foreach (GrabbableObject grabbable in FindObjectsOfType<GrabbableObject>())
                        {
                            grabbable.SetScrapValue(value);
                        }
                    }

                    bool buyall = GUILayout.Button("Buy all shop items <color=red>(HOST)</color>", tabst);
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

                    bool makesellguyattack = GUILayout.Button("Force sell guy attack", tabst);
                    if (makesellguyattack)
                    {
                        foreach (DepositItemsDesk shop in FindObjectsOfType<DepositItemsDesk>())
                        {
                            shop.AttackPlayersServerRpc();
                        }
                    }
                    break;
                case 4: //server
                    scrolling = GUILayout.BeginScrollView(scrolling);
                    Toggles.Toggles.inst.t_tvspz = GUILayout.Toggle(Toggles.Toggles.inst.t_tvspz, "TV spaz");
                    //Toggles.Toggles.inst.t_spazlights = GUILayout.Toggle(Toggles.Toggles.inst.t_spazlights, "Spaz lights"); CS
                    Toggles.Toggles.inst.spamhorn = GUILayout.Toggle(Toggles.Toggles.inst.spamhorn, "Spam horn");
                    if (Toggles.Toggles.inst.spamhorn)
                    {
                        foreach (ShipAlarmCord horn in FindObjectsOfType<ShipAlarmCord>())
                        {
                            horn.HoldCordDown();
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
                    bool makeallmessage = GUILayout.Button("Make everyone message", tabst);
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

                    bool storeall = GUILayout.Button("Store all ship objects", tabst);
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

                    if (GUILayout.Button("Break all giftboxes <color=red>(NOT TESTED)</color>", tabst))
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

                    if (GUILayout.Button("Tp random player to another player", tabst))
                    {
                        for (int i = 0; i < StartOfRound.Instance.unlockablesList.unlockables.Count; i++)
                        {
                            var unlockable = StartOfRound.Instance.unlockablesList.unlockables[i];
                            if (unlockable.unlockableName == "Teleporter")
                            {
                                foreach (Terminal term in FindObjectsOfType<Terminal>())
                                    StartOfRound.Instance.BuyShipUnlockableServerRpc(i, term.groupCredits);
                            }
                        }

                        foreach (ShipTeleporter tel in FindObjectsOfType<ShipTeleporter>())
                        {
                            var randomplayer = Stuff.inst.GetRandomPlayer();
                            var randomplayer2 = Stuff.inst.GetRandomPlayer();
                            StartOfRound.Instance.mapScreen.targetedPlayer = randomplayer;
                            tel.gameObject.transform.position = randomplayer2.transform.position + new Vector3(0f, 6f, 0f);
                            tel.teleporterPosition = randomplayer2.transform;
                            tel.teleportOutPosition = randomplayer2.transform;
                            tel.PressTeleportButtonServerRpc();
                        }
                    }

                    GUILayout.EndScrollView();
                    break;
                case 5: //settings
                    Toggles.Toggles.inst.t_time = GUILayout.Toggle(Toggles.Toggles.inst.t_time, "Better time");
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
                    Toggles.Toggles.inst.w_enemies = GUILayout.Toggle(Toggles.Toggles.inst.w_enemies, "Enemy manager");
                    Toggles.Toggles.inst.w_items = GUILayout.Toggle(Toggles.Toggles.inst.w_items, "Item manager");
                    Toggles.Toggles.inst.w_players = GUILayout.Toggle(Toggles.Toggles.inst.w_players, "Player manager");
                    Toggles.Toggles.inst.w_explorer = GUILayout.Toggle(Toggles.Toggles.inst.w_explorer, "Scene explorer");
                    Toggles.Toggles.inst.w_moons = GUILayout.Toggle(Toggles.Toggles.inst.w_moons, "Moons manager");
                    Toggles.Toggles.inst.w_pexplorer = GUILayout.Toggle(Toggles.Toggles.inst.w_pexplorer, "Prefab explorer");
                    Toggles.Toggles.inst.w_spawners = GUILayout.Toggle(Toggles.Toggles.inst.w_spawners, "Enemy spawner");
                    Toggles.Toggles.inst.w_ispawner = GUILayout.Toggle(Toggles.Toggles.inst.w_ispawner, "Item spawner");
                    Toggles.Toggles.inst.w_landmine = GUILayout.Toggle(Toggles.Toggles.inst.w_landmine, "Landmine manager");
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
