using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    //UIprafab
    public GameObject healthUIPrefab;
    //人物设定的血条位置坐标
    public Transform barPoint;
    //长久可见的布尔值
    public bool alwaysVisible;
    //可视化时间
    public float visibleTime;
    private float timeLeft;

    //滑动条
    Image healthSlider;
    //血条
    Transform UIbar;
    //摄像机位置
    Transform cam;
    //拿到血量数据
    CharacterStats currentStats;

    void Awake()
    {
        //拿到数据组件
        currentStats = GetComponent<CharacterStats>();
        //为事件订阅更新血量方法
        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    void OnEnable()
    {
        //相机位置
        cam = Camera.main.transform;

        //遍历现有的Canvas
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            //判断Canvas是否是世界坐标渲染
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                //生成prefab，拿到血条的坐标
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;
                //拿到子组件 图片
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                //设置为长久可见
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    //更新血量
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        //血量为0销毁
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);

        //先设置为可见
        UIbar.gameObject.SetActive(true);
        //显示时间
        timeLeft = visibleTime;

        //血量百分比赋值给fillAmount
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    //实现UI跟随人物
    void LateUpdate()
    {
        if (UIbar != null)
        {
            //更新UI的坐标跟随人物
            UIbar.position = barPoint.position;
            //血条面对摄像机
            UIbar.forward = -cam.forward;

            //倒计时为0是，不显示血条
            if (timeLeft <= 0 && !alwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
            }
            else
            {
                //计时器
                timeLeft -= Time.deltaTime;
            }
        }
    }
}
