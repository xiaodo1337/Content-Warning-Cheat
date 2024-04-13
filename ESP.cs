using System;
using System.Threading;
using UnityEngine;
using Zorro.Core;

namespace TestUnityPlugin
{
    internal class ESP
    {
        public static bool EnableDrawString { get; set; } = true;
        public static bool EnableDrawLine { get; set; } = false;
        public static bool EnablePlayerESP { get; set; } = true;
        public static bool EnableMonsterESP { get; set; } = true;
        public static bool EnableItemESP { get; set; } = true;
        public static bool EnableDistance { get; set; } = false;
        public static void StartESP()
        {
            PlayerESP();
            MonsterESP();
            ItemESP();
        }
        public static void PlayerESP()
        {
            if (!EnablePlayerESP)
                return;
            foreach (Player player in GameObject.FindObjectsOfType<Player>())
            {
                if (player.ai || player.IsLocal)
                    continue;
                Vector3 ppos = player.refs.headPos.position;
                ppos.y += 0.5f;
                Vector3 spos = Camera.main.WorldToScreenPoint(ppos);
                float distance = Vector3.Distance(ppos, Player.localPlayer.refs.headPos.position);
                float fontsize = Mathf.Clamp(10f / distance, 0.5f, 1f) * 17f;
                // 绘制
                if (spos.z > 0f)
                {
                    if (EnableDrawLine) Renderer.DrawLine(new Vector2((float)Screen.width / 2, Screen.height), new Vector2(spos.x, Screen.height - spos.y), Color.white, 2f);
                    if (EnableDrawString)
                    {
                        Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y), player.refs.view.Owner.NickName, Color.white, fontsize);
                        if (EnableDistance) Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y + Renderer.CalcStringSize(player.refs.view.Owner.NickName, fontsize).y), Mathf.RoundToInt(distance).ToString() + "m", Color.yellow, fontsize);
                    }
                }
            }
        }
        public static void MonsterESP()
        {
            if (!EnableMonsterESP)
                return;
            foreach (Bot monster in GameObject.FindObjectsOfType<Bot>())
            {
                Vector3 mpos = monster.groundTransform.position;
                mpos.y -= 0.2f;
                Vector3 spos = Camera.main.WorldToScreenPoint(mpos);
                float distance = Vector3.Distance(mpos, Player.localPlayer.refs.headPos.position);
                float fontsize = Mathf.Clamp(10f / distance, 0.5f, 1f) * 17f;
                // 绘制
                if (spos.z > 0f)
                {
                    if (EnableDrawLine) Renderer.DrawLine(new Vector2((float)Screen.width / 2, Screen.height), new Vector2(spos.x, Screen.height - spos.y), Color.red, 2f);
                    if (EnableDrawString)
                    {
                        Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y), monster.transform.parent.name.Replace("(Clone)", ""), Color.red, fontsize);
                        if (EnableDistance) Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y + Renderer.CalcStringSize(monster.transform.parent.name.Replace("(Clone)", ""), fontsize).y), Mathf.RoundToInt(distance).ToString() + "m", Color.yellow, fontsize);
                    }
                }
            }
        }
        public static void ItemESP()
        {
            if (!EnableItemESP)
                return;
            foreach (Pickup pickup in GameObject.FindObjectsOfType<Pickup>())
            {
                if (pickup.name != "PickupHolder(Clone)")
                    continue;
                GameObject item;
                Vector3 ipos;
                try
                {
                    item = pickup.transform.GetChild(0).gameObject;
                    ipos = item.transform.position;
                }
                catch
                {
                    continue;
                }
                ipos.y -= 0.2f;
                Vector3 spos = Camera.main.WorldToScreenPoint(ipos);
                float distance = Vector3.Distance(ipos, Player.localPlayer.refs.headPos.position);
                float fontsize = Mathf.Clamp(10f / distance, 0.5f, 1f) * 17f;
                // 绘制
                if (spos.z > 0f)
                {
                    if(EnableDrawLine) Renderer.DrawLine(new Vector2((float)Screen.width / 2, Screen.height), new Vector2(spos.x, Screen.height - spos.y), Color.green, 2f);
                    if (EnableDrawString)
                    {
                        Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y), item.name.Replace("(Clone)", ""), Color.green, fontsize);
                        if(EnableDistance) Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y + Renderer.CalcStringSize(item.name.Replace("(Clone)", ""), fontsize).y), Mathf.RoundToInt(distance).ToString() + "m", Color.yellow, fontsize);
                    }
                }
            }
        }
    }
}
