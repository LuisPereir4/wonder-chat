using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using System.Text;

public class ConnectionManager : NetworkBehaviour
{
    [Header("Connections")]
    public List<Chatter> chatters = new List<Chatter>();

    [Header("Connections")]
    [SerializeField] UIManager uiManager;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRoom()
    {
        try
        {
            Allocation l_allocation = await RelayService.Instance.CreateAllocationAsync(20);
            string l_joinCode = await RelayService.Instance.GetJoinCodeAsync(l_allocation.AllocationId);

            Debug.Log($"[ConnectionManager] Created relay server with code: {l_joinCode}");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                l_allocation.RelayServer.IpV4,
                (ushort)l_allocation.RelayServer.Port,
                l_allocation.AllocationIdBytes,
                l_allocation.Key,
                l_allocation.ConnectionData);

            ListenToNetworkManagerEvents(true);
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log($"[ConnectionManager] Fail while hosting {e}");
        }
    }

    public async void JoinRoom(string p_joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(p_joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData);

            ListenToNetworkManagerEvents(true);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            ListenToNetworkManagerEvents(false);
            Debug.Log($"[ConnectionManager] Fail while connecting {e}");
        }
    }

    public void ListenToNetworkManagerEvents(bool p_state)
    {
        if (p_state)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }
        else
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    public void Disconnect()
    {
        Debug.Log("[ConnectionManager] Disconnecting from chat");
        NetworkManager.Singleton.Shutdown();
        ListenToNetworkManagerEvents(false);
    }

    private void HandleClientConnected(ulong p_clientId)
    {
        Debug.Log($"[ConnectionManager] Client connected with id {p_clientId}");

        if (p_clientId == NetworkManager.Singleton.LocalClientId) uiManager.OnConnectedIntoServer();
    }

    private void HandleClientDisconnected(ulong p_clientId)
    {
        Debug.Log($"[ConnectionManager] Client disconnected with id {p_clientId}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifyServerAboutConnectionServerRpc(ulong p_clientId, string p_nickname)
    {
        chatters.Add(new Chatter(p_nickname, p_clientId));

        string[] l_chattersNicknameList = new string[chatters.Count];
        for (int i = 0; i < chatters.Count; i++)
        {
            l_chattersNicknameList[i] = chatters[i].nickname;
        }

        NotifyClientsAboutConnectionClientRpc(BuildChattersNicknameList());
    }

    [ClientRpc]
    public void NotifyClientsAboutConnectionClientRpc(string p_chattersNicknames)
    {
        //string[] l_chattersNickNames 
    }

    StringBuilder l_stringBuilder;
    private string BuildChattersNicknameList()
    {
        l_stringBuilder.Clear();

        for (int i = 0; i < chatters.Count; i++)
        {
            l_stringBuilder.Append($"{chatters[i].nickname}/{chatters[i].networkId}|");
        }

        return l_stringBuilder.ToString();
    }

}
