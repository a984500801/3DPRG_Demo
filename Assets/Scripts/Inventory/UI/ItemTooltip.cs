using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    //获得Text组件
    public Text itemNameText;
    public Text itemInfoText;
    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    //提示信息赋值
    public void SetupTooltip(ItemData_SO item)
    {
        itemNameText.text = item.itemName;
        itemInfoText.text = item.description;
    }

    void OnEnable()
    {
        UpdatePosition();
    }

    void Update()
    {
        UpdatePosition();
    }

    //更新提示信息面板位置
    public void UpdatePosition()
    {
        Vector3 mousePos = Input.mousePosition;     //鼠标位置

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);     //拿到坐标数组

        float width = corners[3].x - corners[0].x;  //宽
        float height = corners[1].y - corners[0].y; //高

        if (mousePos.y < height)
            rectTransform.position = mousePos + Vector3.up * height * 0.6f;

        else if (Screen.width - mousePos.x < width)
            rectTransform.position = mousePos + Vector3.left * width * 0.6f;
        else
            rectTransform.position = mousePos + Vector3.right * width * 0.6f;
    }
}

