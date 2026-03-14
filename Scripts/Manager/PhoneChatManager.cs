using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

namespace Halabang.Blueberry.pp {
  /// <summary>
  /// 通讯录消息列表
  /// </summary>
  [System.Serializable]
  public class ChatTargetInformation {
    [Tooltip("聊天对象")] public string TargetName;
    [Tooltip("对象头像")] public Sprite TargetPicture;
    [Tooltip("id")] public int TargetID;
    [Header("AI配置")]
    public string apiKey = "sk-8feaa601bd574b8484b4a86f6218a1fd";     // sk-xxx
    public string appId = "1e5e3648a0d54dfc9e5895ff092e515e";      // 你的百炼应用ID
    [TextArea(6, 20)]
    public string systemMessage = string.Empty;
  }
  /// <summary>
  /// 对话appManager
  /// </summary>
  public class PhoneChatManager : MonoBehaviour {
    public List<ChatTargetInformation> _chatTargetInformation => ChatTargetInformation;
    public PhoneDialogueManager _PhoneDialogueManager => phoneDialogueManager;
    public PhoneChatController _phoneChatController => phoneChatController;
    public string _mySystemMessages => mySystemMessages;
    public string _myApiKey => myApiKey;
    public string _myAppId => myAppId;
    public string _storyApiKey => storyApiKey;
    public string _storyAppId => storyAppId;
    public string _storySystemMessages => storySystemMessages;
        [Header("设定")]
    [SerializeField] private PhoneChatController phoneChatController;
    [Header("通讯列表信息")]
    [SerializeField] private List<ChatTargetInformation> ChatTargetInformation = new List<ChatTargetInformation>();
    [SerializeField] private PhoneDialogueManager phoneDialogueManager;
    [Header("AI配置")]
    [SerializeField]private string myApiKey;
    [SerializeField] private string myAppId;
    [TextArea(6, 20)]
    [SerializeField] private string mySystemMessages;
    [SerializeField] private string storyApiKey;
    [SerializeField] private string storyAppId;
    [TextArea(6, 20)]
    [SerializeField] private string storySystemMessages;



        //获取打开的controller面板
        public GameObject GetPanelStatus() {
      if (phoneChatController._panelStatus == true) {
        return phoneChatController.gameObject;
      }

      return null;
    }
    public void Open() {
       phoneChatController.OpenApp();
    }
    public void Close() {
            
      if(GetPanelStatus() != null)
      {
        phoneChatController.CloseApp();
                _PhoneDialogueManager.Close();
      }
     
    }

  }
}