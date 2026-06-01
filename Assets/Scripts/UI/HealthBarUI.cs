using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;
    public bool alwaysVisible;
    public float visibleTime;
    private float timeLeft;

    Image healthSlider;
    Transform UIbar;
    Transform cam;

    CharacterStats currentStats;

    private void Awake()
    {
        currentStats = GetComponentInParent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())   //在场景中找到所有的Canvas组件
        {
            if (canvas.renderMode == RenderMode.WorldSpace)   //如果这个Canvas是屏幕空间覆盖模式的，就把它作为UIbar的父对象
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;   //实例化血条UI，并把它放在这个Canvas下面
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();   //获取血条UI的第一个子对象的Image组件，这个Image组件就是血条的填充部分
                UIbar.gameObject.SetActive(alwaysVisible);   //如果alwaysVisible是true，就让血条UI一直显示，否则就隐藏
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
        {
            Destroy(UIbar.gameObject);
        }
        UIbar.gameObject.SetActive(true);   //每次更新血条的时候都让它显示出来
        timeLeft = visibleTime;   //重置可见时间
        float sliderPercent = (float)currentHealth / maxHealth;   //计算当前血量占最大血量的百分比
        healthSlider.fillAmount = sliderPercent;   //设置血条的填充量为这个百分比
    }

    private void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barPoint.position;   //让血条UI的位置跟随barPoint的位置
            UIbar.forward = -cam.forward;   //让血条UI始终面向摄像机

            if(timeLeft <= 0 && !alwaysVisible)   //如果可见时间已经到了，并且不是一直可见的，就隐藏血条UI
            {
                UIbar.gameObject.SetActive(false);
            }
            else
            {
                timeLeft -= Time.deltaTime;   //否则就减少可见时间
            }
        }
    }
}
