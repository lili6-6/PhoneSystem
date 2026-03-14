using DG.Tweening;
using Halabang.Plugin;
using Halabang.UI;
using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 对话controller，发送接收消息
    /// </summary>
    public class PhoneDialogueController : MonoBehaviour
    {
        public ScrollRect _dialogueScroll => dialogueScroll;
        public TextMeshExtend _contactName => contactName;
        public AIChatManager _TargetAIChatManager => TargetAIChatManager;
        public PhonePictureController _phonePictureController => phonePictureController;
        public Sprite contactPicture { get; private set; }//对方头像
        public void changeContactPicture(Sprite picture)
        {
            contactPicture = picture;
        }
        public int _encounterWeek => EncounterWeek;
        public int _encounterDay => EncounterDay;
        public void chanLoad(bool Load)
        {
            isLoad = Load;
        }
        public void changePanelStatus(bool Status)
        {
            panelStatus = Status;
        }
        public AIChatManager _MyAIChatManager=> MyAIChatManager;
        public AIChatManager _StoryAIManager => StoryAIManager;
        public ButtonManagerExt _option1 => option1;
        public ButtonManagerExt _option2 => option2;
        public string _optionContent1 => optionContent1;
        public string _optionContent2 => optionContent2;
        public bool _isReplying=> isReplying;
        //[Header("预制件")]
        //[SerializeField][Tooltip("发送信息预制件")] private GameObject sendMesPrefab;//发送信息预制件
        //[SerializeField][Tooltip("收到消息预制件")] private GameObject receiveMesPrefab;//收到消息预制件
        //[SerializeField][Tooltip("发送图片预制件")] private GameObject sendPicPrefab;//发送图片预制件
        //[SerializeField][Tooltip("收到图片预制件")] private GameObject receivePicPrefab;//收到图片预制件
        [Header("引用")]
        [SerializeField][Tooltip("输入框")] private CustomInputFieldExt InputMessage;//输入框
        //[SerializeField][Tooltip("消息列表")] private RectTransform messageList;//消息列表
        [SerializeField][Tooltip("列表组件")] private RectTransform ListView;//列表组件
        [SerializeField][Tooltip("对话滚动条")] private ScrollRect dialogueScroll;//对话滚动条
        [SerializeField][Tooltip("对方名字")] private TextMeshExtend contactName;//对方名字
        [SerializeField][Tooltip("对象aimanager")] private AIChatManager TargetAIChatManager;//aimanager
        [SerializeField]private TextMeshExtend loadingText;//加载文本
        [SerializeField] private AIChatManager MyAIChatManager;//aimanager
        [SerializeField] private AIChatManager StoryAIManager;//故事剧情ai
        [SerializeField]private ButtonManagerExt option1;//选项一
        [SerializeField]private ButtonManagerExt option2;//选项二
        [SerializeField] private phoneMessagesList phoneMessagesList;//消息列表脚本
        private string optionContent1;//选项一内容
        private string optionContent2;//选项二内容
        [Header("事件")]
        [SerializeField] public UnityEvent showPicture;
        [SerializeField] public UnityEvent hidePicture;
        [SerializeField] public UnityEvent showDialoguePanel;
        [SerializeField] public UnityEvent hideDialoguePanel;

        [SerializeField] private PhonePictureController phonePictureController;

        public PhoneDialogueManager phoneDialogueManager { get; private set; }
        public bool isLoad { get; private set; } = false;
        public bool panelStatus { get; private set; } = false;

        private ButtonManagerExt SelectedPicture;//选中的图片
        private List<ChatMessage> chatMessages = new List<ChatMessage>();//聊天内容列表
        private bool isShowedPic = false;
        private int EncounterWeek;
        private int EncounterDay;
        private string storyAIPrompt;
        private bool receiveStoryAIPromptComplete = false;
        private bool isReplying = false;
        

        public  void Start()
        {
            phoneDialogueManager = BlueberryManager.Instance.CurrentPhoneManager._PhoneChatManager._PhoneDialogueManager;
            phoneDialogueManager.resistController(this);
            _phonePictureController.phoneDialogueController = this;

            //UpdateOption("说你好", "说好久不见", "你好", "好久不见");

        }
        public void Opened()
        {

        }

        //更新选项
        public void UpdateOption(string title1,string title2,string content1,string content2)
        {
            option1.SetText(title1);
            option2.SetText(title2);
            optionContent1 = content1;
            optionContent2 = content2;
            option1.TargetButton.Interactable(true);
            option2.TargetButton.Interactable(true);

            phoneDialogueManager.SaveOptions(title1, title2, content1, content2, contactName.GetText);//保存选项

            isReplying= false;
        }
        //发送消息
        public void SendMessage(ButtonManagerExt targetOption)
        {

            if (SelectedPicture == null)
            {
                GameObject newMessage = phoneMessagesList.sendMessage(targetOption);
                

                AddMessageList(newMessage, BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName);//加到聊天内容列表
                //禁用选项按钮
                option1.TargetButton.Interactable(false);
                option2.TargetButton.Interactable(false);

                StartCoroutine(sendToAi(newMessage.GetComponent<messageController>()._Message.GetText,false));
                //InputMessage.TargetInputField.inputText.text = "";//清空  输入版
            }
        }
        /// <summary>
        /// 等待剧情提示词 发送给AI
        /// </summary>
        /// <param name="message">发送内容</param>
        /// <param name="toMe">是不是给玩家扮演角色发送</param>
        /// <returns></returns>
        public IEnumerator sendToAi(string message,bool toMe)
        {
            receiveStoryAIPromptComplete = false;
            isReplying = true;


            if (toMe == true)
            {
                StoryAIManager.Send(chatMessages, message, storyAIPrompt, contactName.GetText);//发送给故事AI
                yield return new WaitUntil(() => receiveStoryAIPromptComplete == true);
                Debug.Log("storyAIPrompt:" + storyAIPrompt);
                MyAIChatManager.Send(chatMessages, message, storyAIPrompt, contactName.GetText);//发送给自己AI
                Debug.Log("发送给自己AI");
            }
            else
            {
                StoryAIManager.Send(chatMessages, message, storyAIPrompt, BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName);//发送给故事AI
                yield return new WaitUntil(() => receiveStoryAIPromptComplete == true);
                Debug.Log("storyAIPrompt:" + storyAIPrompt);
                updateTitle(true);//更新标题
                //调用ChatManager发送消息
                _TargetAIChatManager.Send(chatMessages, message, storyAIPrompt, BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName);//发送
                Debug.Log("发送给对方AI");
            }
                
        }

        //接收StoryAI提示
        public void ReceiveStoryAIPrompt(string prompt)
        {
            receiveStoryAIPromptComplete= true;
            storyAIPrompt = prompt;

            
        }
        public void updateTitle(bool change)
        {
            if (change)
            {
                loadingText.gameObject.SetActive(true);
                contactName.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            }
               
            else
            {
                contactName.gameObject.GetComponent<CanvasGroup>().alpha = 1;
                loadingText.gameObject.SetActive(false);
            }
                
        }
        //接收消息
        public void ReceiveMessage(string message)
        {
            updateTitle(false);//更新标题

            GameObject newMessage = phoneMessagesList.receiveMessage(message);
            AddMessageList(newMessage,contactName.GetText);//添加

            StartCoroutine(sendToAi(message, true));

        }
        //发送图片
        public void InputPic(ButtonManagerExt beSelectPic)
        {
                SelectedPicture = beSelectPic;
           
            if (InputMessage.TargetInputField.inputText.text == null || SelectedPicture != null)
            {
                GameObject newPicture = phoneMessagesList.sendPicture(SelectedPicture);
                AddMessageList(newPicture, BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName);//添加
                SelectedPicture = null;
            }

        }
        //显示或隐藏图片选择栏
        public void showPic()
        {
            if (!isShowedPic)
            {
                DOTween.To(()=> ListView.sizeDelta.y, x=> ListView.sizeDelta=new Vector2(ListView.sizeDelta.x,x), ListView.sizeDelta.y -85, 0.5f);
                isShowedPic = true;
                showPicture.Invoke();
                _phonePictureController.refreshPictureList();

            }
            else if(isShowedPic)
            {
                DOTween.To(() => ListView.sizeDelta.y, x => ListView.sizeDelta = new Vector2(ListView.sizeDelta.x, x), ListView.sizeDelta.y +85, 0.5f);
                isShowedPic = false;
                hidePicture.Invoke();
            }
        }
        //隐藏图片栏
        public void hidePic()
        {
            if (isShowedPic)
            {
                DOTween.To(() => ListView.sizeDelta.y, x => ListView.sizeDelta = new Vector2(ListView.sizeDelta.x, x), ListView.sizeDelta.y + 85, 0.5f);
                isShowedPic = false;
                hidePicture.Invoke();
            }

        }
        //将消息添加到消息列表
        public void AddMessageList(GameObject newMessage,string namestr)
        {
            if (chatMessages.Count == 0)//初次认识=第一次聊天
            {
                EncounterWeek=BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.CurrentDay.Key;
                EncounterDay = (int)BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.CurrentDay.Value;
            }
           
            ChatMessage chatMessage = new ChatMessage();
            
            if (newMessage.GetComponent<messageController>() != null)//文本消息
            {
                
                chatMessage.message = newMessage.GetComponent<messageController>()._Message.GetText;
                chatMessage.Picture = null;
            }
            if (newMessage.GetComponent<message_PicController>() != null)//图片消息
            {
                
                chatMessage.message = null;
                chatMessage.Picture = newMessage.GetComponent<message_PicController>()._Message_Pic.sprite ;
            }
            chatMessage.targetname = contactName.GetText;//聊天对象
            chatMessage.nameStr=namestr;//发送人

            chatMessages.Add(chatMessage);
            foreach (ChatMessage child in chatMessages)
            {
                Debug.Log(child.nameStr + child.message+child.Picture );
            }
            phoneDialogueManager.SaveChat(chatMessages,contactName);//保存
            _dialogueScroll.gameObject.GetComponent<AutoScrollToBottom>().ScrollToBottom();//滑动
        }
        //生成消息
        public void SpawenMessages(string SaveContactName,string message,Sprite pic)
        {
            Debug.Log(SaveContactName+" "+contactName.GetText+" "+ BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName);
            
            chatMessages.Add(phoneMessagesList.spawnMessages(SaveContactName,message,pic));
            _dialogueScroll.gameObject.GetComponent<AutoScrollToBottom>().ScrollToBottom();
        }
        
        //关闭面板
        public void CloseDialoguePanel()
        {
            Debug.Log("关闭面板");
            hideDialoguePanel?.Invoke();
            changePanelStatus(false);
        }
        //电话
        public void Call()
        {
            phoneDialogueManager._phoneCallManager.phoneCallControllers.Call(contactPicture,contactName);
        }
       

    }
}