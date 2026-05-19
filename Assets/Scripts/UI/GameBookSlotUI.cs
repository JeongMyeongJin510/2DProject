using Cysharp.Threading.Tasks;
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

    private event Action<string> _onClickSlot;

    private string _slotDataId;

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
        _onClickSlot?.Invoke(_slotDataId);
    }

    private void OnDisable()
    {
        _onClickSlot = null;
    }

    public void InitSlot(string dataId, Action<string> onClickCallback) // TODO : 카테고리에 따라 다른 데이터를 받아올 필요가 있다.
    {
        var itemData = DaniTechGameDataManager.Instance.GetDNItemData(dataId);
        if (itemData == null) return;

        Text_MainName.text = itemData.Name;

        string iconPath = itemData.IconPath;
        if (string.IsNullOrEmpty(iconPath) == true) return;

        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, iconPath).Forget();

        _slotDataId = dataId;

        _onClickSlot += onClickCallback;


    }

    public void SetSelectedUI(bool isSelect)
    {
        GObj_Selected.SetActive(isSelect);
    }
}
