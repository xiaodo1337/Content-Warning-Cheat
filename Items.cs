using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

namespace ContentWarningCheat
{
    internal class Items
    {
        public static Item[] ItemsList { get; set; }
        public static bool PartyMode = true;
        public static bool InfinityPower = true;
        public static bool NoGrabberLimit = true;
        public static bool BlockBombSpawn = false;
        public enum SpawnType
        {
            AddToInventory,
            CreatePickup,
            CallDrone
        }
        public static SpawnType SpawnMethod = SpawnType.AddToInventory;
        public enum ItemType
        {
            Lights,
            Medicals,
            Tools,
            Emotes,
            Miscs,
            Others
        }
        public static Dictionary<ItemType, Dictionary<byte, string>> ItemsTypeList = new Dictionary<ItemType, Dictionary<byte, string>>() {
            { ItemType.Lights, new Dictionary<byte, string> { } },
            { ItemType.Medicals, new Dictionary<byte, string> { } },
            { ItemType.Tools, new Dictionary<byte, string> { } },
            { ItemType.Emotes, new Dictionary<byte, string> { } },
            { ItemType.Miscs, new Dictionary<byte, string> { } },
            { ItemType.Others, new Dictionary<byte, string> { } },
        };
        public static readonly Dictionary<ItemType, List<string>> ItemsTypeDictionary = new Dictionary<ItemType, List<string>>()
        {
            { ItemType.Lights, new List<string> {"FakeOldFlashlight", "Flare", "Long Flashlight Pro", "Long Flashlight",
                "Modern Flashlight Pro" , "Modern Flashlight", "Old Flashlight", "Wide Flashlight 2","Wide Flashlight 3" } },
            { ItemType.Medicals, new List<string> { "Defibrilator", "Hugger" } },
            { ItemType.Tools, new List<string> { "Boom Mic", "Clapper", "GooBall", "Reporter Mic", "ShockStick", "SoundPlayer", "WalkieTalkie", "GrabberArm" } },
            { ItemType.Emotes, new List<string> { } },
            { ItemType.Miscs, new List<string> { "PartyPopper", "Radio" } },
            { ItemType.Others, new List<string> { } },
        };
        public static void Run()
        {
            Player player = Player.localPlayer;
            if (player == null)
                return;

            if (ItemsList == null)
            {
                DatabaseAsset<ItemDatabase, Item> instance = SingletonAsset<ItemDatabase>.Instance;
                ItemsList = (from item in instance.Objects
                             select item).ToArray();

                foreach (Item item in ItemsList)
                {
                    if (item.name.Contains("Emote_"))
                    {
                        ItemsTypeList[ItemType.Emotes].Add(item.id, item.name);
                        continue;
                    }
                    bool found = false;
                    foreach (KeyValuePair <ItemType, List<string>> itemtypekv in ItemsTypeDictionary)
                    {
                        found = false;
                        foreach (string str in itemtypekv.Value)
                        {
                            if (str != item.name)
                                continue;
                            found = true;
                            ItemsTypeList[itemtypekv.Key].Add(item.id, item.name);
                            break;
                        }
                        if (found) break;
                    }
                    if(!found)
                    {
                        ItemsTypeList[ItemType.Others].Add(item.id, item.name);
                    }
                }
            }
            if (NoGrabberLimit) SetGrabberArmNotBreak();
            if (BlockBombSpawn) CheckIsBombSpawn();
            if (InfinityPower) SetMaxBattery();
            if (PartyMode) SetPartyPopperUnused();
        }
        public static void CheckIsBombSpawn()
        {
            foreach (Pickup pickup in GameObject.FindObjectsOfType<Pickup>())
            {
                if (pickup.itemInstance.item.name != "Bomb")
                    continue;
                Traverse.Create(pickup).Field("m_photonView").GetValue<PhotonView>().RPC("RPC_Remove", RpcTarget.MasterClient, Array.Empty<object>());
            }
        }
        public static void SetGrabberArmNotBreak()
        {
            foreach (ItemGrabberArm grabberArm in GameObject.FindObjectsOfType<ItemGrabberArm>())
            {
                if (grabberArm.transform.parent.GetComponent<Player>() == Player.localPlayer)
                {
                    grabberArm.breakForceStartEnd = new Vector2(float.MaxValue, float.MaxValue);
                    grabberArm.breakForceTransitionTime = float.MaxValue;
                    /*
                    Player HoldingPlayer = Traverse.Create(grabberArm).Field("playerHoldingItem").GetValue<Player>();
                    if (HoldingPlayer != null)
                    {
                        HoldingPlayer.refs.view.RPC("RPCA_Fall", RpcTarget.All, new object[] { 1f });
                    }
                    */
                }
            }
        }
        public static void SetMaxBattery()
        {
            foreach(ShockStick stockStick in GameObject.FindObjectsOfType<ShockStick>())
            {
                if(stockStick.transform.parent.GetComponent<Player>() == Player.localPlayer)
                {
                    BatteryEntry batteryEntry = Traverse.Create(stockStick).Field("m_batteryEntry").GetValue<BatteryEntry>();
                    batteryEntry.m_charge = batteryEntry.m_maxCharge;
                }
            }
            foreach (Flashlight flashlight in GameObject.FindObjectsOfType<Flashlight>())
            {
                if (flashlight.transform.parent.GetComponent<Player>() == Player.localPlayer)
                {
                    BatteryEntry batteryEntry = Traverse.Create(flashlight).Field("m_batteryEntry").GetValue<BatteryEntry>();
                    batteryEntry.m_charge = batteryEntry.m_maxCharge;
                }
            }
            foreach (Defib defib in GameObject.FindObjectsOfType<Defib>())
            {
                if (defib.transform.parent.GetComponent<Player>() == Player.localPlayer)
                {
                    BatteryEntry batteryEntry = Traverse.Create(defib).Field("m_batteryEntry").GetValue<BatteryEntry>();
                    batteryEntry.m_charge = batteryEntry.m_maxCharge;
                }
            }
            foreach (ArtifactRadio artifactRadio in GameObject.FindObjectsOfType<ArtifactRadio>())
            {
                if (artifactRadio.transform.parent.GetComponent<Player>() == Player.localPlayer)
                {
                    BatteryEntry batteryEntry = Traverse.Create(artifactRadio).Field("batteryEntry").GetValue<BatteryEntry>();
                    batteryEntry.m_charge = batteryEntry.m_maxCharge;
                }
            }
        }
        public static void SetPartyPopperUnused()
        {
            foreach(PartyPopper partyPopper in GameObject.FindObjectsOfType<PartyPopper>())
            {
                if(!partyPopper.transform.parent.TryGetComponent(out Player owner))
                    continue;
                if (Player.localPlayer == owner)
                {
                    bool used = Traverse.Create(partyPopper).Field("usedEntry").GetValue<OnOffEntry>().on;
                    if(used && Player.localPlayer.TryGetInventory(out PlayerInventory inventory))
                    {
                        byte popper_id = byte.MaxValue;
                        foreach(KeyValuePair<byte, string> keyValuePair in ItemsTypeList[ItemType.Miscs])
                        {
                            if (keyValuePair.Value != "PartyPopper")
                                continue;
                            popper_id = keyValuePair.Key;
                            break;
                        }
                        if (!ItemDatabase.TryGetItemFromID(popper_id, out Item item))
                            return;

                        //PlayerItems playerItems = Player.localPlayer.GetComponent<PlayerItems>();
                        var temp = Player.localPlayer.data.selectedItemSlot;
                        if (temp != -1 && inventory.slots[temp].ItemInSlot.item.name == item.name)
                        {
                            inventory.slots[temp].Clear();
                            ItemDescriptor newitem = new ItemDescriptor(item, new ItemInstanceData(new Guid()));
                            if (inventory.TryAddItem(newitem))
                            {
                                Player.localPlayer.refs.view.RPC("RPC_SelectSlot", RpcTarget.All, new object[] { temp });
                            }
                            else
                            {
                                Debug.Log("Cant get slot");
                            }
                        }
                    }
                }
            }
        }
        public static void SpawnEmoteBooks()
        {
            foreach (KeyValuePair<byte, string> itemkeypair in ItemsTypeList[ItemType.Emotes])
            {
                if (SpawnMethod == SpawnType.CallDrone)
                    SpawnItem(new byte[] { itemkeypair.Key });
                if (SpawnMethod == SpawnType.CreatePickup)
                    SpawnItem(new byte[] { itemkeypair.Key }, UsefulFuncs.GetCrosshairPosition(true, 1.5f));
            }
        }
        public static void SpawnAllItems()
        {
            List<Item> array = ItemsList.ToList<Item>();
            List<byte> array_id = new List<byte>();
            array.ForEach((Item item) =>
            {
                if (item.name != "Bomb")
                    array_id.Add(item.id);
            });

            if (SpawnMethod == SpawnType.CallDrone)
                SpawnItem(array_id.ToArray());
            if (SpawnMethod == SpawnType.CreatePickup)
                SpawnItem(array_id.ToArray(), UsefulFuncs.GetCrosshairPosition(true, 1.5f));
        }
        public static void SpawnItem(byte itemid)
        {
            if (!ItemDatabase.TryGetItemFromID(itemid, out Item item) || !Player.localPlayer.TryGetInventory(out PlayerInventory inventory))
                return;
            if (!inventory.TryAddItem(new ItemDescriptor(item, new ItemInstanceData(new Guid()))))
                HelmetText.Instance.SetHelmetText($"无法将{item.name}添加至物品栏", 3f);
        }
        public static void SpawnItem(byte[] itemid, Vector3 pos)
        {
            for (int i = 0; i < itemid.Length; i++)
                Player.localPlayer.RequestCreatePickup(itemid[i], new ItemInstanceData(Guid.NewGuid()), pos, UnityEngine.Random.rotation, (Vector3.down + UnityEngine.Random.onUnitSphere) * 2f, UnityEngine.Random.onUnitSphere * 5f);
        }
        public static void SpawnItem(byte[] itemid)
        {
            try
            {
                Traverse.Create(ShopHandler.Instance).Method("BuyItem", 0, itemid, 0.0f, 0.0f, 0.0f).GetValue(0, itemid, 0.0f, 0.0f, 0.0f);
            }
            catch
            {
                HelmetText.Instance.SetHelmetText($"无法在下界使用无人机投递", 3f);
            }
        }
        public static void DestoryDropedItems()
        {
            foreach (Pickup pickup in GameObject.FindObjectsOfType<Pickup>())
            {
                if (pickup.itemInstance.item.name == "Camera" || pickup.itemInstance.item.name == "Old Flashlight")
                    continue;
                Traverse.Create(pickup).Field("m_photonView").GetValue<PhotonView>().RPC("RPC_Remove", RpcTarget.MasterClient, Array.Empty<object>());
            }
        }
        public static void DumpItemsToConsole()
        {
            DatabaseAsset<ItemDatabase, Item> instance = SingletonAsset<ItemDatabase>.Instance;
            Item[] array = (from item in instance.Objects
                            select item).ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                Debug.Log($"{array[i].name}({array[i].id})");
            }
        }
    }
}
