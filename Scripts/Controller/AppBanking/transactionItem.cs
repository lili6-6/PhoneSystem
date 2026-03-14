using Halabang.Plugin;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Halabang.Blueberry.pp
{

    /// <summary>
    /// 显示一条交易记录：
    ///金额、类型、时间、交易对象
    ///提供 SetTransactionInfo 方法填数据
    /// </summary>
    public class transactionItem : MonoBehaviour
    {
        public enum transactionUsage
        {
            shopping,// 购物
            dining,// 餐饮
            entertainment,// 娱乐
            transportation,// 交通
            other// 其他
        }
        [SerializeField] private TextMeshExtend tradeTime;// 交易时间
        [SerializeField] private TextMeshExtend tradeType;// 交易类型
        [SerializeField] private TextMeshExtend tradeAmount;// 交易金额
        [SerializeField] private TextMeshExtend tradeMemo;//交易备注

        public TransactionData itemInfo{get;set;}
       

        /// <summary>
        /// 设定交易信息并刷新显示
        /// </summary>
        public void SetTransactionInfo(TransactionData data)
        {
            itemInfo=data;
            itemInfo.Usage= data.Usage;
            itemInfo.Amount = data.Amount;
            itemInfo.TradingTarget = data.TradingTarget;
            itemInfo.Payer = data.Payer;
            itemInfo.Payee = data.Payee;
            itemInfo.Time=data.Time;
            itemInfo.Memo=data.Memo;

            tradeTime.SetText(getDate(data.Time));
            tradeType.SetText(itemInfo.Usage.ToString());
            tradeAmount.SetText(itemInfo.Amount.ToString("C"));
            tradeMemo.SetText(itemInfo.Memo);
        }
        //字符串拼接时间
        private string getDate(KeyValuePair<int, PhoneDictionary.DayOfWeek> date)
        {
            string dateStr = $"week{date.Key} .{date.Value.ToString()}";
            return dateStr;
        }
    }
}