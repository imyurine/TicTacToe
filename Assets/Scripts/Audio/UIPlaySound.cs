using UnityEngine;
using UnityEngine.EventSystems;

/// 用于播放ui操作音效
public class UIPlaySound : MonoBehaviour, IPointerDownHandler
{
    public string audioId;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            AudioSystem.Instance.PlayAudio(audioId);
        }
    }
}