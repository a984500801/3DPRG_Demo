using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;



public class MouseManager : Singleton<MouseManager>
{
    //鼠标图片
    public Texture2D point, doorway, attack, target, arrow;

    //射线变量，存储信息
    RaycastHit hitInfo;
    //鼠标点击，返回值Vector3坐标
    public event Action<Vector3> OnMouseClicked;
    //鼠标点击敌人，返回值敌人物体坐标
    public event Action<GameObject> OnEnemyClicked;

    protected override void Awake()
    {
        //基于父类保留Awake
        base.Awake();

        DontDestroyOnLoad(this);
    }   


    void Update()
    {
        SetCursonTexture();
        if (InteractWithUI()) return;//与UI面板互动直接return，不执行鼠标控制方法
        MouseControl();
    }

    //检测射线信息
    void SetCursonTexture()
    {
        //发射射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (InteractWithUI())
        {
            Cursor.SetCursor(point, Vector2.zero, CursorMode.Auto);
            return;
        }

        //画射线
        if (Physics.Raycast(ray, out hitInfo))
        {
            //射线判断，切换鼠标贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }

    }

    //鼠标控制
    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                //触发事件，不为空，执行Invoke,实现移动
                OnMouseClicked?.Invoke(hitInfo.point);
            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }
            if (hitInfo.collider.gameObject.CompareTag("Item"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }
        }
    }

    //UI界面互动判断
    bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true; 
        else    
            return false;
    }
}
