using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

namespace TestUnityPlugin
{
    internal class Misc
    {
        public static bool AutoJoinRandom = false;
        public static bool ForceJoinOthersRoom = true;
        public static bool DisableAutoJoinRandomWhenJoined = true;
        public static void Run()
        {
            AutoJoinRandomFunc();
        }

        public static void AutoJoinRandomFunc()
        {
            if (AutoJoinRandom)
            {
                if (DisableAutoJoinRandomWhenJoined && Player.localPlayer)
                {
                    if (PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer)
                    {
                        PhotonNetwork.Disconnect();
                        return;
                    }
                    AutoJoinRandom = false;
                    return;
                }
                if (ForceJoinOthersRoom && Player.localPlayer && PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer)
                {
                    PhotonNetwork.Disconnect();
                    return;
                }
                if (PhotonNetwork.NetworkClientState != Photon.Realtime.ClientState.ConnectedToMasterServer)
                    return;
                foreach (EscapeMenuButton escapeMenuButton in GameObject.FindObjectsOfType<EscapeMenuButton>())
                {
                    if (escapeMenuButton.name != "ModalButton(Clone)")
                        continue;
                    UnityEngine.UI.Button OkButton = escapeMenuButton.GetComponent<UnityEngine.UI.Button>();
                    OkButton.onClick.Invoke();
                    break;
                }
                /*
                 * Old Method
                MainMenuMainPage mainMenuMainPage = GameObject.FindObjectOfType<MainMenuMainPage>() ;
                if( mainMenuMainPage && mainMenuMainPage.gameObject.activeSelf )
                {
                    GameObject Buttons = mainMenuMainPage.transform.Find("Buttons").gameObject;
                    if(Buttons && Buttons.activeSelf)
                    {
                        UnityEngine.UI.Button JoinButton = Buttons.transform.Find("Join").gameObject.GetComponent<UnityEngine.UI.Button>();
                        if(JoinButton) JoinButton.onClick.Invoke();
                    }
                }*/
                MainMenuHandler.Instance.JoinRandom();
            }
        }
    }
}
