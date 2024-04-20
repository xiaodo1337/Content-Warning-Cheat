using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using BepInEx;
using ContentWarningCheat;
using DefaultNamespace;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Zorro.Core;
using Zorro.UI;

namespace ContentWarningCheat
{
    class Win32
    {
        [DllImport("Shell32.dll")]
        public static extern int ShellExecuteA(IntPtr hwnd, StringBuilder lpszOp, StringBuilder lpszFile, StringBuilder lpszParams, StringBuilder lpszDir, int FsShowCmd);
    }
    [BepInPlugin("xiaodo.plugin.ContentWarningCheat", "Cheating!", "1.0")]
    public class HelloWorld : BaseUnityPlugin
    {
        void Start()
        {
            //Bypass Plugin Check
            Traverse.Create(GameHandler.Instance).Field("m_pluginHash").SetValue(null);

            //Check Plugin Update
            CheckUpdate.CheckForUpdate();
        }
        void OnGUI()
        {
            CheckUpdate.DrawHasNewUpdate();
            ESP.StartESP();
            //显示菜单
            if (!DisplayingWindow)
                return;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            windowRect = GUI.Window(114519810, windowRect, WindowFunc, "小豆-内容警告 免费模组禁止倒卖！");
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
                DisplayingWindow = !DisplayingWindow;

            Hack.UpdateData();
            Players.Run();
            Items.Run();
            Misc.Run();
        }
        public bool DisplayingWindow = false;
        public Rect windowRect = new Rect(0, 0, 400, 550);
        private readonly int title_height = 17;
        private Vector2 scrollPosition = Vector2.zero;
        private void WindowFunc(int winId)
        {
            //可拖拽窗口
            GUI.DragWindow(new Rect(0, 0, windowRect.width, title_height));

            //功能栏
            GUILayout.BeginArea(new Rect(0, title_height + 5, windowRect.width, 30));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("玩家"))
                Hack.GUILabel = Hack.Label.Players;
            if (GUILayout.Button("物品"))
                Hack.GUILabel = Hack.Label.Items;
            if (GUILayout.Button("怪物"))
                Hack.GUILabel = Hack.Label.Monsters;
            if (GUILayout.Button("ESP"))
                Hack.GUILabel = Hack.Label.ESP;
            if (GUILayout.Button("杂项"))
                Hack.GUILabel = Hack.Label.Misc;
            GUILayout.EndHorizontal();
            CenterLabel("严厉谴责国内某个拿github开源代码圈钱的傻狗");
            GUILayout.EndArea();

            if (Hack.GUILabel == Hack.Label.Players)
            {
                int x = 0, y = 0;
                GUILayout.BeginArea(new Rect(x, y + (title_height + 35), windowRect.width, windowRect.height - y - (title_height + 35)));
                GUILayout.BeginHorizontal();
                Players.InfinityHealth = GUILayout.Toggle(Players.InfinityHealth, "无限血量");
                Players.InfinityStamina = GUILayout.Toggle(Players.InfinityStamina, "无限体力");
                Players.InfinityOxy = GUILayout.Toggle(Players.InfinityOxy, "无限氧气");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                Players.InfinityJump = GUILayout.Toggle(Players.InfinityJump, "无跳跃间隔");
                Players.NeverFalldown = GUILayout.Toggle(Players.NeverFalldown, "永不摔倒");
                Players.NeverDie = GUILayout.Toggle(Players.NeverDie, "永不死亡");
                GUILayout.EndHorizontal();
                CenterLabel($"奔跑速度倍率: {Math.Round(Players.SprintMultipiler, 2)}x");
                Players.SprintMultipiler = GUILayout.HorizontalSlider(Players.SprintMultipiler, 1f, 20f);
                CenterLabel($"跳跃高度倍率: {Math.Round(Players.JumpHeightMultipiler, 2)}x");
                Players.JumpHeightMultipiler = GUILayout.HorizontalSlider(Players.JumpHeightMultipiler, 1f, 20f);
                if (Player.localPlayer)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("复活"))
                        Players.Respawn(true);
                    if (GUILayout.Button("自杀"))
                        Players.Die(true);
                    if (GUILayout.Button("打胶"))
                        Players.Cum(true);
                    if (GUILayout.Button("摔倒"))
                        Players.Falldown(true);
                    if (GUILayout.Button("进入领域"))
                        Players.JoinRealm(true);
                    if (GUILayout.Button("移出领域"))
                        Players.RemoveRealm(true);
                    GUILayout.EndHorizontal();
                }
                if(Players.InGame.Count > 0)
                {
                    CenterLabel("执行对象");
                    GUILayout.BeginHorizontal();
                    foreach (var playerkvp in new Dictionary<Player, bool>(Players.InGame)) // 使用复制的集合进行迭代, 不然会报错
                    {
                        Players.InGame[playerkvp.Key] = GUILayout.Toggle(Players.InGame[playerkvp.Key], playerkvp.Key.refs.view.Owner.NickName);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("复活"))
                        Players.Respawn();
                    if (GUILayout.Button("杀死"))
                        Players.Die();
                    if (GUILayout.Button("打胶"))
                        Players.Cum();
                    if (GUILayout.Button("摔倒"))
                        Players.Falldown();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("蹦蹦炸弹"))
                        Players.Explode();
                    if (GUILayout.Button("鼓掌"))
                        Players.PlayEmote(43);
                    if (GUILayout.Button("竖中指"))
                        Players.PlayEmote(52);
                    if (GUILayout.Button("俯卧撑"))
                        Players.PlayEmote(54);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("朝自己飞过来"))
                        Players.DragToLocal();
                    if (GameObject.FindObjectOfType<PlayerCustomizer>() && Players.GetChoosedPlayerCount() == 1 && GUILayout.Button("强制进入终端"))
                        Players.ForceEnterTerminal();
                    if (GUILayout.Button("进入领域"))
                        Players.JoinRealm();
                    if (GUILayout.Button("移出领域"))
                        Players.RemoveRealm();
                    GUILayout.EndHorizontal();
                }

                CenterLabel("严厉谴责国内某个拿github开源代码圈钱的傻狗");
                GUILayout.EndArea();
            }
            if (Hack.GUILabel == Hack.Label.Items)
            {
                int x = 0, y = 0;
                GUILayout.BeginArea(new Rect(x, y + (title_height + 35), windowRect.width, windowRect.height - y - (title_height + 35)));

                CenterLabel("生成方式");
                Items.SpawnMethod = (Items.SpawnType)GUILayout.Toolbar((int)Items.SpawnMethod, new string[] { "添加至物品栏", "在面前创建", "召唤无人机生成" });
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                GUILayout.BeginHorizontal();
                Items.PartyMode = GUILayout.Toggle(Items.PartyMode, "无限派对烟花");
                Items.InfinityPower = GUILayout.Toggle(Items.InfinityPower, "无限电量");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                Items.BlockBombSpawn = GUILayout.Toggle(Items.BlockBombSpawn, "阻止炸弹生成");
                Items.NoGrabberLimit = GUILayout.Toggle(Items.NoGrabberLimit, "取消夹子抓取限制");
                GUILayout.EndHorizontal();
                if (Player.localPlayer)
                {
                    CenterLabel("物品操作");
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("清除所有物品"))
                        Items.DestoryDropedItems();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("生成所有表情书籍"))
                        Items.SpawnEmoteBooks();
                    if (GUILayout.Button("生成所有物品"))
                        Items.SpawnAllItems();
                    GUILayout.EndHorizontal();

                    int count = 0;
                    //灯光
                    CenterLabel("灯光");
                    foreach (KeyValuePair<byte, string> item in Items.ItemsTypeList[Items.ItemType.Lights])
                    {
                        if (count == 0)
                            GUILayout.BeginHorizontal();

                        if (GUILayout.Button(item.Value))
                        {
                            switch (Items.SpawnMethod)
                            {
                                case Items.SpawnType.AddToInventory:
                                    Items.SpawnItem(item.Key);
                                    break;

                                case Items.SpawnType.CreatePickup:
                                    Items.SpawnItem(new byte[] { item.Key }, UsefulFuncs.GetCrosshairPosition(MaxDistance: 1.5f));
                                    break;

                                case Items.SpawnType.CallDrone:
                                    Items.SpawnItem(new byte[] { item.Key });
                                    break;
                            }
                        }

                        count++;

                        if (count >= 3)
                        {
                            GUILayout.EndHorizontal();
                            count = 0;
                        }
                    }
                    if (count > 0)
                    {
                        GUILayout.EndHorizontal();
                        count = 0;
                    }

                    //医疗用具
                    CenterLabel("医疗用具");
                    foreach (KeyValuePair<byte, string> item in Items.ItemsTypeList[Items.ItemType.Medicals])
                    {
                        if (count == 0)
                            GUILayout.BeginHorizontal();

                        if (GUILayout.Button(item.Value))
                        {
                            switch (Items.SpawnMethod)
                            {
                                case Items.SpawnType.AddToInventory:
                                    Items.SpawnItem(item.Key);
                                    break;

                                case Items.SpawnType.CreatePickup:
                                    Items.SpawnItem(new byte[] { item.Key }, UsefulFuncs.GetCrosshairPosition(MaxDistance: 1.5f));
                                    break;

                                case Items.SpawnType.CallDrone:
                                    Items.SpawnItem(new byte[] { item.Key });
                                    break;
                            }
                        }

                        count++;

                        if (count >= 3)
                        {
                            GUILayout.EndHorizontal();
                            count = 0;
                        }
                    }
                    if (count > 0)
                    {
                        GUILayout.EndHorizontal();
                        count = 0;
                    }

                    //工具
                    CenterLabel("工具");
                    foreach (KeyValuePair<byte, string> item in Items.ItemsTypeList[Items.ItemType.Tools])
                    {
                        if (count == 0)
                            GUILayout.BeginHorizontal();

                        if (GUILayout.Button(item.Value))
                        {
                            switch (Items.SpawnMethod)
                            {
                                case Items.SpawnType.AddToInventory:
                                    Items.SpawnItem(item.Key);
                                    break;

                                case Items.SpawnType.CreatePickup:
                                    Items.SpawnItem(new byte[] { item.Key }, UsefulFuncs.GetCrosshairPosition(MaxDistance: 1.5f));
                                    break;

                                case Items.SpawnType.CallDrone:
                                    Items.SpawnItem(new byte[] { item.Key });
                                    break;
                            }
                        }

                        count++;

                        if (count >= 3)
                        {
                            GUILayout.EndHorizontal();
                            count = 0;
                        }
                    }
                    if (count > 0)
                    {
                        GUILayout.EndHorizontal();
                        count = 0;
                    }

                    //表情
                    CenterLabel("表情");
                    foreach (KeyValuePair<byte, string> item in Items.ItemsTypeList[Items.ItemType.Emotes])
                    {
                        if (count == 0)
                            GUILayout.BeginHorizontal();

                        if (GUILayout.Button(item.Value))
                        {
                            switch (Items.SpawnMethod)
                            {
                                case Items.SpawnType.AddToInventory:
                                    Items.SpawnItem(item.Key);
                                    break;

                                case Items.SpawnType.CreatePickup:
                                    Items.SpawnItem(new byte[] { item.Key }, UsefulFuncs.GetCrosshairPosition(MaxDistance: 1.5f));
                                    break;

                                case Items.SpawnType.CallDrone:
                                    Items.SpawnItem(new byte[] { item.Key });
                                    break;
                            }
                        }

                        count++;

                        if (count >= 3)
                        {
                            GUILayout.EndHorizontal();
                            count = 0;
                        }
                    }
                    if (count > 0)
                    {
                        GUILayout.EndHorizontal();
                        count = 0;
                    }

                    //杂项
                    CenterLabel("杂项");
                    foreach (KeyValuePair<byte, string> item in Items.ItemsTypeList[Items.ItemType.Miscs])
                    {
                        if (count == 0)
                            GUILayout.BeginHorizontal();

                        if (GUILayout.Button(item.Value))
                        {
                            switch (Items.SpawnMethod)
                            {
                                case Items.SpawnType.AddToInventory:
                                    Items.SpawnItem(item.Key);
                                    break;

                                case Items.SpawnType.CreatePickup:
                                    Items.SpawnItem(new byte[] { item.Key }, UsefulFuncs.GetCrosshairPosition(MaxDistance: 1.5f));
                                    break;

                                case Items.SpawnType.CallDrone:
                                    Items.SpawnItem(new byte[] { item.Key });
                                    break;
                            }
                        }

                        count++;

                        if (count >= 3)
                        {
                            GUILayout.EndHorizontal();
                            count = 0;
                        }
                    }
                    if (count > 0)
                    {
                        GUILayout.EndHorizontal();
                        count = 0;
                    }

                    //其他
                    CenterLabel("其他");
                    foreach (KeyValuePair<byte, string> item in Items.ItemsTypeList[Items.ItemType.Others])
                    {
                        if (count == 0)
                            GUILayout.BeginHorizontal();

                        if (GUILayout.Button(item.Value))
                        {
                            switch (Items.SpawnMethod)
                            {
                                case Items.SpawnType.AddToInventory:
                                    Items.SpawnItem(item.Key);
                                    break;

                                case Items.SpawnType.CreatePickup:
                                    Items.SpawnItem(new byte[] { item.Key }, UsefulFuncs.GetCrosshairPosition(MaxDistance: 1.5f));
                                    break;

                                case Items.SpawnType.CallDrone:
                                    Items.SpawnItem(new byte[] { item.Key });
                                    break;
                            }
                        }

                        count++;

                        if (count >= 3)
                        {
                            GUILayout.EndHorizontal();
                            count = 0;
                        }
                    }
                    if (count > 0)
                    {
                        GUILayout.EndHorizontal();
                        count = 0;
                    }
                }
                CenterLabel("严厉谴责国内某个拿github开源代码圈钱的傻狗");
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            if (Hack.GUILabel == Hack.Label.Monsters)
            {
                int x = 0, y = 0;
                GUILayout.BeginArea(new Rect(x, y + (title_height + 35), windowRect.width, windowRect.height - y - (title_height + 35)));
                if (Player.localPlayer)
                {
                    CenterLabel("功能");
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("苏醒"))
                        Monsters.ReviveAll();
                    if (GUILayout.Button("杀死"))
                        Monsters.KillAll();
                    if (GUILayout.Button("生成炸弹"))
                        Monsters.Explode();
                    if (GUILayout.Button("摔倒"))
                        Monsters.Falldown();
                    if (GUILayout.Button("打胶"))
                        Monsters.Cum();
                    if (GUILayout.Button("拖拽到身边"))
                        Monsters.DragToLocal();
                    GUILayout.EndHorizontal();
                    if (GUILayout.Button("让狗叫怪狗叫"))
                        Monsters.MakeMouthesScream();
                    if (GUILayout.Button("清除所有怪物"))
                        Monsters.ClearAllMonsters();

                    CenterLabel("生成");
                    int count = 0;
                    foreach (string name in Monsters.MonsterNames)
                    {
                        if (count == 0)
                            GUILayout.BeginHorizontal();

                        if (GUILayout.Button(name))
                            Monsters.SpawnMonster(name);

                        count++;

                        if (count >= 2)
                        {
                            GUILayout.EndHorizontal();
                            count = 0;
                        }
                    }

                    if (count > 0)
                        GUILayout.EndHorizontal();
                }
                else CenterLabel("必须在游戏内才能使用此功能！");
                CenterLabel("严厉谴责国内某个拿github开源代码圈钱的傻狗");
                GUILayout.EndArea();
            }
            if (Hack.GUILabel == Hack.Label.ESP)
            {
                int x = 0, y = 0;
                GUILayout.BeginArea(new Rect(x, y + (title_height + 35), windowRect.width, windowRect.height - y - (title_height + 35)));
                GUILayout.BeginHorizontal();
                ESP.EnablePlayerESP = GUILayout.Toggle(ESP.EnablePlayerESP, "玩家");
                ESP.EnableMonsterESP = GUILayout.Toggle(ESP.EnableMonsterESP, "怪物");
                ESP.EnableItemESP = GUILayout.Toggle(ESP.EnableItemESP, "物品");
                ESP.EnableDivingBellESP = GUILayout.Toggle(ESP.EnableDivingBellESP, "潜艇");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                ESP.EnableDrawLine = GUILayout.Toggle(ESP.EnableDrawLine, "射线");
                ESP.EnableDrawString = GUILayout.Toggle(ESP.EnableDrawString, "名字");
                ESP.EnableDistance = GUILayout.Toggle(ESP.EnableDistance, "距离");
                GUILayout.EndHorizontal();
                CenterLabel("严厉谴责国内某个拿github开源代码圈钱的傻狗");
                GUILayout.EndArea();
            }
            if (Hack.GUILabel == Hack.Label.Misc)
            {
                int x = 0, y = 0;
                GUILayout.BeginArea(new Rect(x, y + (title_height + 35), windowRect.width, windowRect.height - y - (title_height + 35)));

                CenterLabel("大厅");
                GUILayout.BeginHorizontal();
                Misc.AutoJoinRandom = GUILayout.Toggle(Misc.AutoJoinRandom, "自动快速游戏");
                Misc.ForceJoinOthersRoom = GUILayout.Toggle(Misc.ForceJoinOthersRoom, "强制加入他人房间");
                GUILayout.EndHorizontal();
                Misc.DisableAutoJoinRandomWhenJoined = GUILayout.Toggle(Misc.DisableAutoJoinRandomWhenJoined, "加入游戏后关闭自动快速游戏选项");
                if (GUILayout.Button("创建多人公开房间") && MainMenuHandler.Instance)
                    MainMenuHandler.Instance.SilentHost();

                CenterLabel("其他");
                if (GUILayout.Button("Dump Items List To Console"))
                    Items.DumpItemsToConsole();

                CenterLabel("严厉谴责国内某个拿github开源代码圈钱的傻狗");
                GUILayout.EndArea();
            }
        }
        void CenterLabel(string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(label);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
    internal class Hack
    {
        private static float NextUpdate = 0f;
        private static readonly float UpdateInterval = 1.0f;
        public static Label GUILabel = Label.Misc;
        public enum Label
        {
            Items = 0,
            Monsters,
            ESP,
            Misc,
            Players
        }
        public static void UpdateData()
        {
            if (Time.time >= NextUpdate)
            {
                NextUpdate = Time.time + UpdateInterval;
                if (Player.localPlayer == null)
                    return;

                if(ESP.EnablePlayerESP) ESP.PlayersList = GameObject.FindObjectsOfType<Player>();
                if(ESP.EnableItemESP) ESP.PickupsList = GameObject.FindObjectsOfType<Pickup>();
                if(ESP.EnableMonsterESP) ESP.BotsList = GameObject.FindObjectsOfType<Bot>();
                if(ESP.EnableDivingBellESP) ESP.DivingBellsList = GameObject.FindObjectsOfType<UseDivingBellButton>();
                foreach (Player __player in GameObject.FindObjectsOfType<Player>())
                {
                    if (__player.ai || __player.IsLocal || Players.InGame.ContainsKey(__player))
                        continue;
                    Players.InGame.Add(__player, false);
                }
                foreach (KeyValuePair<Player, bool> keyValuePair in Players.InGame)
                {
                    if (keyValuePair.Key != null)
                        continue;
                    Players.InGame.Remove(keyValuePair.Key);
                }
                Debug.Log("Update Lists");
            }
        }
    }
}
