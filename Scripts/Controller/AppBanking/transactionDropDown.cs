using Halabang.Plugin;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Michsky.MUIP;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 显示交易对象列表
    ///抛事件告诉 TransactionController 当前选择
    /// </summary>

    public class transactionDropDown : MonoBehaviour
    {
        public CustomDropdownExt _dropDown=>dropDown;
        [SerializeField] private CustomDropdownExt dropDown;
        [SerializeField] private TransactionController transactionController;

        private List<CustomDropdown.Item> targetList=new List<CustomDropdown.Item>();

        

        //更新下拉菜单
        public void UpdateDropItem(List<string> list)
        {
            refreshIndex();
            var dropdown = dropDown.TargetDropdown;
            dropdown.items.Clear();

            // 构建新 items
            for (int i = 0; i < list.Count; i++)
            {
                var item = new CustomDropdown.Item
                {
                    itemIndex = i,
                    itemName = list[i],
                    OnItemSelection = new UnityEngine.Events.UnityEvent()
                };
                dropdown.items.Add(item);
                targetList.Add(item);
            }

            // 不改变 selectedItemIndex，让 MUIP 显示自己设置的文本
            
            dropdown.SetupDropdown();

            dropdown.selectedText.text = "请选择";  // 仅显示，不影响 items
        }

        //重置下拉菜单
        private void refreshIndex()
        {
            var dropdown = dropDown.TargetDropdown;

            if (dropdown.items == null || dropdown.items.Count == 0)
            {
                Debug.LogWarning("Dropdown 没有任何选项，无法刷新索引");
                return;
            }

            // 1️⃣ 清空已有选项
            dropdown.items.Clear();

            // 2️⃣ 重置索引
            dropdown.selectedItemIndex = 0;

            // 3️⃣ 重建下拉菜单（如果你有默认项可以在这里加回去）
            // 比如添加默认选项
            dropdown.items.Add(new CustomDropdown.Item { itemName = "请选择" });

            // 4️⃣ 重新刷新 UI
            dropdown.SetupDropdown();

            targetList.Clear();
        }

        /// <summary>
        /// 收起下拉菜单（如果是展开状态才收起）
        /// </summary>
        public void CollapseDropdown()
        {
            var dropdown = dropDown.TargetDropdown;
            if (dropdown == null) return;

            // MUIP 的 CustomDropdown 里 isOn 表示当前是否展开
            if (dropdown.isOn)
            {
                dropdown.Animate(); // Animate() 会根据 isOn 切换展开/收起
            }
        }


        //选择交易对象，回调
        public void OnValueChange()
        {
            string target = targetList[dropDown.TargetDropdown.selectedItemIndex].itemName;
            transactionController.OnTargetSelected(target);
            Debug.Log("回调名字：" + target);
        }



    }
}