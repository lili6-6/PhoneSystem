using DG.Tweening;
using Halabang.Blueberry.pp;
using Halabang.Plugin;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Halabang.Blueberry.pp
{

    /// <summary>
    /// 单个相片的controllerEXT。控制照片显示关闭
    /// </summary>
    public class PhonePictureController_EXT : MonoBehaviour
    {
        public bool _showPicture=>showPicture;
        [SerializeField] private CanvasGroup canvasGroup;

        private PhonePictureManager phonePictureManager;
        public PhonePictureController PictureController { get; private set; }
        
        private RectTransform pictureList;
        private bool showPicture=false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

            phonePictureManager = BlueberryManager.Instance.CurrentPhoneManager._PhonePictureManager;
        }
        public void GetController(PhonePictureController controller)
        {
            PictureController = controller;
        }
        //button调用
        public void OnClick()
        {
            if (PictureController._isPictureApp == true)
            {
                OpenPicture();
            }
            else
            {
                SendPicture();
            }
        }
        private void SendPicture()
        {
            if(PictureController.phoneDialogueController!= null)
            {
                PictureController.phoneDialogueController.InputPic(this.gameObject.GetComponent<ButtonManagerExt>());
            }
        }
        private void OpenPicture()
        {
            if (PictureController._isPictureApp == false)
            {
                return;
            }
            PictureController._TargetPicture.position = this.gameObject.GetComponent<RectTransform>().position;
            PictureController._TargetPicture.localScale = Vector3.zero;

            PictureController._TargetPicture.DOLocalMove(Vector3.zero, phonePictureManager._pictureOpenDuration).SetEase(phonePictureManager._pictureOpenEaseType);
            PictureController._TargetPicture.DOScale(Vector3.one, phonePictureManager._pictureOpenDuration).SetEase(phonePictureManager._pictureOpenEaseType);
            PictureController.openPicture?.Invoke();

            PictureController._pictureDisplay.GetComponent<Image>().sprite = this.GetComponent<ButtonManagerExt>().BackgroundSprite;
            Debug.Log("打开时传入图片：" + this.GetComponent<ButtonManagerExt>().BackgroundSprite.name);
            PictureController.changeIndex(GetIndex());
            Debug.Log("打开时传入index：" + PictureController.index);
            showPicture= true;
        }
        private int GetIndex()
        {
            return this.GetComponent<RectTransform>().transform.GetSiblingIndex();

        }
        public void ClosePicture()
        {
            if (PictureController._isPictureApp == false)
            {
                return;
            }
            PictureController._TargetPicture.DOLocalMove(this.gameObject.GetComponent<RectTransform>().position, phonePictureManager._pictureOpenDuration).SetEase(phonePictureManager._pictureOpenEaseType);

            PictureController._TargetPicture.DOScale(Vector3.zero, phonePictureManager._pictureOpenDuration).SetEase(phonePictureManager._pictureOpenEaseType);
            PictureController.closePicture?.Invoke();
            showPicture= false;

        }
    }
}