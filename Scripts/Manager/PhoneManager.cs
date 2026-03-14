using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Halabang.UI;
using Halabang.Item;

namespace Halabang.Blueberry.pp {
    /// <summary>
    /// 总控manager，管理所有app的manager 以及全局类操作
    /// </summary>

    public class PhoneManager : MonoBehaviour {
    public enum PHONE_APP {
      Null, Chat, Music, Camera, Album, Calendar, Rhythm, Banking, Map
    }
    public PhoneChatManager _PhoneChatManager => phoneChatManager;
    public PhoneMusicPlayerManager _PhoneMusicPlayerManager => phoneMusicPlayerManager;
    public PhoneCameraManager _PhoneCameraManager => phoneCameraManager;
    public PhoneBankingManager _PhoneBankingManager => phoneBankingManager;
    public PhonePictureManager _PhonePictureManager => phonePictureManager;
    //public PhoneDialogueManager _PhoneDialogueManager => phoneDialogueManager;
    //public PhoneCallManager _PhoneCallManager => phoneCallManager;
    public PhoneCalendarManager _PhoneCalendarManager => phoneCalendarManager;
    public PhoneResultManager _PhoneResultManager => phoneResultManager;
    public PhoneMapManager _PhoneMapManager => phoneMapManager;
    public PhoneTrainingManager _PhoneTrainingManager => phoneTrainingManager;
    public string _phoneOwnerName => phoneOwnerName;
    public Sprite _phoneOwnerPicture => phoneOwnerPicture;
    public bool _showAppName => ShowAppName;
    public Canvas PhoneCanvas => phoneCanvas;
    public Camera PhoneCamera => phoneCamera;
    public DateTime currentTime { get; private set; } = DateTime.Now;
    public bool IsTurnedOn => phoneItem.CurrentState == BasicItem.ItemState.OPEN;
    public PHONE_APP ActiveApp {  get; private set; }

    [Header("setting")]
    [SerializeField] private CanvasGroup triggerCG;
    [SerializeField] private Canvas phoneCanvas;
    [SerializeField] private Camera phoneCamera;
    [SerializeField] private BasicItem phoneItem;

    [Header("manager")]
    [SerializeField] private PhoneChatManager phoneChatManager;
    [SerializeField] private PhoneMusicPlayerManager phoneMusicPlayerManager;
    //[HideInInspector]public PhoneTelephoneManager phoneTelephoneManager;
    [SerializeField] private PhoneCameraManager phoneCameraManager;
    [SerializeField] private PhoneBankingManager phoneBankingManager;
    [SerializeField] private PhonePictureManager phonePictureManager;
    //[SerializeField] private PhoneDialogueManager phoneDialogueManager;
    //[SerializeField] private PhoneCallManager phoneCallManager;
    [SerializeField] private PhoneCalendarManager phoneCalendarManager;
    [SerializeField] private PhoneResultManager phoneResultManager;
    [SerializeField] private PhoneMapManager phoneMapManager;
    [SerializeField] private PhoneTrainingManager phoneTrainingManager;

    [Header("事件")]
    [SerializeField] private UnityEvent openPhoneEvent;
    [SerializeField] private UnityEvent closePhoneEvent;
    public PhoneController PhoneController { get; private set; }
    public void resistcontroller(PhoneController phoneController) {
      PhoneController = phoneController;
    }
    //[HideInInspector]public PhoneAppController PhoneAppController;

    [Header("玩家信息")]
    [SerializeField] private string phoneOwnerName = "Player";
    [SerializeField] private Sprite phoneOwnerPicture;
    [SerializeField] private bool ShowAppName = false;

    public void changeTime(DateTime time) {
      currentTime = time;
    }
    void Start() {
    }

    public void ShowTrigger() {
      triggerCG.Enable(true);
    }
    public void HideTrigger() {
      triggerCG.Disable(false);
    }
    public void TogglePhone() {
      if (IsTurnedOn) {
        ClosePhone();
      } else {
        OpenPhone();
      }
    }
    //show phone
    public void OpenPhone() {
      OpenPhone(PHONE_APP.Null);
    }
    public void OpenPhone(PHONE_APP targetApp = PHONE_APP.Null) {
      Home();
      openApp(targetApp);
      phoneItem.Open();
      openPhoneEvent?.Invoke();
    }
    //hide phone
    public void ClosePhone() {
      phoneItem.Close();
      Home();
      closePhoneEvent?.Invoke();
    }
    //home-return main panel
    public void Home() {
      phoneBankingManager.Close();
      phoneCalendarManager.Close();
      //phoneCallManager.Close();
      phoneCameraManager.Close();
      phoneChatManager.Close();
      //phoneMusicPlayerManager.Close();
      phonePictureManager.Close();
      phoneResultManager.Close();
      phoneMapManager.Close();

      ActiveApp = PHONE_APP.Null;
    }
    public void OpenApp(PHONE_APP targetApp) {
      openApp(targetApp);
    }

    private void openApp(PHONE_APP targetApp) {
      ActiveApp = targetApp;
      switch (targetApp) {
        case PHONE_APP.Chat:
          phoneChatManager.Open();
          break;
        case PHONE_APP.Music:
          phoneMusicPlayerManager.Open();
          break;
        case PHONE_APP.Camera:
          phoneCameraManager.Open();
          break;
        case PHONE_APP.Album:
          phonePictureManager.Open();
          break;
        case PHONE_APP.Calendar:
          phoneCalendarManager.Open();
          break;
        case PHONE_APP.Banking:
          phoneBankingManager.Open();
          break;
        case PHONE_APP.Rhythm:
          phoneResultManager.Open();
          break;
        case PHONE_APP.Map:
          phoneMapManager.Open();
          break;
        default:
          break;
      }
    }
  }
}