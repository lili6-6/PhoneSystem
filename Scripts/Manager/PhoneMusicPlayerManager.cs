using UnityEngine;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 音乐appManager，
    /// </summary>
    public class PhoneMusicPlayerManager : MonoBehaviour
    {
        public PhoneMusicPlayerController _phoneMusicPlayerController => phoneMusicPlayerController;
        [Header("设定")]
        [SerializeField] private PhoneMusicPlayerController phoneMusicPlayerController;
        
        //获取打开的controller面板
        public GameObject GetPanelStatus()
        {
            if (phoneMusicPlayerController._panelStatus == true)
                {
                    return phoneMusicPlayerController.gameObject;
                }
           
            return null;
        }
        //关闭打开的面板
        public void Close()
        {
            if(GetPanelStatus()!=null)
            phoneMusicPlayerController.CloseApp();
        }
        public void Open()
        {
            phoneMusicPlayerController.OpenApp();
        }
    }
}