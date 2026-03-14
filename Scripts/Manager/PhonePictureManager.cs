using DG.Tweening;
using Halabang.Plugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 相册app的manager图片读取
    /// </summary>
    public class PhonePictureManager : MonoBehaviour
    {
        public float _pictureOpenDuration => pictureOpenDuration;
        public Ease _pictureOpenEaseType => pictureOpenEaseType;
        public Sprite _InitialPicture => InitialPicture;
        [Header("打开图片设置")]
        [SerializeField]private float pictureOpenDuration=0.1f;
        [SerializeField] private Ease pictureOpenEaseType=Ease.Flash;
        [SerializeField] private Sprite InitialPicture;
        public List<PhonePictureController> phonePictureControllers { get; private set; } =new List<PhonePictureController>();
        public void resistController(PhonePictureController controller)
        {
            phonePictureControllers.Add(controller);
        }
        public List<Sprite> pictureList { get; private set; } = new List<Sprite>();
        public void addToPictureList(Sprite picture)
        {
            pictureList.Add(picture);
        }

        /// <summary>
        /// 加载相册数据（修复原代码的路径和序列化问题）
        /// </summary>
        public void LoadPicture(PhonePictureController pictureController)
        {
            pictureList.Clear();

            string[] files = Directory.GetFiles(BlueberryManager.Instance.CurrentPhoneManager._PhoneCameraManager._dirPath, "*.Png");

           for(int i = 0; i < files.Length; i++)
            {
                byte[] picBytes = File.ReadAllBytes(files[i]);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(picBytes); // 载入图片字节流
               
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)); // 转为Sprite
                pictureController.spawnPicture(i, sprite);
                //PictureHolder.GetChild(i).GetComponent<ButtonManagerExt>().SetBackground(sprite);
                //PictureHolder.GetChild(i).GetComponent<CanvasGroup>().interactable = true;
                //PictureHolder.GetChild(i).GetComponent<CanvasGroup>().blocksRaycasts = true;
                Debug.Log("设置图片项：" + sprite.name+"载入sprite："+ pictureController._pictureList._pictureHolder.GetChild(i).GetComponent<ButtonManagerExt>().BackgroundSprite);
                
                
                pictureList.Add(sprite);
            }
            Debug.Log(pictureList.Count);

        }
        public void DeletePicture(int index)
        {
            List<GameObject> deleteList = new List<GameObject>();

            foreach (PhonePictureController controller in phonePictureControllers)
            {
                // 防护1：判空控制器本身（避免访问已销毁的controller）
                if (controller == null)
                {
                    Debug.LogWarning("跳过已销毁的PhonePictureController");
                    continue;
                }
                // 防护2：判空_pictureList（核心！避免访问已销毁的RectTransform）
                RectTransform pictureList = controller._pictureList._pictureHolder;
                if (pictureList == null)
                {
                    Debug.LogWarning($"PhonePictureController的_pictureList为空/已销毁，跳过");
                    continue;
                }
                // 防护3：校验childCount，避免索引越界（childCount-1 ≥ 0）
                if (pictureList.childCount <= 0)
                {
                    Debug.LogWarning($"pictureList无子女物体，跳过");
                    continue;
                }
                // 防护4：获取子物体并判空（避免访问已销毁的子物体）
                Transform targetChild = pictureList.GetChild(pictureList.childCount - 1);
                if (targetChild == null)
                {
                    Debug.LogWarning($"pictureList最后一个子物体已销毁，跳过");
                    continue;
                }

                // 安全添加到删除列表
                deleteList.Add(targetChild.gameObject);
            }

            // 批量销毁（再次判空，避免重复销毁）
            foreach (GameObject target in deleteList)
            {
                if (target != null)
                {
                    Destroy(target);
                    Debug.Log($"已销毁目标物体：{target.name}");
                }
            }
            Debug.Log("准备删除图片，索引：" + index);
            string[] files = Directory.GetFiles(BlueberryManager.Instance.CurrentPhoneManager._PhoneCameraManager._dirPath, "*.Png");
            if (index < 0 || index >= files.Length)
            {
                Debug.LogError("索引超出范围，无法删除图片。");
                return;
            }
            // 删除文件
            File.Delete(files[index]);
            Debug.Log("已删除图片文件: " + files[index]);
            pictureList.RemoveAt(index);
            
            
        }
        //获取打开的controller面板
        public GameObject GetPanelStatus()
        {
            foreach (PhonePictureController controller in phonePictureControllers)
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
                PhonePictureController target = GetPanelStatus().GetComponent<PhonePictureController>();
                target.CloseApp();
                target.ClosePicture();
            }
        }
        public void Open()
        {
            foreach(PhonePictureController controller in phonePictureControllers)
            {
                if (controller._isPictureApp)
                {
                    controller.OpenApp();
                }
            }
        }

        [System.Serializable]
        private class AlbumDataContainer
        {
            public List<string> picturePaths = new List<string>(); // 存储图片文件路径
        }



    }

}