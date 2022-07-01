using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    //UIprafab
    public GameObject healthUIPrefab;
    //�����趨��Ѫ��λ������
    public Transform barPoint;
    //���ÿɼ��Ĳ���ֵ
    public bool alwaysVisible;
    //���ӻ�ʱ��
    public float visibleTime;
    private float timeLeft;

    //������
    Image healthSlider;
    //Ѫ��
    Transform UIbar;
    //�����λ��
    Transform cam;
    //�õ�Ѫ������
    CharacterStats currentStats;

    void Awake()
    {
        //�õ��������
        currentStats = GetComponent<CharacterStats>();
        //Ϊ�¼����ĸ���Ѫ������
        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    void OnEnable()
    {
        //���λ��
        cam = Camera.main.transform;

        //�������е�Canvas
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            //�ж�Canvas�Ƿ�������������Ⱦ
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                //����prefab���õ�Ѫ��������
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;
                //�õ������ ͼƬ
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                //����Ϊ���ÿɼ�
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    //����Ѫ��
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        //Ѫ��Ϊ0����
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);

        //������Ϊ�ɼ�
        UIbar.gameObject.SetActive(true);
        //��ʾʱ��
        timeLeft = visibleTime;

        //Ѫ���ٷֱȸ�ֵ��fillAmount
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    //ʵ��UI��������
    void LateUpdate()
    {
        if (UIbar != null)
        {
            //����UI�������������
            UIbar.position = barPoint.position;
            //Ѫ����������
            UIbar.forward = -cam.forward;

            //����ʱΪ0�ǣ�����ʾѪ��
            if (timeLeft <= 0 && !alwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
            }
            else
            {
                //��ʱ��
                timeLeft -= Time.deltaTime;
            }
        }
    }
}
