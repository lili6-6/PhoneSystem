using Halabang.Plugin;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 电话controller。控制电话开关
    /// </summary>
    public class PhoneCallController : MonoBehaviour
    {
        [SerializeField] private RectTransform callPanel;
        [SerializeField] private Image targetPicture;
        [SerializeField] private TextMeshExtend targetName;
        [SerializeField] public UnityEvent open;
        [SerializeField] public UnityEvent close;
        public bool panelStatus { get; private set; } = false;
        public void changePanelStatus(bool status)
        {
            panelStatus = status;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public  void Start()
        {
        }
        
        public void Call(Sprite picture, TextMeshExtend name)
        {
           targetPicture.sprite = picture;
            targetName.SetText(name.GetText);
            open?.Invoke();
            changePanelStatus(true);
        }
        public void EndCall()
        {
            close?.Invoke();
            changePanelStatus(false);
        }
    }
}