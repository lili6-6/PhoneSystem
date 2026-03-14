using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 交易信息列表controller
    /// </summary>
    public class transactionList : MonoBehaviour
    {
        [SerializeField] private RectTransform itemHolder;
        [SerializeField] private transactionItem itemPrefab;

        //更新item
        public void UpdateItem(List<TransactionData> data)
        {
            foreach (Transform child in itemHolder)
                Destroy(child.gameObject);

            if (data == null || data.Count == 0)
                return;

            // 时间排序
            var sorted = data.OrderByDescending(t => t.Time.Key)
                             .ThenByDescending(t => (int)t.Time.Value)
                             .ToList();

            foreach (var item in sorted)
            {
                var go = Instantiate(itemPrefab, itemHolder);
                go.SetTransactionInfo(item);
                //Debug.Log(go.GetComponent<transactionItem>()._tradeTarget);
            }
        }
    }
}
