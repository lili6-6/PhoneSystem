using Halabang.Blueberry.pp;
using Halabang.Plugin;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 用于存储联系人信息 以及方便调用自己打开对应的聊天面板，挂到联系人的button预制件上
    /// </summary>

    public class ChatTargetInformation_EXT : MonoBehaviour
    {
        [SerializeField]public TextMeshExtend TargetName;//聊天对象名字
        [SerializeField] public Image TargetPicture;//聊天对象头像
        [SerializeField] public int TargetID;//对象id


        private int index = 0;
        
        //打开对应聊天面板-索引获取
        public void OpenDialoguePanel()
        {
            index = this.GetComponent<RectTransform>().transform.GetSiblingIndex();
            BlueberryManager.Instance.CurrentPhoneManager._PhoneChatManager._phoneChatController.OpenDialoguePanel(index);
            
        }
    }
}