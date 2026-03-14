using BehaviorDesigner.Runtime.Tasks;
using Halabang.Blueberry;
using Halabang.Blueberry.pp;
using NUnit.Framework;
using UnityEngine;

[TaskCategory("Halabang/Blueberry/pp")]
public class BD_Action_Trade : Action
{
   public enum tradeType
    {
        income,
        expenditure
    }
    [Header("交易类型")]
    public tradeType Type;
    [Header("交易金额")]
    public float Amount=0;
    [Header("交易对象")]
    public string Target;
    [Header("交易备注")]
    public string Memo;
    [Header("交易用途")]
    public transactionItem.transactionUsage Usage;

    public override void OnStart()
    {
        CheckInformation();
        checkAmount();
        creatTrade();
    }
    private TaskStatus CheckInformation()
    {
        if(Amount==0)
        {
            Debug.LogError("金额不能为0");
            return TaskStatus.Failure;
        } 
        else if(Target==null)
        {
            Debug.LogError("交易对象不能为空");
            return TaskStatus.Failure;
        }
        else
        {
            return TaskStatus.Running;
        }
        
    }
    private void checkAmount()
    {
        switch (Type)
        {
            case tradeType.income:
            if(Amount<0) Amount*=-1;
            break;

            case tradeType.expenditure:
            if(Amount>0) Amount*=-1;
            break;
        }
    }
    private TaskStatus creatTrade()
    {
        BlueberryManager.Instance.CurrentPhoneManager._PhoneBankingManager.CreateTransaction(Amount,Target,Memo,Usage);
        return TaskStatus.Success;
    }
        
    
}
