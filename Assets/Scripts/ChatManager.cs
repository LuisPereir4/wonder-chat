using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

[Serializable]
public class Chatter
{
    public string nickname;
    public ulong networkId;

    public Chatter(string nickname, ulong networkId)
    {
        this.nickname = nickname;
        this.networkId = networkId;
    }
}

[Serializable]
public class Chat 
{
    public List<Chatter> chatters;
}

public class ChatManager : NetworkBehaviour
{
    public Chat chat;

    [Header("Components")]
    [SerializeField] TextMeshProUGUI chatText;

    [ServerRpc(RequireOwnership = false)]
    public void SendMessageServerRpc(string p_message, string p_nickname)
    {
        SendMessageToAllChattersClientRpc(p_message, p_nickname);
    }

    [ClientRpc]
    public void SendMessageToAllChattersClientRpc(string p_message, string p_nickname)
    {
        chatText.text += $"\n<b><color=#1D5EAF>[{p_nickname}]</b></color> {p_message}";
        Debug.Log("Message received: " + p_message);
    }

}
