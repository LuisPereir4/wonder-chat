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

    [Header("Buttons")]
    [SerializeField] Button sendMessageButton;
    [SerializeField] Button joinChatButton;
    [SerializeField] Button leaveChatButton;

    [Header("Buttons - Debug")]
    [SerializeField] Button startServerButton;
    [SerializeField] Button startHostButton;
    [SerializeField] Button startClientButton;

    [Header("Input fields")]
    [SerializeField] TMP_InputField nickNameInputField;
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

    private void Update() => CheckKeyboard();

    public void ConnectedIntoChat()
    {
        mainPanel.SetActive(false);
        chatPanel.SetActive(true);

        for (int i = 0; i < chatters.Count; i++)
        {
            if (chatters[i].gameObject.activeSelf == false)
            {
                chatters[i].gameObject.SetActive(true);
                chatters[i].SetupChatterUI(PlayerPrefs.GetString("Nickname"), true);
                break;
            }
        }
    }

    public void LeftChat()
    {
        mainPanel.SetActive(true);
        chatPanel.SetActive(false);
    }

    public void InputFieldValidations()
    {
        nickNameInputField.onValueChanged.AddListener(l_nickNameText =>
        {
            if (nickNameInputField.text.Length > 0)
            {
                joinChatButton.interactable = true;
            }
            else
            {
                joinChatButton.interactable = false;
            }
        });

        chatInputField.onValueChanged.AddListener(l_chatText =>
        {
            if (chatInputField.text.Length > 0)
            {
                sendMessageButton.interactable = true;
            }
            else
            {
                sendMessageButton.interactable = false;
            }
        });
    }

    public void SetupButtonsListeners()
    {
        joinChatButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString("Nickname", nickNameInputField.text);
            connectionManager.ConnectClient();
            ConnectedIntoChat();
        });

        leaveChatButton.onClick.AddListener(() =>
        {
            connectionManager.DisconnectClient();
            LeftChat();
        });

        sendMessageButton.onClick.AddListener(() =>
        {
            chatManager.SendMessageServerRpc(chatInputField.text, PlayerPrefs.GetString("Nickname"));
            chatInputField.text = string.Empty;
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

    private void CheckKeyboard()
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

}
