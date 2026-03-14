using DG.Tweening.Plugins.Core.PathCore;
using Halabang.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 对话manager，聊天记录保存加载
    /// </summary>
    [Serializable]
    public class ChatMessage
    {
        public string targetname;//聊天对象
        public string message;//发送内容
        public Sprite Picture;//发送图片
        public string nameStr;//发送人
        public byte[] pictureData; // 用于序列化图片的字节数组
    }

    public class PhoneDialogueManager : MonoBehaviour
    {
        public PhoneCallManager _phoneCallManager=> phoneCallManager;
        [Header("manager")]
        [SerializeField] private PhoneChatManager phoneChatManager;
        [SerializeField]private PhoneCallManager phoneCallManager;
        
        public List<PhoneDialogueController> phoneDialogueControllers { get; private set; } = new List<PhoneDialogueController>();//聊天面板列表
        public void resistController(PhoneDialogueController controller)
        {
            phoneDialogueControllers.Add(controller);
        }
         private List<ChatMessage> chatMessages = new List<ChatMessage>();// 当前加载的联系人消息列表
        
        
        // 基础保存目录（所有联系人的JSON都存在这里）
        private string BaseSaveDir
        {
            get
            {
                string dirPath = System.IO.Path.Combine(Application.persistentDataPath, "PhoneChatData");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                return dirPath;
            }
        }
        //选项保存路径
        private string OptionSaveDir
        {
            get
            {
                string dir = System.IO.Path.Combine(Application.persistentDataPath,"PhoneChatData","Options");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return dir;
            }
        }


        // 核心：根据联系人名称生成专属文件路径
        private string GetSaveFilePath(string contactName)
        {
            // 过滤非法字符（避免文件名包含/、\、:等导致创建失败）
            string safeName = SanitizeFileName(contactName);
            // 拼接路径：基础目录/联系人名称.json
            return System.IO.Path.Combine(BaseSaveDir, $"{safeName}.json");
        }
        //根据联系人名称生成选项对话路径
        private string GetOptionFilePath(string contactName)
        {
            string safeName = SanitizeFileName(contactName);
            return System.IO.Path.Combine(OptionSaveDir, $"{safeName}_OptionDialogue.json" );
        }


        // 辅助方法：过滤文件名非法字符
        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "未知联系人";
            // 替换Windows/OSX/Linux的非法字符
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c.ToString(), "_");
            }
            return fileName.Trim();
        }

        // 保存聊天消息
        // 优化后的 SaveChat 方法：异步保存
        public async void SaveChat(List<ChatMessage> chatMessages, TextMeshExtend contactName)
        {
            if (contactName == null)
            {
                Debug.LogError("SaveChat：contactName TextMeshExtend 组件为空！");
                return;
            }

            string contactNameStr = contactName.GetText;
            if (string.IsNullOrEmpty(contactNameStr))
            {
                Debug.LogWarning("SaveChat：联系人名称为空，使用默认名称");
                contactNameStr = "未知联系人";
            }

            if (chatMessages == null || chatMessages.Count == 0)
            {
                Debug.LogWarning($"SaveChat：[{contactNameStr}] 传入的消息列表为空，跳过保存");
                return;
            }

            try
            {
                // 将图片转换为字节数组
                foreach (var chatMessage in chatMessages)
                {
                    chatMessage.pictureData = SpriteToByteArray(chatMessage.Picture);  // 转换图片为字节数组
                }

                ChatMessageWrapper wrapper = new ChatMessageWrapper();
                wrapper.chatMessages = chatMessages;

                string jsonData = JsonUtility.ToJson(wrapper, true);

                string savePath = GetSaveFilePath(contactNameStr);

                // 异步写入文件
                await File.WriteAllTextAsync(savePath, jsonData);

                Debug.Log($"[{contactNameStr}] 消息保存成功！共 {chatMessages.Count} 条，路径：{savePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[{contactNameStr}] 保存消息失败：{e.Message}\n{e.StackTrace}");
            }
        }

        // 将 Sprite 转换为 byte[]
        private byte[] SpriteToByteArray(Sprite sprite)
        {
            if (sprite == null) return null;

            Texture2D texture = sprite.texture;
            byte[] byteArray = texture.EncodeToPNG();  // 将 Texture2D 编码为 PNG 格式字节数组
            return byteArray;
        }
        // 加载聊天消息
        public void LoadChatMessages(PhoneDialogueController dialogue, string contactName = "未知联系人")
        {
            if (dialogue.isLoad) return;

            string loadPath = GetSaveFilePath(contactName);
            if (!File.Exists(loadPath))
            {
                Debug.Log($"[{contactName}] 无保存的消息文件，路径：{loadPath}");
                chatMessages = new List<ChatMessage>();
                return;
            }

            string jsonData = File.ReadAllText(loadPath);
            ChatMessageWrapper wrapper = JsonUtility.FromJson<ChatMessageWrapper>(jsonData);

            if (wrapper != null && wrapper.chatMessages != null)
            {
                chatMessages = wrapper.chatMessages;
                Debug.Log($"[{contactName}] 消息加载成功！共 {chatMessages.Count} 条");

                for (int i = 0; i < chatMessages.Count; i++)
                {
                    try
                    {
                        ChatMessage currentMsg = chatMessages[i];
                        if (currentMsg == null)
                        {
                            Debug.LogWarning($"[{contactName}] 第{i + 1}条消息为空，跳过");
                            continue;
                        }

                        string msgNameStr = currentMsg.nameStr ?? "未知发送人";
                        string msgContent = currentMsg.message ?? "空消息";
                        Sprite msgPicture = ByteArrayToSprite(currentMsg.pictureData);  // 从字节数组恢复 Sprite

                        Debug.Log($"- 消息 {i + 1}: 发送人={msgNameStr}, 内容={msgContent}, 图片={msgPicture}");

                        if (dialogue != null)
                        {
                            dialogue.SpawenMessages(msgNameStr, msgContent, msgPicture);
                            Debug.Log($"第{i + 1}条消息预制件生成成功");
                        }
                        else
                        {
                            Debug.LogError($"[{contactName}] 第{i + 1}条消息：dialogue 为空，无法生成预制件");
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[{contactName}] 第{i + 1}条消息处理失败：{ex.Message}\n{ex.StackTrace}");
                        continue;
                    }
                }
            }
            else
            {
                chatMessages = new List<ChatMessage>();
            }

            dialogue.chanLoad(true);
        }

        // 从 byte[] 恢复 Sprite
        private Sprite ByteArrayToSprite(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;

            Texture2D texture = new Texture2D(2, 2);  // 创建一个临时的 Texture2D 对象
            texture.LoadImage(byteArray);  // 将字节数组加载到纹理

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        //保存选项
        public void SaveOptions(string tit1, string tit2, string con1, string con2,string contactName)
        {
            OptionDialogue data = new OptionDialogue
            {
                title1 = tit1,
                title2 = tit2,
                content1 = con1,
                content2 = con2
            };

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(GetOptionFilePath(contactName), json);

        }
        //读取选项
        public void LoadOptions(PhoneDialogueController dialogue,string contactName)
        {
            if (dialogue._isReplying) return;
            string path = GetOptionFilePath(contactName);
            if (!File.Exists(path)) return;

            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json)) return;

            OptionDialogue data = JsonUtility.FromJson<OptionDialogue>(json);
            if (data == null) return;

            dialogue.UpdateOption(data.title1,data.title2,data.content1,data.content2);
        }




        //获取打开的controller面板
        public GameObject GetPanelStatus()
        {
            foreach(PhoneDialogueController controller in phoneDialogueControllers)
            {
                if (controller.panelStatus == true)
                {
                    return controller.gameObject;
                }
            }
            return null;
        }
       //关闭打开的面板
        public void Close()
        {
            if (GetPanelStatus() != null)
            {
                PhoneDialogueController target = GetPanelStatus().GetComponent<PhoneDialogueController>();
                target.CloseDialoguePanel();
                _phoneCallManager.Close();
            }
        }
        public void Open(string name)
        {
            int index= 0;
            foreach(ChatTargetInformation target in phoneChatManager._chatTargetInformation)
            {
                if(target.TargetName == name)
                {
                    index= target.TargetID;
                }
            }
            phoneChatManager._phoneChatController.OpenDialoguePanel(index);

        }


    }

    [Serializable]
    public class ChatMessageWrapper
    {
        public List<ChatMessage> chatMessages;
    }

    [System.Serializable]
    public class OptionDialogue
    {
        public string title1;
        public string title2;
        public string content1;
        public string content2;
    }

}
