using UnityEngine;
using Halabang.Plugin;

namespace Halabang.Blueberry.pp
{


    public class phoneMessagesList : MonoBehaviour
    {
        [Header("预制件")]
        [SerializeField][Tooltip("发送信息预制件")] private GameObject sendMesPrefab;//发送信息预制件
        [SerializeField][Tooltip("收到消息预制件")] private GameObject receiveMesPrefab;//收到消息预制件
        [SerializeField][Tooltip("发送图片预制件")] private GameObject sendPicPrefab;//发送图片预制件
        [SerializeField][Tooltip("收到图片预制件")] private GameObject receivePicPrefab;//收到图片预制件

        [SerializeField][Tooltip("消息列表")] private RectTransform messageList;//消息列表
        [SerializeField]private PhoneDialogueController phoneDialogueController;//对话控制器
        private string optionContent;
        public GameObject sendMessage(ButtonManagerExt targetOption)
        {
            GameObject newMessage = Instantiate(sendMesPrefab, messageList);
            if (targetOption == phoneDialogueController._option1)
            {
                optionContent= phoneDialogueController._optionContent1;
                
                Debug.Log("选项一内容：" + phoneDialogueController._optionContent1);
            }
            else if (targetOption == phoneDialogueController._option2)
            {
                optionContent= phoneDialogueController._optionContent2;
               
                Debug.Log("选项二内容：" + phoneDialogueController._optionContent2);
            }
            newMessage.GetComponent<messageController>().setContent(
                BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerPicture,
                optionContent,
                BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName);
            return newMessage;
           
        }
        public GameObject receiveMessage(string message)
        {
            GameObject newMessage = Instantiate(receiveMesPrefab, messageList);
            
            if (newMessage.GetComponent<messageController>() != null)
            {
                newMessage.GetComponent<messageController>().setContent(
                    phoneDialogueController.contactPicture,
                    message,
                    phoneDialogueController._contactName.GetText);
            }
            else if (newMessage.GetComponent<message_PicController>() != null)
            {
                newMessage.GetComponent<message_PicController>().setContent(
                    phoneDialogueController.contactPicture,
                    null,
                    phoneDialogueController._contactName.GetText);
            }
           return newMessage;
        }
        public GameObject sendPicture(ButtonManagerExt SelectedPicture)
        {
            GameObject newPicture = Instantiate(sendPicPrefab, messageList);//图片消息预制件
            newPicture.GetComponent<message_PicController>().setContent(
                BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerPicture,
                SelectedPicture.BackgroundSprite,
                BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName);
            return newPicture;
        }
        public ChatMessage spawnMessages(string SaveContactName, string message, Sprite pic)
        {
            ChatMessage chatMessage = new ChatMessage();
            if (SaveContactName == phoneDialogueController._contactName.GetText)//对方消息
            {
                Debug.Log("生成" + SaveContactName + "的预制件");

                if (message != null && pic == null)//文本消息
                {
                    GameObject newSaveMessage = Instantiate(receiveMesPrefab, messageList);
                    newSaveMessage.GetComponent<messageController>().setContent(
                        phoneDialogueController.contactPicture,
                        message,
                        phoneDialogueController._contactName.GetText);

                    chatMessage.message = message;
                }
                else if (pic != null)//图片消息
                {
                    GameObject newSavePicture = Instantiate(receivePicPrefab, messageList);
                    newSavePicture.GetComponent<message_PicController>().setContent(
                        phoneDialogueController.contactPicture,
                        pic,
                        phoneDialogueController._contactName.GetText);

                    chatMessage.Picture = pic;
                }
                chatMessage.nameStr = phoneDialogueController._contactName.GetText;

            }
            else if (SaveContactName == BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName)//自己消息
            {
                Debug.Log("生成" + SaveContactName + "的预制件");
                if (pic != null)
                {
                    GameObject newPicture = Instantiate(sendPicPrefab, messageList);
                    newPicture.GetComponent<message_PicController>().setContent(
                        BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerPicture,
                        pic,
                        BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName);

                    chatMessage.Picture = pic;
                }
                else
                {
                    GameObject newMessage = Instantiate(sendMesPrefab, messageList);
                    newMessage.GetComponent<messageController>().setContent(
                        BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerPicture,
                        message,
                        BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName);

                    chatMessage.message = message;
                }

                chatMessage.nameStr = BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName;
            }
            chatMessage.targetname = phoneDialogueController._contactName.GetText;
            return chatMessage;
        }
        //清理聊天内容
        public void ClearList()
        {
            foreach (RectTransform child in messageList)
            {
                Destroy(child.gameObject);
            }
        }
    }
}