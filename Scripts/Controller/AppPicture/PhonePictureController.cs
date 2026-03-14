using DG.Tweening;
using Halabang.Plugin;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 相册app的controller。控制app的基本功能
    /// </summary>
    public class PhonePictureController : PhoneAppController
    {
        
        public Image _pictureDisplay => pictureDisplay;
        public RectTransform _TargetPicture => TargetPicture;
        public bool _isPictureApp => isPictureApp;
        public PhonePictureList _pictureList=> pictureList;
        public PhonePictureManager _phonePictureManager => phonePictureManager;

        [Header("引用")]
        [SerializeField]private PhonePictureList pictureList;
        [SerializeField] private RectTransform TargetPicture;
        [SerializeField] private Image pictureDisplay;
        [SerializeField] private bool isPictureApp=false;
        [Header("图片事件")]
        [SerializeField] public UnityEvent openPicture;
        [SerializeField] public UnityEvent closePicture;
        public PhoneDialogueController phoneDialogueController {  get; set; }=null;
        
        public int index { get; private set; } = 0;
        public void changeIndex(int ind)
        {
            index = ind;
        }
        private  PhonePictureManager phonePictureManager;

        public override void Start()
        {
            base.Start();
            phonePictureManager = BlueberryManager.Instance.CurrentPhoneManager._PhonePictureManager;
            phonePictureManager.resistController(this);
        }
        public void spawnPicture(int index,Sprite sprite)
        {
            pictureList.spwenPicture(index,sprite);
        }
        
        public override void OpenApp()
        {
            base.OpenApp();
            refreshPictureList();
        }
        //重新加载
        public void refreshPictureList()
        {
            _phonePictureManager.LoadPicture(this);
            foreach (RectTransform child in pictureList._pictureHolder)
            {
                child.GetComponent<PhonePictureController_EXT>().GetController(this);
            }
        }
        //关闭
        public void ClosePicture()
        {
            if (isPictureApp == false)
            {
                return;
            }
            TargetPicture.DOLocalMove(this.gameObject.GetComponent<RectTransform>().position, phonePictureManager._pictureOpenDuration).SetEase(phonePictureManager._pictureOpenEaseType);

            TargetPicture.DOScale(Vector3.zero, phonePictureManager._pictureOpenDuration).SetEase(phonePictureManager._pictureOpenEaseType);
            closePicture?.Invoke();

        }
        //删除相册
        public void delete()
        {
            phonePictureManager.DeletePicture(index);
            //Debug.Log("删除时传入index：" + index);
            refreshPictureList();
        }
        //重置app
        public void resetPictureList()
        {
            pictureList.resetList();

        }
       
    }
}