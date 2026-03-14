using System.Collections.Generic;
using UnityEngine;

namespace Halabang.Blueberry.pp
{
    
    public class PhoneDialogueList : MonoBehaviour
    {
        public List<PhoneDialogueController> _dialogueControllers => dialogueControllers;

        [SerializeField] private RectTransform dialogueHolder;//聊天面板父级
        [SerializeField] private GameObject dialoguePrefab;//聊天面板预制件
        [SerializeField] private PhoneChatController phoneChatController;


        private List<PhoneDialogueController> dialogueControllers = new List<PhoneDialogueController>();
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
        //生成聊天面板预制件
        public void SpawnDialoguePrefab()
        {
            phoneChatController.clearHolder(dialogueHolder);

            for (int i = 0; i < chatTargetInformation.Count; i++)
            {
                GameObject newDialoguePanel = Instantiate(dialoguePrefab, dialogueHolder);
                _dialogueControllers.Add(newDialoguePanel.GetComponent<PhoneDialogueController>());
            }
            refreshDialogueList();
        }
        public void refreshDialogueList()
        {
            for (int i = 0; i < chatTargetInformation.Count; i++)
            {
                // 目标面板的信息
                _dialogueControllers[i]._contactName.SetText(chatTargetInformation[i].TargetName);//联系人
                _dialogueControllers[i].changeContactPicture(chatTargetInformation[i].TargetPicture);//联系人头像
                

                if (chatTargetInformation[i].apiKey != null && chatTargetInformation[i].appId != null)
                {
                    _dialogueControllers[i]._TargetAIChatManager._client.changeApiKey(chatTargetInformation[i].apiKey);//apikey
                    _dialogueControllers[i]._TargetAIChatManager._client.changeAppId(chatTargetInformation[i].appId);//appid
                    _dialogueControllers[i]._TargetAIChatManager.changeSystemMessage(chatTargetInformation[i].systemMessage);//systemMessage
                    //Debug.Log(_dialogueControllers[i]._contactName.GetText + "的api为：" + chatTargetInformation[i].apiKey);
                    _dialogueControllers[i]._MyAIChatManager._client.changeApiKey(phoneChatController._phoneChatManager._myApiKey);//apikey
                    _dialogueControllers[i]._MyAIChatManager._client.changeAppId(phoneChatController._phoneChatManager._myAppId);//appid
                    _dialogueControllers[i]._MyAIChatManager.changeSystemMessage(phoneChatController._phoneChatManager._mySystemMessages);//清除聊天记录

                    _dialogueControllers[i]._StoryAIManager._client.changeApiKey(phoneChatController._phoneChatManager._storyApiKey);//剧情apikey
                    _dialogueControllers[i]._StoryAIManager._client.changeAppId(phoneChatController._phoneChatManager._storyAppId);//剧情appid
                    _dialogueControllers[i]._StoryAIManager.changeSystemMessage(phoneChatController._phoneChatManager._storySystemMessages);//剧情systemMessage
                }
                _dialogueControllers[i].Opened();
            }
        }
    }
}