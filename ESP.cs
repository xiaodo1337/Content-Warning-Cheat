using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zorro.Core;

namespace ContentWarningCheat
{
    internal class ESP
    {
        public static bool EnableDrawString { get; set; } = true;
        public static bool EnableDrawLine { get; set; } = false;
        public static bool EnablePlayerESP { get; set; } = true;
        public static bool EnableMonsterESP { get; set; } = true;
        public static bool EnableItemESP { get; set; } = true;
        public static bool EnableDivingBellESP { get; set; } = true;
        public static bool EnableDistance { get; set; } = false;
        public static Player[] PlayersList;
        public static Pickup[] PickupsList;
        public static Bot[] BotsList;
        public static UseDivingBellButton[] DivingBellsList;
        public static void StartESP()
        {
            if (Player.localPlayer == null)
                return;
            if (EnablePlayerESP) PlayerESP();
            if (EnableMonsterESP) MonsterESP();
            if (EnableItemESP) ItemESP();
            if (EnableDivingBellESP) DivingBellESP();
        }
        public static void PlayerESP()
        {
            foreach(Player player in PlayersList)
            {
                if (player == null || player.ai || player.IsLocal)
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
                        Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y), player.refs.view.Owner.NickName, Color.white, fontsize);
                    if (EnableDistance)
                        Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y + (EnableDrawString ? Renderer.CalcStringSize(player.refs.view.Owner.NickName, fontsize).y : 0f)), Mathf.RoundToInt(distance).ToString() + "m", Color.yellow, fontsize);
                }
            }
        }
        public static void MonsterESP()
        {
            foreach(Bot monster in BotsList)
            {
                if (monster == null)
                    continue;
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
                        Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y), monster.transform.parent.name.Replace("(Clone)", ""), Color.red, fontsize);
                    if (EnableDistance)
                        Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y + (EnableDrawString ? Renderer.CalcStringSize(monster.transform.parent.name.Replace("(Clone)", ""), fontsize).y : 0f)), Mathf.RoundToInt(distance).ToString() + "m", Color.yellow, fontsize);
                }
            }
        }
        public static void ItemESP()
        {
            foreach (Pickup pickup in PickupsList)
            {
                if (pickup == null || pickup.name != "PickupHolder(Clone)")
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
                        Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y), item.name.Replace("(Clone)", ""), Color.green, fontsize);
                    if(EnableDistance)
                        Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y + (EnableDrawString ? Renderer.CalcStringSize(item.name.Replace("(Clone)", ""), fontsize).y : 0f)), Mathf.RoundToInt(distance).ToString() + "m", Color.yellow, fontsize);
                }
            }
        }
        public static void DivingBellESP()
        {
            foreach (UseDivingBellButton divingbellbutton in DivingBellsList)
            {
                if (divingbellbutton == null)
                    continue;
                Vector3 dpos = divingbellbutton.transform.position;
                dpos.y -= 0.2f;
                Vector3 spos = Camera.main.WorldToScreenPoint(dpos);
                float distance = Vector3.Distance(dpos, Player.localPlayer.refs.headPos.position);
                float fontsize = Mathf.Clamp(10f / distance, 0.5f, 1f) * 17f;
                // 绘制
                if (spos.z > 0f)
                {
                    if (EnableDrawLine) Renderer.DrawLine(new Vector2((float)Screen.width / 2, Screen.height), new Vector2(spos.x, Screen.height - spos.y), Color.cyan, 2f);
                    if (EnableDrawString)
                        Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y), "潜艇", Color.cyan, fontsize);
                    if (EnableDistance)
                        Renderer.DrawColorString(new Vector2(spos.x, Screen.height - spos.y + (EnableDrawString ? Renderer.CalcStringSize("潜艇", fontsize).y : 0f)), Mathf.RoundToInt(distance).ToString() + "m", Color.yellow, fontsize);
                }
            }
        }
    }
}
