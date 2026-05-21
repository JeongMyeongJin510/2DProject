using Cysharp.Threading.Tasks;
using NUnit.Framework.Interfaces;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameBookSlotUI : MonoBehaviour
{
    [Header("슬롯 기본 정보")]
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private GameObject GObj_Selected;
    [SerializeField] private DaniTechUIButton Button_SlotClick;

    private event Action<string, EGameBookCategory> _onClickSlot;

    private string _slotDataId;
    private EGameBookCategory _curSlotCategory;

    public string GetSlotDataId()
    {
        return _slotDataId;
    }

    private void OnEnable()
    {
        Button_SlotClick.BindOnClickButtonEvent(OnClick_GameBookSlot);
    }

    public void OnClick_GameBookSlot()
    {
        _onClickSlot?.Invoke(_slotDataId, _curSlotCategory);
    }

    private void OnDisable()
    {
        _onClickSlot = null;
    }

    private void SetSlotUI(string dataName, string iconPath)
    {
        Text_MainName.text = dataName;

        if (string.IsNullOrEmpty(iconPath) == false)
        {
            DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, iconPath).Forget();

        }
    }

    public void InitSlot(string dataId, EGameBookCategory curCategory, Action<string, EGameBookCategory> onClickCallback) // TODO : 카테고리에 따라 다른 데이터를 받아올 필요가 있다.
    {
        if(curCategory == EGameBookCategory.ItemCategory)
        {
            var itemData = DaniTechGameDataManager.Instance.GetDNItemData(dataId);
            if (itemData == null) return;

            SetSlotUI(itemData.Name, itemData.IconPath);
        }
        else if (curCategory == EGameBookCategory.MonsterCategory)
        {
            var monsterData = DaniTechGameDataManager.Instance.GetDNMonsterData(dataId);
            if (monsterData == null) return;

            SetSlotUI(monsterData.Name, monsterData.IconPath);

        }
        else if (curCategory == EGameBookCategory.HarvestCategory)
        {
            var fieldObjectData = DaniTechGameDataManager.Instance.GetDNFieldObjectData(dataId);
            if (fieldObjectData == null) return;

            SetSlotUI(fieldObjectData.Name, fieldObjectData.IconPath);
        }


        _slotDataId = dataId;
        _curSlotCategory = curCategory;
        _onClickSlot += onClickCallback;
    }

    public void SetSelectedUI(bool isSelect)
    {
        GObj_Selected.SetActive(isSelect);
    }
}
