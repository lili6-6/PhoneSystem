using System.Collections.Generic;
using UnityEngine;
using Halabang.Plugin;

namespace Halabang.Blueberry.pp
{
    public class PhonePictureList : MonoBehaviour
    {
        public RectTransform _pictureHolder => PictureHolder;
        public GameObject _pictureItemPrefab => pictureItemPrefab;

        [SerializeField] private RectTransform PictureHolder;
        [SerializeField] private GameObject pictureItemPrefab;
        [SerializeField] private PhonePictureController phonePictureController;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
       
        public void resetList()
        {
            if (PictureHolder == null)
                return;

            List<GameObject> childObjects = new List<GameObject>();
            foreach (Transform child in PictureHolder)
            {
                if (child != null)
                {
                    childObjects.Add(child.gameObject);
                }
            }
            foreach (GameObject childObj in childObjects)
            {
                if (childObj != null)
                {
                    Destroy(childObj);
                }
            }
        }
        public void spwenPicture(int index, Sprite sprite)
        {
            if (index >= _pictureHolder.childCount)
            {
                //生成新的图片项
                GameObject newPictureItem = Instantiate(pictureItemPrefab,_pictureHolder);
                //continue;
            }
            PictureHolder.GetChild(index).GetComponent<ButtonManagerExt>().SetBackground(sprite);
            PictureHolder.GetChild(index).GetComponent<CanvasGroup>().interactable = true;
            PictureHolder.GetChild(index).GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}