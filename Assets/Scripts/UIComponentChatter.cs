using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentChatter : MonoBehaviour
{
    [SerializeField] Image status;
    [SerializeField] TextMeshProUGUI chatterNickname;

    public void SetupChatterUI(string p_chatterNickname, bool p_isOnline)
    {
        chatterNickname.text = p_chatterNickname;

        if (p_isOnline) status.color = Color.green;
        else status.color = Color.grey;
    }

}
