using Halabang.Plugin;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 管理交易信息
    /// </summary>

    public class TransactionController : MonoBehaviour
    {
        public List<TransactionData> _incomeList=> incomeList;
        public List<TransactionData> _expenditureList=> expenditureList;
        public enum tradeType
        {
            Income=0,//收入
            Expenditure=1//支出
        }
        [Header("controllers")]
        [SerializeField] private PhoneBankingController phoneBankingController;
        [SerializeField] private transactionDropDown transactionDropDown;
        [SerializeField] private transactionList transactionList;
        [Header("事件")]
        [SerializeField] public UnityEvent OnTransactionOpen;
        [SerializeField] public UnityEvent OnTransactionClose;
        [Header("引用")]
        [SerializeField] private TextMeshExtend title;

        private List<TransactionData> incomeList=new List<TransactionData>();
        private List<TransactionData> expenditureList=new List<TransactionData>();

        private tradeType currentType;
        private string currentTarget;

        //刷新列表信息
        private void updateList()
        {
            Debug.Log("刷新列表");
            List<TransactionData> source =
                currentType == tradeType.Income ? incomeList : expenditureList;

            IEnumerable<TransactionData> temp = source;

            if (!string.IsNullOrEmpty(currentTarget))
            {
                temp = temp.Where(t => t.TradingTarget == currentTarget);
                Debug.Log(currentTarget);
            }

            temp = temp
                .OrderByDescending(t => t.Time.Key)
                .ThenByDescending(t => (int)t.Time.Value);

            transactionList.UpdateItem(temp.ToList());
        }
        //刷新下拉菜单
        private void updateDropDown()
        {
            List<string> targetList=new List<string>();
            if (currentType == TransactionController.tradeType.Income)
            {
                targetList = incomeList.Select(t => t.TradingTarget)
                                                            .Distinct()
                                                            .ToList();
            }
            else if (currentType == TransactionController.tradeType.Expenditure)
            {
                targetList = expenditureList.Select(t => t.TradingTarget)
                                                                  .Distinct()
                                                                  .ToList();
            }
            transactionDropDown.UpdateDropItem(targetList);
            if (targetList.Count == 0) return;
            
        }
        //选择对象并刷新
        public void OnTargetSelected(string target)
        {
            currentTarget = target;
            updateList();
        }
        //设定收入和支出列表
        public void SetTransactionList(List<TransactionData> allList)
        {
            expenditureList = allList.Where(t => t.Amount < 0).ToList();
            incomeList= allList.Where(tag => tag.Amount > 0).ToList();
        }
        //打开面板
        public void OpenPanel(bool isIncome)
        {
            OnTransactionOpen?.Invoke();

            currentType = isIncome ? tradeType.Income : tradeType.Expenditure;
            currentTarget = null; // 默认显示全部

            title.SetText(currentType.ToString());

            updateDropDown();
            updateList();
        }

        public void ClosePanel()
        {
            OnTransactionClose?.Invoke();
            transactionDropDown.CollapseDropdown();
        }
    }
}