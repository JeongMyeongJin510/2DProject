using Cysharp.Threading.Tasks;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EGameBookCategory
{
    None = 0,
    ItemCategory,
    MonsterCategory,
    HarvestCategory
}

public class GameBookUI : DaniTechUIBase
{
    [Header("동적 생성 할 프리팹")]
    [SerializeField] private GameObject Prefab_Slot;

    [Header("디테일 정보 영역")]
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private Text Text_Description;
    [SerializeField] private DaniTechUIButton Button_CloseUI;

    [Header("상단 카테고리")]
    [SerializeField] private DaniTechUIButton Button_ItemCategory;
    [SerializeField] private DaniTechUIButton Button_MonsterCategory;
    [SerializeField] private DaniTechUIButton Button_HarvestCategory;





    //[Header("부가 정보")]
    //[SerializeField] private GameObject Layout_SubInfoSkill; 

    [Header("슬롯 리스트 영역")]
    [SerializeField] private Transform Transform_SlotRoot;

    private  Dictionary<string, GameBookSlotUI> _slotList = new Dictionary<string, GameBookSlotUI> ();

    private void OnEnable()
    {
        OnClick_ItemCategory();

        Button_CloseUI.BindOnClickButtonEvent(OnClick_CloseGameBookUI);
        Button_ItemCategory.BindOnClickButtonEvent(OnClick_ItemCategory);
        Button_MonsterCategory.BindOnClickButtonEvent(OnClick_MonsterCategory);
        Button_HarvestCategory.BindOnClickButtonEvent(OnClick_HarvestCategory);




    }


    private void OnDisable()
    {
        DestroyAndClearrSlotList();
    }

    private void DestroyAndClearrSlotList()
    {
        if (_slotList.Count > 0)
        {
            foreach (var slotKv in _slotList)
            {
                var slot = slotKv.Value;
                DestroyImmediate(slot.gameObject);
            }


            _slotList.Clear();
        }
    }


    public void OnClick_CloseGameBookUI()
    {
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.GameBookUI);
    }

    public void OnClick_ItemCategory()
    {
        SetGameBookLayoutByCategory(EGameBookCategory.ItemCategory);
    }

    public void OnClick_MonsterCategory()
    {
        SetGameBookLayoutByCategory(EGameBookCategory.MonsterCategory);

    }

    public void OnClick_HarvestCategory() 
    {
        SetGameBookLayoutByCategory(EGameBookCategory.HarvestCategory);

    }

    private void SetGameBookLayoutByCategory(EGameBookCategory category)
    {

        DestroyAndClearrSlotList();

        switch (category)
        {
            case EGameBookCategory.ItemCategory:
                ReadItemLstAndCreateSlot();
                break;
            case EGameBookCategory.MonsterCategory:
                ReadMonsterListAndCreateSlot();
                break;
            case EGameBookCategory.HarvestCategory:
                ReadHarvestListAndCreateSlot();
                break;
            default:
                break;
        }
    }


    private void ReadItemLstAndCreateSlot()
    {
        var dataList = DaniTechGameDataManager.Instance.ItemDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if (data == null) continue; 

            CreateGameBookSlot(data.Id, EGameBookCategory.ItemCategory);
           
        }
        SelectFirstSlot();
    }

    private void ReadMonsterListAndCreateSlot()
    {
        var dataList = DaniTechGameDataManager.Instance.MonsterDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if (data == null) continue;

            CreateGameBookSlot(data.Id, EGameBookCategory.MonsterCategory);

        }
        SelectFirstSlot();
    }

    private void ReadHarvestListAndCreateSlot()
    {
        var dataList = DaniTechGameDataManager.Instance.FieldObjectDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if (data == null) continue;


            if (data.FieldObjectType != "Harvest") continue;


            CreateGameBookSlot(data.Id, EGameBookCategory.HarvestCategory);

        }

        SelectFirstSlot();
    }

    private void SelectFirstSlot()
    {
        if (_slotList.Count > 0)
        {
            foreach (var slotKv in _slotList)
            {
                var slot = slotKv.Value;
                slot.OnClick_GameBookSlot();
            }
        }
    }



    private void CreateGameBookSlot(string dataId, EGameBookCategory curCategory)
    {
        var gObj = Instantiate(Prefab_Slot, Transform_SlotRoot);
        if (gObj == null) return;

        var slotComponent = gObj.GetComponent<GameBookSlotUI>();
        if (slotComponent == null) return;

        slotComponent.InitSlot(dataId, curCategory, OnClickChildSlotSelected);
        _slotList.Add(dataId, slotComponent);

    }

    private void SetDetailInfoUI(string dataName, string dataDescription = "", string iconPath = "")
    {
        //Text_SellingPrice.text = currentSelectedData.SellingPrice;
        //Text_SellingPrice.gameObject.SetActivecurrentSelectedData.SellingPrice > 0);
        Text_MainName.text = dataName;
        Text_Description.text = dataDescription;


        if (string.IsNullOrEmpty(iconPath) == false)
        {
            DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, iconPath).Forget();

        }
        Image_MainIcon.gameObject.SetActive(string.IsNullOrEmpty(iconPath) == false);
    }


    private void OnClickChildSlotSelected(string slotDataId, EGameBookCategory selectedSlotCategory)
    {
 

        if (selectedSlotCategory == EGameBookCategory.ItemCategory)
        {
            var currentSelectedData = DaniTechGameDataManager.Instance.GetDNItemData(slotDataId);
            if (currentSelectedData == null) return;

            SetDetailInfoUI(currentSelectedData.Name, currentSelectedData.Description, currentSelectedData.IconPath);
        }
        else if (selectedSlotCategory == EGameBookCategory.MonsterCategory)
        {
            var currentSelectedData = DaniTechGameDataManager.Instance.GetDNMonsterData(slotDataId);
            if (currentSelectedData == null) return;

            SetDetailInfoUI(currentSelectedData.Name, currentSelectedData.Description, currentSelectedData.IconPath);

        }
        else if (selectedSlotCategory == EGameBookCategory.HarvestCategory) 
        {
            var currentSelectedData = DaniTechGameDataManager.Instance.GetDNFieldObjectData(slotDataId);
            if (currentSelectedData == null) return;

            SetDetailInfoUI(currentSelectedData.Name, currentSelectedData.Description, currentSelectedData.IconPath);

        }

        foreach (var slotKv in _slotList)
        {
            var slot = slotKv.Value;
            var dataId = slot.GetSlotDataId();
            slot.SetSelectedUI(slotDataId == dataId);
        }
    }
}
