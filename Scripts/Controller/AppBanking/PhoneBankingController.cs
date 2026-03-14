using System;
using UnityEngine;
using System.Collections.Generic;
using Halabang.Plugin;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 记账app的controller。
    /// </summary>

    public class PhoneBankingController : PhoneAppController
    {
        [Header("controllers")]
        [SerializeField] private PhoneBankingManager phoneBankingManager;
        [SerializeField] private TransactionController transactionController;
        [Header("引用")]
        [SerializeField] private TextMeshExtend balanceText;
        [SerializeField] private ButtonManagerExt incomeText;
        [SerializeField] private ButtonManagerExt expenditureText;

        public override void Start()
        {
            base.Start();
        }
        //传递allTransactionsDatas
        public void UpdateList(List<TransactionData> transactionDatas)
        {
            transactionController.SetTransactionList(transactionDatas);
        }

        //更新余额ui
        public void UpdateBalanceText(float balance,float income,float expenditure)
        {
            balanceText.SetText(balance + "$");
            incomeText.SetText("收入："+income + "$");
            expenditureText.SetText("支出："+expenditure + "$");
        }

        public override void CloseApp()
        {
            if(_panelStatus==false) return;
            base.CloseApp();
            transactionController.ClosePanel();
        }

    }
}