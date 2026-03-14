using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace Halabang.Blueberry.pp
{

    /// <summary>
    /// 自动滚动到底部
    /// </summary>

    [RequireComponent(typeof(ScrollRect))]
    public class AutoScrollToBottom : MonoBehaviour
    {
        private ScrollRect scrollRect;

        void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        /// <summary>
        /// 新消息加入后调用
        /// </summary>
        public void ScrollToBottom()
        {
            StartCoroutine(ScrollNextFrame());
        }

        private IEnumerator ScrollNextFrame()
        {
            // 等一帧，让 VerticalLayoutGroup 把高度算完
            yield return new WaitForSeconds(0.1f);

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }

    }
}