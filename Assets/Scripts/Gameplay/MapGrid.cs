using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// 棋盘的一个格子
public class MapGrid : MonoBehaviour, IPointerDownHandler
{
    private Image bg;
    public Vector2 coord; // 格子的坐标
    public Action<Vector2> onClick; // 点击格子的回调
    private bool isEmpty; // 该格子是否已落子

    public void Init()
    {
        isEmpty = true;
        bg = transform.Find("bg").GetComponent<Image>();
    }

    public void SetAsOX(Sprite sprite)
    {
        if (isEmpty)
        {
            isEmpty = false;
            bg.sprite = sprite;
            bg.SetNativeSize();
            bg.transform.localScale = Vector3.one;
        }
    }

    public void SetAsEmpty()
    {
        isEmpty = true;
        bg.sprite = null;
        bg.transform.localScale = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onClick?.Invoke(coord);
    }
}