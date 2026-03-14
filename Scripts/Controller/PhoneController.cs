using Halabang.Plugin;
using System;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 手机总控controller，用于手机的总控的ui
    /// </summary>

    public class PhoneController : MonoBehaviour
    {
        [Tooltip("状态栏时间")]
        [SerializeField] private TextMeshExtend StatusDisplayTime;

        
        private float _timer = 0f;
        private PhoneManager phoneManager;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            phoneManager = BlueberryManager.Instance.CurrentPhoneManager;
            phoneManager.resistcontroller(this);

            UpdateTime();
        }
       

        void Update()
        {
            _timer+= Time.deltaTime;
            if (_timer >= 60f)
            {
                phoneManager.changeTime(phoneManager.currentTime.AddMinutes(1) );
                UpdateTime();
                _timer = 0f;
            }

        }
        //更新时间显示
        public void UpdateTime()
        {
            StatusDisplayTime.SetText(phoneManager.currentTime.ToString("HH:mm"));
            //Debug.Log("时间已更新为"+ currentTime.ToString("HH:mm"));
        }
        
    }
}