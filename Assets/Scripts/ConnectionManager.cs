using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Button startServerButton;
    [SerializeField] Button startHostButton;
    [SerializeField] Button startClientButton;

    [Header("Connections")]
    public List<ulong> clientsIds = new List<ulong>();

    private void Awake()
    {
        startServerButton.onClick.AddListener(() =>
        {
            Debug.Log("[ConnectionManager] Starting Server");
            NetworkManager.Singleton.StartServer();
            ListenToNetworkManagerEvents(true);
        });

        startHostButton.onClick.AddListener(() =>
        {
            Debug.Log("[ConnectionManager] Starting Host");
            NetworkManager.Singleton.StartHost();
            ListenToNetworkManagerEvents(true);
        });

        startClientButton.onClick.AddListener(() =>
        {
            Debug.Log("[ConnectionManager] Starting Client");
            NetworkManager.Singleton.StartClient();
            ListenToNetworkManagerEvents(true);
        });
    }

    private void ListenToNetworkManagerEvents(bool p_state)
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

    public void ConnectClient()
    {
        Debug.Log("[ConnectionManager] Starting Client");
        NetworkManager.Singleton.StartClient();
        ListenToNetworkManagerEvents(true);
    }

    public void DisconnectClient()
    {
        Debug.Log("[ConnectionManager] Disconnecting Client");
        NetworkManager.Singleton.Shutdown();
        ListenToNetworkManagerEvents(false);
    }

    private void HandleClientConnected(ulong p_clientId)
    {
        Debug.Log($"[ConnectionManager] Client connected with id {p_clientId}");

        if (NetworkManager.Singleton.IsServer)
        {
            clientsIds.Add(p_clientId);
        }
    }

    private void HandleClientDisconnected(ulong p_clientId)
    {
        Debug.Log($"[ConnectionManager] Client disconnected with id {p_clientId}");

        if (NetworkManager.Singleton.IsServer)
        {
            clientsIds.Remove(p_clientId);
        }
    }

}
