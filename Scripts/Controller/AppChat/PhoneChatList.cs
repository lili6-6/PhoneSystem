using System.Collections.Generic;
using UnityEngine;

namespace Halabang.Blueberry.pp
{
    public class PhoneChatList : MonoBehaviour
    {
        public List<ChatTargetInformation_EXT> _chatTarget => chatTarget;

        [SerializeField] private RectTransform chatHolder;//联系人列表
        [SerializeField] private GameObject chatPrefab;//联系人预制件
        [SerializeField] private PhoneChatController phoneChatController;

        private List<ChatTargetInformation_EXT> chatTarget = new List<ChatTargetInformation_EXT>();
        private List<ChatTargetInformation> chatTargetInformation;//联系人信息
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            chatTargetInformation = BlueberryManager.Instance.CurrentPhoneManager._PhoneChatManager._chatTargetInformation;
        }

        // Update is called once per frame
        void Update()
        {

        }
        //生成联系人预制件
        public void SpawnChatPrefab()
        {
            phoneChatController.clearHolder(chatHolder);

            for (int i = 0; i < chatTargetInformation.Count; i++)
            {
                GameObject newchat = Instantiate(chatPrefab, chatHolder);
                _chatTarget.Add(newchat.GetComponent<ChatTargetInformation_EXT>());
            }
            refreshChatList();
        }
        //根据通讯录信息刷新通讯录列表
        public void refreshChatList()
        {
            for (int i = 0; i < chatTargetInformation.Count; i++)
            {
                _chatTarget[i].TargetName.SetText(chatTargetInformation[i].TargetName);
                _chatTarget[i].TargetPicture.sprite = chatTargetInformation[i].TargetPicture;
                _chatTarget[i].TargetID = chatTargetInformation[i].TargetID;
            }
        }
    }
}