using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject chatPanel;
    [SerializeField] GameObject roomNotFoundPanel;

    [Header("Buttons")]
    [SerializeField] Button sendMessageButton;
    [SerializeField] Button joinChatButton;
    [SerializeField] Button leaveChatButton;
    [SerializeField] Button gotItButton;

    [Header("Input fields")]
    [SerializeField] TMP_InputField nickNameInputField;
    [SerializeField] TMP_InputField roomIdInputField;
    [SerializeField] TMP_InputField chatInputField;

    [Header("Chatters")]
    [SerializeField] List<UIComponentChatter> chatters;

    [Header("Managers")]
    [SerializeField] ChatManager chatManager;
    [SerializeField] ConnectionManager connectionManager;

    private void Awake()
    {
        HandlePlayerPreferences();
        InputFieldValidations();
        SetupButtonsListeners();
    }

    private void Update() => CheckKeyboardInput();

    public void OnConnectedIntoServer()
    {
        mainPanel.SetActive(false);
        chatPanel.SetActive(true);
        roomNotFoundPanel.SetActive(false);

        SetupChattersList();
    }

    public void OnLeftChat()
    {
        mainPanel.SetActive(true);
        chatPanel.SetActive(false);
        roomNotFoundPanel.SetActive(false);
    }

    public void InputFieldValidations()
    {
        nickNameInputField.onValueChanged.AddListener(l_nickNameText =>
        {
            if (nickNameInputField.text.Length > 0 && roomIdInputField.text.Length > 0) joinChatButton.interactable = true;
            else joinChatButton.interactable = false;
        });

        roomIdInputField.onValueChanged.AddListener(l_nickNameText =>
        {
            if (nickNameInputField.text.Length > 0 && roomIdInputField.text.Length > 0) joinChatButton.interactable = true;
            else joinChatButton.interactable = false;
        });

        chatInputField.onValueChanged.AddListener(l_chatText =>
        {
            if (chatInputField.text.Length > 0) sendMessageButton.interactable = true;
            else sendMessageButton.interactable = false;
        });
    }

    public void SetupButtonsListeners()
    {
        joinChatButton.onClick.AddListener(() =>
        {
            joinChatButton.interactable = false;
            PlayerPrefs.SetString("Nickname", nickNameInputField.text);
            connectionManager.JoinRoom(roomIdInputField.text);
        });

        leaveChatButton.onClick.AddListener(() =>
        {
            joinChatButton.interactable = true;
            connectionManager.Disconnect();
            OnLeftChat();
        });

        sendMessageButton.onClick.AddListener(() =>
        {
            chatManager.SendMessageServerRpc(chatInputField.text, PlayerPrefs.GetString("Nickname"));
            chatInputField.text = string.Empty;
        });

        gotItButton.onClick.AddListener(() =>
        {
            joinChatButton.interactable = true;
            roomNotFoundPanel.gameObject.SetActive(false);
        });
    }

    public void HandlePlayerPreferences()
    {
        if (PlayerPrefs.HasKey("Nickname"))
        {
            nickNameInputField.text = PlayerPrefs.GetString("Nickname");
            joinChatButton.interactable = true;
        }
    }

    private void CheckKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (mainPanel.activeSelf && nickNameInputField.text.Length > 0)
            {
                joinChatButton.onClick.Invoke();
            }
            else if (chatPanel.activeSelf && chatInputField.text.Length > 0)
            {
                sendMessageButton.onClick.Invoke();
            }
        }
    }

    private void SetupChattersList()
    {
        for (int i = 0; i < chatters.Count; i++)
        {
            if (chatters[i].gameObject.activeSelf) chatters[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < chatters.Count; i++)
        {
            if (!chatters[i].gameObject.activeSelf)
            {
                chatters[i].gameObject.SetActive(true);
                chatters[i].SetupChatterUI(PlayerPrefs.GetString("Nickname"), true);
                break;
            }
        }
    }

    public void OnRoomNotFound()
    {
        joinChatButton.interactable = false;
        roomNotFoundPanel.SetActive(true);
    }

}
