using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TestUnityPlugin
{
    internal class Monsters
    {
        public static List<string> MonsterNames = new List<string>
        {
            "BarnacleBall",
            "BigSlap",
            "Harpooner",
            "CamCreep",
            "Angler",
            "AnglerMimic",
            "MimicInfiltrator",
            "Wallo",
            "Bombs",
            "Dog",
            "Ear",
            "EyeGuy",
            "Flicker",
            "Ghost",
            "Jello",
            "Knifo",
            "Larva",
            "Mouthe",
            "Slurper",
            "Snatcho",
            "Spider",
            "Toolkit_Fan",
            "Toolkit_Hammer",
            "Toolkit_Iron",
            "Toolkit_Vaccuum",
            "Toolkit_Wisk",
            "Weeping"
        };
        public static bool AutoRemoveLocalMonsters = false;
        public static void Run()
        {
            if(AutoRemoveLocalMonsters) RemoveMonsters();
        }
        public static void RemoveMonsters()
        {
            foreach (Bot monster in GameObject.FindObjectsOfType<Bot>())
            {
                GameObject.Destroy(monster.transform.parent);
            }
        }
        public static void SpawnMonster(string monster)
        {
            PhotonNetwork.Instantiate(monster, UsefulFuncs.GetCrosshairPosition(true), UnityEngine.Quaternion.identity, 0, null);
        }
        //Todo: Make Mouthe Scream
        public static void MakeMouthesScream()
        {
            foreach (Bot_Mouth botMouth in GameObject.FindObjectsOfType<Bot_Mouth>())
            {
                Traverse.Create(botMouth).Field("fleeTime").SetValue(9999f);
                Traverse.Create(botMouth).Method("Combat").GetValue();
            }
        }
        public static void ClearAllMonsters()
        {
            Monster.KillAll();
        }
        public static void KillAll()
        {
            foreach (Bot monster in GameObject.FindObjectsOfType<Bot>())
            {
                monster.transform.parent.GetComponent<Player>().refs.view.RPC("RPCA_PlayerRevive", RpcTarget.All, new object[] { });
            }
        }
        public static void Die()
        {
            foreach (Bot monster in GameObject.FindObjectsOfType<Bot>())
            {
                monster.transform.parent.GetComponent<Player>().refs.view.RPC("RPCA_PlayerDie", RpcTarget.All, new object[] { });
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

            foreach (Bot monster in GameObject.FindObjectsOfType<Bot>())
            {
                Items.SpawnItem(new byte[] { bomb_id }, monster.transform.parent.position);
            }
        }
        public static void Falldown()
        {
            foreach (Bot monster in GameObject.FindObjectsOfType<Bot>())
            {
                monster.transform.parent.GetComponent<Player>().refs.view.RPC("RPCA_Fall", RpcTarget.All, new object[] { 5f });
            }
        }
        public static void Cum()
        {
            foreach (Bot monster in GameObject.FindObjectsOfType<Bot>())
            {
                PhotonNetwork.Instantiate("ExplodedGoop", monster.groundTransform.position, Quaternion.identity);
            }
        }
        public static void DragToLocal()
        {
            foreach (Bot monster in GameObject.FindObjectsOfType<Bot>())
            {
                Vector3 Dir = Player.localPlayer.refs.headPos.position - monster.centerTransform.position;
                monster.transform.parent.GetComponent<Player>().refs.view.RPC("RPCA_TakeDamageAndAddForce", RpcTarget.All, new object[] { 0f, Dir.normalized * 8f, 1.5f });
            }
        }
    }
}
