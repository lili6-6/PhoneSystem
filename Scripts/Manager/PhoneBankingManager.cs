using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Halabang.Blueberry.pp
{
    [Serializable]
    public class TransactionData
    {
        public Guid TransactionID;
        public transactionItem.transactionUsage Usage;
        public float Amount;
        public string TradingTarget;
        public string Payer;
        public string Payee;
        public KeyValuePair<int, PhoneDictionary.DayOfWeek> Time;
        public string Memo;
    }

    public class PhoneBankingManager : MonoBehaviour
    {
        [Header("设定")]
        [SerializeField] private PhoneBankingController phoneBankingController;
        [SerializeField] private float initialBalance;

        private float currentBalance;
        private float currentIncome;
        private float currentExpenditure;
        private List<TransactionData> allTransactions = new List<TransactionData>();
        private string saveFilePath;

        public List<TransactionData> _allTransactions => allTransactions;
        public PhoneBankingController _phoneBankingController => phoneBankingController;
        public float CurrentBalance => currentBalance;

        private void Awake()
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, "transactions.json");
        }

        private void Start()
        {
            loadTransactions();
        }

        /// <summary>
        /// 创建交易记录，同时更新余额
        /// </summary>
        public TransactionData CreateTransaction(float _amount, string _target,string _memo=null, transactionItem.transactionUsage _usage=transactionItem.transactionUsage.other)
        {
            var data = new TransactionData
            {
                TransactionID = Guid.NewGuid(),
                Amount = _amount,
                TradingTarget = _target,
                Usage = _usage,
                Time = BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.CurrentDay,
                Memo=_memo,
            };

            if (_amount > 0)
            {
                data.Payee = BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName;
                data.Payer = _target;
            }
            else
            {
                data.Payer = BlueberryManager.Instance.CurrentPhoneManager._phoneOwnerName;
                data.Payee = _target;
            }

            allTransactions.Add(data);
            updateBalance(_amount);
            phoneBankingController.UpdateList(allTransactions);
            saveTransactions();
            return data;
        }

        /// <summary>
        /// 只更新currentBalance和income/expenditure，不修改初始金额
        /// </summary>
        private void updateBalance(float amount)
        {
            currentBalance += amount;
            if (amount > 0) currentIncome += amount;
            else currentExpenditure += Mathf.Abs(amount);

            phoneBankingController.UpdateBalanceText(currentBalance, currentIncome, currentExpenditure);
        }

        /// <summary>
        /// 保存交易和余额到本地
        /// </summary>
        private void saveTransactions()
        {
            try
            {
                TransactionSaveData saveData = new TransactionSaveData
                {
                    Transactions = allTransactions,
                    currentBalance = currentBalance
                };

                string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                File.WriteAllText(saveFilePath, json);
                Debug.Log($"交易已保存到 {saveFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError("保存交易失败: " + e.Message);
            }
        }

        /// <summary>
        /// 加载交易和余额，如果没有文件使用初始余额
        /// </summary>
        private void loadTransactions()
        {
            if (!File.Exists(saveFilePath))
            {
                Debug.Log("没有找到交易保存文件，使用初始余额");
                allTransactions = new List<TransactionData>();
                currentBalance = initialBalance;
                currentIncome = 0;
                currentExpenditure = 0;
                phoneBankingController.UpdateList(allTransactions);
                phoneBankingController.UpdateBalanceText(currentBalance, currentIncome, currentExpenditure);
                return;
            }

            try
            {
                string json = File.ReadAllText(saveFilePath);
                TransactionSaveData saveData = JsonConvert.DeserializeObject<TransactionSaveData>(json);
                if (saveData == null)
                {
                    Debug.LogWarning("交易保存文件为空或格式错误，使用初始余额");
                    allTransactions = new List<TransactionData>();
                    currentBalance = initialBalance;
                    currentIncome = 0;
                    currentExpenditure = 0;
                    phoneBankingController.UpdateList(allTransactions);
                    phoneBankingController.UpdateBalanceText(currentBalance, currentIncome, currentExpenditure);
                    return;
                }

                allTransactions = saveData.Transactions ?? new List<TransactionData>();
                currentBalance = saveData.currentBalance;

                // 重算收入和支出
                currentIncome = 0;
                currentExpenditure = 0;
                foreach (var t in allTransactions)
                {
                    if (t.Amount > 0) currentIncome += t.Amount;
                    else currentExpenditure += Mathf.Abs(t.Amount);
                }

                phoneBankingController.UpdateList(allTransactions);
                phoneBankingController.UpdateBalanceText(currentBalance, currentIncome, currentExpenditure);

                Debug.Log($"交易已加载，总数: {allTransactions.Count}");
            }
            catch (Exception e)
            {
                Debug.LogError("加载交易失败: " + e.Message);
                allTransactions = new List<TransactionData>();
                currentBalance = initialBalance;
                currentIncome = 0;
                currentExpenditure = 0;
                phoneBankingController.UpdateList(allTransactions);
                phoneBankingController.UpdateBalanceText(currentBalance, currentIncome, currentExpenditure);
            }
        }
        
        //关闭打开的面板
        public void Close()
        {
            phoneBankingController.CloseApp();
        }
        public void Open()
        {
            phoneBankingController.OpenApp();

        }

        [Serializable]
        public class TransactionSaveData
        {
            public List<TransactionData> Transactions;
            public float currentBalance;
        }
    }
}
