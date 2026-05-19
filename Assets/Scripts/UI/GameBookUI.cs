using Cysharp.Threading.Tasks;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBookUI : DaniTechUIBase
{
    [Header("동적 생성 할 프리팹")]
    [SerializeField] private GameObject Prefab_Slot;

    [Header("디테일 정보 영역")]
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private Text Text_Description;
    [SerializeField] private DaniTechUIButton Button_CloseUI;

    //[Header("부가 정보")]
    //[SerializeField] private GameObject Layout_SubInfoSkill; 

    [Header("슬롯 리스트 영역")]
    [SerializeField] private Transform Transform_SlotRoot;

    private  Dictionary<string, GameBookSlotUI> _slotList = new Dictionary<string, GameBookSlotUI> ();

    private void OnEnable()
    {
        ReadItemLstAndCreateSlot();

        Button_CloseUI.BindOnClickButtonEvent(OnClick_CloseGameBookUI);
    }

    public void OnClick_CloseGameBookUI()
    {
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.GameBookUI);
    }

    private void OnDisable()
    {
        if(_slotList.Count > 0)
        {
            foreach (var slotKv in _slotList)
            {
                var slot = slotKv.Value;
                DestroyImmediate(slot.gameObject);
            }


            _slotList.Clear();
        }
    }
    private void ReadItemLstAndCreateSlot()
    {
        var dataList = DaniTechGameDataManager.Instance.ItemDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if (data == null) continue; // 데이터가 Null 일수 있으니 체크

            CreateGameBookSlot(data.Id);
           
        }


        if (_slotList.Count > 0)
        {
            foreach(var slotKv in _slotList)
            {
                var slot = slotKv.Value;
                slot.OnClick_GameBookSlot();
            }
        }
    }

    private void CreateGameBookSlot(string dataId)
    {
        var gObj = Instantiate(Prefab_Slot, Transform_SlotRoot);
        if (gObj == null) return;

        var slotComponent = gObj.GetComponent<GameBookSlotUI>();
        if (slotComponent == null) return;

        slotComponent.InitSlot(dataId, OnClickChildSlotSelected);
        _slotList.Add(dataId, slotComponent);

    }

    private void OnClickChildSlotSelected(string slotDataId)
    {
        var currentSelectedData = DaniTechGameDataManager.Instance.GetDNItemData(slotDataId);
        if (currentSelectedData == null) return;

        //Image_MainIcon;
        Text_MainName.text = currentSelectedData.Name;
        Text_Description.text = currentSelectedData.Description;
        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, currentSelectedData.IconPath).Forget();

        foreach(var slotKv in _slotList)
        {
            var slot = slotKv.Value;
            var dataId = slot.GetSlotDataId();
            slot.SetSelectedUI(slotDataId == dataId);
        }
    }
}
