using UnityEngine;
using  System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.UI;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 相机appManager。拍摄存储
    /// </summary>


    public class PhoneCameraManager : MonoBehaviour
    {
        public List<PhoneCameraController> _phoneCameraController => phoneCameraController;
        public string _dirPath => dirPath;
        [Header("设定")]
        [SerializeField] private List<PhoneCameraController> phoneCameraController = new List<PhoneCameraController>();
        private string dirPath;
        
        void Start()
        {
            BaseSaveDir();
        }
        
        private void BaseSaveDir()
        {
            
                dirPath = Path.Combine(Application.persistentDataPath, "PhonePictureData");


                if (string.IsNullOrWhiteSpace(dirPath))
                {
                    Debug.LogError("拼接后的路径无效，请检查路径： " + dirPath);
                }

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);  // 创建目录
                }

                
            
        }

        
        /// <summary>
        /// 保存图片（适配BaseSaveDir路径）
        /// </summary>
        /// <param name="cameraPreview">UI预览的RawImage</param>
        /// <param name="renderTexture">渲染纹理</param>
        /// <param name="currentCamera">当前渲染相机</param>

        public void SavePicture(RawImage cameraPreview, RenderTexture renderTexture, Camera currentCamera)
        {
            //创建captureTexture
            Texture2D captureTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            //激活rendertexture读取数据
            RenderTexture.active = renderTexture;
            captureTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            captureTexture.Apply();

            Sprite sprite = Sprite.Create(captureTexture, new Rect(0, 0, captureTexture.width, captureTexture.height), new Vector2(0.5f, 0.5f));
            BlueberryManager.Instance.CurrentPhoneManager._PhonePictureManager.addToPictureList(sprite);
            //恢复状态
            RenderTexture.active = null;
            //保存到文件
            // 确保文件名合法，不包含特殊字符
            string safeFileName = System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + ".png";

            // 修改拼接路径，确保目录名称合法
            string filePath = Path.Combine(Application.persistentDataPath, "PhonePictureData", safeFileName);


            // 确保目标目录存在
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            // 将捕获的图像保存到文件
            byte[] bytes = captureTexture.EncodeToPNG();
            try
            {
                File.WriteAllBytes(filePath, bytes);
                Debug.Log("图片已保存：" + filePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError("保存图片失败：" + e.Message);
            }

        }

        //获取打开的controller面板
        public GameObject GetPanelStatus()
        {
            foreach (PhoneCameraController controller in phoneCameraController)
            {
                if (controller._panelStatus == true)
                {
                    return controller.gameObject;
                }
            }
            return null;
        }
        public void Close()
        {
            if (GetPanelStatus() != null)
            {
                PhoneCameraController target = GetPanelStatus().GetComponent<PhoneCameraController>();
                target.CloseApp();
            } 
        }
        public void Open()
        {
            phoneCameraController[0].OpenApp();
        }

    }
}