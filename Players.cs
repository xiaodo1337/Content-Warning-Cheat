using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.LowLevel;
using static UnityEngine.GraphicsBuffer;

namespace TestUnityPlugin
{
    internal class Players
    {
        public static bool InfinityHealth = false;
        public static bool InfinityOxy = false;
        public static bool InfinityStamina = true;
        public static bool InfinityJump = true;
        public static bool InfinityBattary = true;
        public static bool NeverFalldown = false;
        public static bool NeverDie = false;
        public static float SprintMultipiler = 1f;
        public static float JumpHeightMultipiler = 1f;
        public static Dictionary<Player, bool> InGame = new Dictionary<Player, bool>();
        public static List<Player> InRealm = new List<Player>();
        public static void Run()
        {
            Player player = Player.localPlayer;
            if (player == null)
            {
                InRealm.Clear();
                InGame.Clear();
                return;
            }

            foreach (Player __player in GameObject.FindObjectsOfType<Player>())
            {
                if (__player.ai || __player.IsLocal || InGame.ContainsKey(__player))
                    continue;
                InGame.Add(__player, false);
            }
            foreach (KeyValuePair<Player, bool> keyValuePair in InGame)
            {
                if (keyValuePair.Key != null)
                    continue;
                InGame.Remove(keyValuePair.Key);
            }
            foreach (Player __player in InRealm)
            {
                if (__player != null)
                    continue;
                InRealm.Remove(__player);
            }

            if (InfinityHealth)
                player.data.health = 100f;

            if (InfinityOxy)
                player.data.remainingOxygen = 500f;

            if (InfinityStamina)
                player.data.currentStamina = 10f;

            if (player.input.jumpWasPressed)
            {
                player.refs.controller.jumpImpulse = JumpHeightMultipiler * 7f;
                if (InfinityJump)
                {
                    player.data.sinceGrounded = 0.4f;
                    player.data.sinceJump = 0.7f;
                }
            }

            if (NeverFalldown)
                player.data.fallTime = 0.0f;

            if (NeverDie && player.data.dead)
                player.CallRevive();

            player.refs.controller.sprintMultiplier = 2.3f * SprintMultipiler;

            CustomPlayerFace.ChangeFace();
        }
        public static void JoinRealm(bool local = false)
        {
            if (local)
            {
                ShadowRealmHandler.instance.TeleportPlayerToRandomRealm(Player.localPlayer);
            }
            else
            {
                foreach (var keyValuePair in InGame)
                {
                    if (!keyValuePair.Value || keyValuePair.Key.data.playerIsInRealm)
                        continue;
                    ShadowRealmHandler.instance.TeleportPlayerToRandomRealm(keyValuePair.Key);
                }
            }
        }
        public static void RemoveRealm(bool local = false)
        {
            if (local)
            {
                if (Player.localPlayer.data.playerIsInRealm)
                {
                    Debug.Log($"X:{Player.localPlayer.data.groundPos.x} Y:{Player.localPlayer.data.groundPos.y} Z:{Player.localPlayer.data.groundPos.z}");
                    GameObject[] currentRealms = Traverse.Create(ShadowRealmHandler.instance).Field("currentRealms").GetValue<GameObject[]>();
                    for (int i = 0; i < currentRealms.Length; i++)
                    {
                        if (currentRealms[i] && currentRealms[i].GetComponentInChildren<RealmGateTrigger>().playerInRealm != Player.localPlayer)
                            continue;
                        //暂时没传送功能
                        Traverse.Create(ShadowRealmHandler.instance).Field("view").GetValue<PhotonView>().RPC("RPCA_RemovePlayerFromRealm", RpcTarget.All, new object[]
                        {
                            i,
                            Player.localPlayer.refs.view.ViewID,
                            new Vector3(-0.1499804f, 0.06603602f, 0.01805818f)
                        });
                        break;
                    }
                }
            }
            else
            {
                foreach (var keyValuePair in InGame)
                {
                    if (!keyValuePair.Value)
                        continue;
                    if (!keyValuePair.Key.data.playerIsInRealm)
                        continue;

                    GameObject[] currentRealms = Traverse.Create(ShadowRealmHandler.instance).Field("currentRealms").GetValue<GameObject[]>();
                    for (int i = 0; i < currentRealms.Length; i++)
                    {
                        if (currentRealms[i] && currentRealms[i].GetComponentInChildren<RealmGateTrigger>().playerInRealm != keyValuePair.Key)
                            continue;
                        Traverse.Create(ShadowRealmHandler.instance).Field("view").GetValue<PhotonView>().RPC("RPCA_RemovePlayerFromRealm", RpcTarget.All, new object[]
                        {
                            i,
                            keyValuePair.Key.refs.view.ViewID,
                            Player.localPlayer.refs.headPos.position
                        });
                        break;
                    }
                }
            }
        }
        public static void ForceEnterTerminal()
        {
            foreach (var keyValuePair in InGame)
            {
                if (!keyValuePair.Value)
                    continue;
                PlayerCustomizer terminal = GameObject.FindObjectOfType<PlayerCustomizer>();
                Traverse.Create(terminal).Field("view_g").GetValue<PhotonView>().RPC("RPCM_RequestEnterTerminal", RpcTarget.MasterClient, new object[]
                {
                    keyValuePair.Key.refs.view.ViewID
                });
            }
        }
        public static void PlayEmote(byte id)
        {
            foreach (var keyValuePair in InGame)
            {
                if (!keyValuePair.Value)
                    continue;
                keyValuePair.Key.refs.view.RPC("RPC_PlayEmote", RpcTarget.All, new object[] { id });
            }
        }
        public static void Respawn(bool local = false)
        {
            if (local)
            {
                Player.localPlayer.refs.view.RPC("RPCA_PlayerRevive", RpcTarget.All, Array.Empty<object>());
            }
            else
            {
                foreach (var keyValuePair in InGame)
                {
                    if (!keyValuePair.Value)
                        continue;
                    keyValuePair.Key.refs.view.RPC("RPCA_PlayerRevive", RpcTarget.All, Array.Empty<object>());
                }
            }
        }
        public static void Die(bool local = false)
        {
            if (local)
            {
                Player.localPlayer.refs.view.RPC("RPCA_PlayerDie", RpcTarget.All, Array.Empty<object>());
            }
            else
            {
                foreach (var keyValuePair in InGame)
                {
                    if (!keyValuePair.Value)
                        continue;
                    keyValuePair.Key.refs.view.RPC("RPCA_PlayerDie", RpcTarget.All, Array.Empty<object>());
                }
            }
        }
        public static void DragToLocal()
        {
            foreach (var keyValuePair in InGame)
            {
                if (!keyValuePair.Value)
                    continue;
                Vector3 Dir = Player.localPlayer.refs.headPos.position - keyValuePair.Key.refs.headPos.position;
                keyValuePair.Key.refs.view.RPC("RPCA_TakeDamageAndAddForce", RpcTarget.All, new object[] { 0f, Dir.normalized * 8f, 1.5f });
            }
        }
        public static void Explode()
        {
            byte bomb_id = byte.MaxValue;
            Dictionary<byte, string> itemDic = Items.ItemsTypeList[Items.ItemType.Others];
            itemDic.Reverse();
            foreach (var kv in itemDic)
            {
                if (kv.Value == "Bomb")
                    bomb_id = kv.Key;
            }
            if (bomb_id == byte.MaxValue)
                return;
            foreach (var keyValuePair in InGame)
            {
                if (!keyValuePair.Value)
                    continue;
                Items.SpawnItem(new byte[] { bomb_id }, keyValuePair.Key.refs.cameraPos.position);
            }
        }
        public static void Cum(bool local = false)
        {
            if (local)
            {
                PhotonNetwork.Instantiate("ExplodedGoop", Player.localPlayer.refs.headPos.position, Quaternion.identity);
            }
            else
            {
                foreach (var keyValuePair in InGame)
                {
                    if (!keyValuePair.Value)
                        continue;
                    PhotonNetwork.Instantiate("ExplodedGoop", keyValuePair.Key.refs.headPos.position, Quaternion.identity);
                }
            }
        }
        public static void Falldown(bool local = false)
        {
            if (local)
            {
                Player.localPlayer.refs.view.RPC("RPCA_Fall", RpcTarget.All, new object[] { 5f });
            }
            else
            {
                foreach (var keyValuePair in InGame)
                {
                    if (!keyValuePair.Value)
                        continue;
                    keyValuePair.Key.refs.view.RPC("RPCA_Fall", RpcTarget.All, new object[] { 5f });
                }
            }
        }
        public static int GetChoosedPlayerCount()
        {
            int count = 0;
            foreach(var keyValuePair in InGame)
            {
                if (!keyValuePair.Value)
                    continue;
                count++;
            }
            return count;
        }
    }
    class CustomPlayerFace
    {
        public static List<string> Name = new List<string>
        {
            "注意看！",
            "Mamba Back！",
            "是的孩子们，我回来了！",
            "还有，那件事情我们是自愿的。",
            "我嘞个骚杠",
            "我想陆管了",
            "我嘞个豆",
            "兄弟你好香",
            "麻麻生的",
            "哈吉米哈吉米哈吉米莫那没卤多",
            "你有非常好高速运转的机器进入中国",
            "阿米挪思",
            "闭嘴！我的爸爸在为 米哈游 工作！所以他可以在你的电恼上安装原神！",

        };
        public static int CurrentNamePos = 0;
        public static float ColorHUE = 0.005f;
        public static float Rotation = 0f;
        public static bool Direction = true;
        public static float Scale = 0.03f;
        public static bool backScale = false;
        public static bool CanChangeName = true;

        public static void ChangeFace()
        {
            //color
            ColorHUE = ColorHUE >= 1.0f ? 0.005f : ColorHUE + 0.005f;
            //size
            if(backScale)
            {
                Scale -= 0.001f;
                if (Scale <= 0.03f)
                    backScale = !backScale;
            }
            else
            {
                Scale += 0.001f;
                if (Scale >= 0.08f)
                    backScale = !backScale;
            }
            //Rotation
            Rotation = Direction ? Rotation + 0.5f : Rotation - 0.5f;
            if (Rotation > 20f)
                Direction = false;
            else if (Rotation < -20f)
                Direction = true;

            if (CanChangeName)
            {
                if (CurrentNamePos >= Name.Count) CurrentNamePos = 0;
                RollString(CustomPlayerFace.Name[CurrentNamePos], 3);
                CanChangeName = false;
            }

            Player.localPlayer.refs.view.RPC("RPCA_SetAllFaceSettings", RpcTarget.AllBuffered, new object[]
            {
                        ColorHUE,
                        Player.localPlayer.refs.visor.visorColorIndex,
                        Player.localPlayer.refs.visor.visorFaceText.text,
                        AdjustAngle(Rotation),
                        Scale
            });
        }
        public static float AdjustAngle(float Angle)
        {
            // 如果角度小于0，加360使其回到0-360的范围内
            if (Angle < 0f)
                Angle += 360f;
            // 如果角度小于0，减360使其回到0-360的范围内
            if (Angle > 360f)
                Angle -= 360f;

            return Angle;
        }
        private static async void RollString(string str, int displayslot)
        {
            str = str.PadLeft(str.Length + displayslot, ' ');
            str = str.PadRight(str.Length + displayslot, ' ');
            int currentPos = 0;
            while (currentPos + displayslot <= str.Length)
            {
                if (Player.localPlayer == null)
                    break;
                await Task.Delay(400);
                await Task.Run(() =>
                {
                    Player.localPlayer.refs.view.RPC("RPCA_SetVisorText", RpcTarget.AllBuffered, new object[] { str.Substring(currentPos, displayslot) });
                });
                
                currentPos++;
            }
            CurrentNamePos++;
            CanChangeName = true;
        }
    }
}
