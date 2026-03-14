using DG.Tweening;
using Halabang.Blueberry.pp;
using Halabang.Plugin;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 通讯录appManager，管理通讯录列表以及对话列表
    /// </summary>

    public class PhoneChatController : PhoneAppController
    {
        
        public PhoneChatManager _phoneChatManager => phoneChatManager;


        public List<ChatTargetInformation> _chatTargetInformation => chatTargetInformation;//联系人信息
        //public GameObject _dialoguePrefab => dialoguePrefab;
        [Header("列表")]
        [SerializeField] private PhoneChatList phoneChatList;
        [SerializeField] private PhoneDialogueList phoneDialogueList;

        
        
        private PhoneChatManager phoneChatManager;
        private List<ChatTargetInformation> chatTargetInformation;//联系人信息


       public override void Start()
        {
            base.Start();
            phoneChatManager = BlueberryManager.Instance.CurrentPhoneManager._PhoneChatManager;
            chatTargetInformation = phoneChatManager._chatTargetInformation;


            phoneChatList.SpawnChatPrefab();
            phoneDialogueList.SpawnDialoguePrefab();
            

        }
       
      
        
        
       
        public void clearHolder(RectTransform targetHolder)
        {
            if (targetHolder == null || targetHolder.transform == null)
                return;
            List<GameObject> childObjs = new List<GameObject>();
            foreach (Transform child in targetHolder.transform)
            {
                if (child != null) childObjs.Add(child.gameObject);
            }
            foreach (GameObject obj in childObjs)
            {
                if (obj != null) Destroy(obj);
            }
        }

        //打开面板同时设置目标manager信息
        public void OpenDialoguePanel(int index)
        {
            phoneDialogueList._dialogueControllers[index].showDialoguePanel?.Invoke();

            phoneDialogueList._dialogueControllers[index].phoneDialogueManager.LoadChatMessages(phoneDialogueList._dialogueControllers[index], chatTargetInformation[index].TargetName);//加载聊天记录
            phoneDialogueList._dialogueControllers[index].phoneDialogueManager.LoadOptions(phoneDialogueList._dialogueControllers[index], chatTargetInformation[index].TargetName);//加载选项
            phoneDialogueList._dialogueControllers[index]._dialogueScroll.gameObject.GetComponent<AutoScrollToBottom>().ScrollToBottom();//自动滚动
            //_dialogueControllers[index]._phonePictureController.refreshPictureList();
            phoneDialogueList._dialogueControllers[index].changePanelStatus(true);//面板状态
            

        }
        
    }
}