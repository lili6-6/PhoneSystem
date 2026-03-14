using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 相机appcontroller。控制相机设置
    /// </summary>

    public class PhoneCameraController : PhoneAppController
    {
        [SerializeField] private Camera afterCamera;
        [SerializeField] private Camera frontCamera;
        [SerializeField]private RawImage cameraPreview;    // 相机预览区域
        [SerializeField]private RenderTexture renderTexture; // 渲染纹理（承载相机画面）
        private Camera currentCamera;

        private PhoneCameraManager phoneCameraManager;

        public override void Start()
        {
            base.Start();
            afterCamera.gameObject.SetActive(true);
            frontCamera.gameObject.SetActive(false);
            currentCamera=afterCamera;
            phoneCameraManager = BlueberryManager.Instance.CurrentPhoneManager._PhoneCameraManager;
        }
       

        // Update is called once per frame
        void Update()
        {

        }
        public void ChangeCamera()
        {
            if(afterCamera.gameObject.activeSelf)
            {
                afterCamera.gameObject.SetActive(false);
                frontCamera.gameObject.SetActive(true);
                currentCamera=frontCamera;
            }
            else
            {
                afterCamera.gameObject.SetActive(true);
                frontCamera.gameObject.SetActive(false);
                currentCamera=afterCamera;
            }
        }
        public void SavePicture()
        {
            phoneCameraManager.SavePicture(cameraPreview, renderTexture,currentCamera);
        }

      
    }
}