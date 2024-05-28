using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimatedBackground : MonoBehaviour
{
    [SerializeField] RawImage m_backgroundImage;
    [SerializeField] float m_xMoveVelocty;
    [SerializeField] float m_yMoveVelocty;

    private void OnEnable() => StartCoroutine(MoveBackground());

    private void OnDisable() => StopCoroutine(MoveBackground());

    IEnumerator MoveBackground()
    {
        while (true)
        {
            yield return null;
            m_backgroundImage.uvRect = new Rect(m_backgroundImage.uvRect.position + new Vector2(m_xMoveVelocty, m_yMoveVelocty) * Time.deltaTime, m_backgroundImage.uvRect.size);
        }
    }

}