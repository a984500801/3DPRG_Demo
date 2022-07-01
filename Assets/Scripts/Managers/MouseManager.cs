using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;



public class MouseManager : Singleton<MouseManager>
{
    //���ͼƬ
    public Texture2D point, doorway, attack, target, arrow;

    //���߱������洢��Ϣ
    RaycastHit hitInfo;
    //�����������ֵVector3����
    public event Action<Vector3> OnMouseClicked;
    //��������ˣ�����ֵ������������
    public event Action<GameObject> OnEnemyClicked;

    protected override void Awake()
    {
        //���ڸ��ౣ��Awake
        base.Awake();

        DontDestroyOnLoad(this);
    }   


    void Update()
    {
        SetCursonTexture();
        if (InteractWithUI()) return;//��UI��廥��ֱ��return����ִ�������Ʒ���
        MouseControl();
    }

    //���������Ϣ
    void SetCursonTexture()
    {
        //��������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (InteractWithUI())
        {
            Cursor.SetCursor(point, Vector2.zero, CursorMode.Auto);
            return;
        }

        //������
        if (Physics.Raycast(ray, out hitInfo))
        {
            //�����жϣ��л������ͼ
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

    //������
    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                //�����¼�����Ϊ�գ�ִ��Invoke,ʵ���ƶ�
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

    //UI���滥���ж�
    bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true; 
        else    
            return false;
    }
}
