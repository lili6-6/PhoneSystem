using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 电话manager
    /// </summary>
    public class PhoneCallManager : MonoBehaviour
    {
        [SerializeField] public PhoneCallController phoneCallControllers;
        //获取打开的controller面板
        public GameObject GetPanelStatus()
        {
            if (phoneCallControllers.panelStatus == true)
            {
                return phoneCallControllers.gameObject;
            }

            return null;
        }
        public void Close()
        {
            if (GetPanelStatus() != null)
                phoneCallControllers.EndCall();
        }
    }
}