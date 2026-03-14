using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using Halabang.Plugin;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 所有appController的父类 除去dialogueController、callController，以及后续不是直接对接app的controller，统一的打开关闭app的方法，同时更新各app面板内的名字
    /// </summary>

    public class PhoneAppController : MonoBehaviour
    {
        [Header("app打开动画")]
        [SerializeField] private float appOpenDuration=0.1f;
        [SerializeField] private Ease openEaseType=Ease.Flash;
        [SerializeField] public UnityEvent openApp;
        [SerializeField] public UnityEvent closeApp;

        [SerializeField] private RectTransform app;
        [SerializeField] private TextMeshExtend appIconText;//app下的名字
        [SerializeField] private TextMeshExtend appNameText;//app界面内的名字
        [SerializeField]private string appName;
        private bool panelStatus = false;
        public bool _panelStatus=>panelStatus;

        private PhoneManager phoneManager;
        private RectTransform targetap;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
       public virtual void Start()
        {
            if(appIconText!=null)
                appIconText.gameObject.SetActive(false);
            phoneManager = BlueberryManager.Instance.CurrentPhoneManager;
            UpdateName();
            if (phoneManager._showAppName)
            {
                ShowAppName();
            }
        }


        //打开app
         public virtual void OpenApp()
        {
            
            this.GetComponent<RectTransform>().localPosition = app.localPosition;
            this.GetComponent<RectTransform>().DOLocalMove(Vector3.zero, appOpenDuration).SetEase(openEaseType);
            this.GetComponent<RectTransform>().localScale = Vector3.zero;
            this.GetComponent<RectTransform>().DOScale(Vector3.one, appOpenDuration).SetEase(openEaseType);
            openApp?.Invoke();
            panelStatus=true;
            targetap=app;
            Debug.Log("openApp");
        }
        //关闭app
        public virtual void CloseApp()
        {
            if (targetap != null)
            {
                this.GetComponent<RectTransform>().DOLocalMove(targetap.localPosition, appOpenDuration).SetEase(openEaseType);
                this.GetComponent<RectTransform>().DOScale(Vector3.zero, appOpenDuration).SetEase(openEaseType);
                Debug.Log("closeApp");
            }

            closeApp?.Invoke();
            panelStatus = false;
            Debug.Log("closeApp");

        }
        //显示app的名字
        public void ShowAppName()
        {
            if(appName!=null)
            {
                appIconText.gameObject.SetActive(true);
                appIconText.SetText(appName);
            }
        }
        //更新app主界面的名字
        public void UpdateName()
        {
            if (appNameText != null)
            {
                appNameText.SetText(appName);
            }
           
        }
    }
}