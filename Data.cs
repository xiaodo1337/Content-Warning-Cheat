using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ContentWarningCheat
{
    internal class Data
    {
        private static float NextUpdate = 0f;
        private static readonly float UpdateInterval = 1.0f;
        public static ItemGrabberArm[] ItemGrabberArmsList;
        public static PartyPopper[] PartyPoppersList;
        public static ShockStick[] ShockSticksList;
        public static Flashlight[] FlashlightsList;
        public static Defib[] DefibsList;
        public static ArtifactRadio[] ArtifactRadiosList;
        public static Player[] PlayersList;
        public static Pickup[] PickupsList;
        public static Bot[] BotsList;
        public static UseDivingBellButton[] DivingBellsList;
        public static void UpdateData()
        {
            if (Time.time >= NextUpdate)
            {
                NextUpdate = Time.time + UpdateInterval;
                if (Player.localPlayer == null)
                    return;

                ItemGrabberArmsList = GameObject.FindObjectsOfType<ItemGrabberArm>();
                PartyPoppersList = GameObject.FindObjectsOfType<PartyPopper>();
                ShockSticksList = GameObject.FindObjectsOfType<ShockStick>();
                FlashlightsList = GameObject.FindObjectsOfType<Flashlight>();
                DefibsList = GameObject.FindObjectsOfType<Defib>();
                ArtifactRadiosList = GameObject.FindObjectsOfType<ArtifactRadio>();
                PlayersList = GameObject.FindObjectsOfType<Player>();
                PickupsList = GameObject.FindObjectsOfType<Pickup>();
                BotsList = GameObject.FindObjectsOfType<Bot>();
                DivingBellsList = GameObject.FindObjectsOfType<UseDivingBellButton>();

                //Players
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
