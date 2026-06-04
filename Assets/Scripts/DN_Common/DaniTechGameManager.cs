using System.Collections.Generic;
using UnityEngine;

public class DaniTechGameManager : MonoBehaviour
{
    public static DaniTechGameManager Inst { get; set; }

    //private MJ_2DPlayer _localPlayer;

    // 플레이 중에 저장되어야 하는 정보들이 있는 위치
    private DaniTechPlayerModel _playerModel = new DaniTechPlayerModel();

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        LoadSaveData();
    }

    public void SaveData()
    {
        DaniTechNetworkManager.Inst.RequstSaveData(_playerModel);
    }

    public void SaveAndEndGame()
    {
        SaveData();
        Application.Quit();
    }

    private void LoadSaveData()
    {
        _playerModel = DaniTechNetworkManager.Inst.RequstLoadSaveData();
    }

    public void IncreasePlayerExp(int exp)
    {
        // 추후에 한곳에서 관리할 수 있게 익스텐션으로 빼도 된다
        _playerModel.PlayerTotalExp += exp;
    }

    public void AddItem(string itemDataId, int addItemCount)
    {
        // 저장할때 고유값 ID를 부여하기 위해 사용
        long uniqueId = DaniTechGameUtil.GenerateUniqueId();

        // TODO : 우선 쉽게 사용할 수 있도록 중복 처리는 빼두었다. 습득할때마다 아이템이 하나씩 추가되도록 해두고
        // 추후에 중복값은 StackCount가 다 찰때까지 누적해줄 수 있도록 로직을 추가하자
        var newItem = new DaniTechItemModel();
        newItem.ItemUniqueId = uniqueId;
        newItem.ItemDataId = itemDataId;
        newItem.ItemStackCount = addItemCount;

        _playerModel.ItemList.Add(newItem);
        SaveData();
    }

    public bool RequestUseItem(long requestUseTargetItemUniqueId)
    {
        // 아이템의 실제적인 사용 부분이다.
        int removeTargetIdx = 0;
        bool isRemoveItemExist = false;
        foreach (var itemModel in _playerModel.ItemList)
        {
            if (itemModel.ItemUniqueId == requestUseTargetItemUniqueId)
            {
                isRemoveItemExist = true;

                // 데이터 분해
                string itemDataId = itemModel.ItemDataId;
                var itemData = DaniTechGameDataManager.Instance.GetDNItemData(itemDataId);
                if(string.IsNullOrEmpty(itemData.UseItemType) == false)
                {
                    //사용할 수 있는 아이템이므로
                    UseItemFunction(itemData.UseItemType, itemData.UseItemParameterList);
                }

                break;
            }

            removeTargetIdx++;
        }

        RequestRemoveItem(isRemoveItemExist, removeTargetIdx);

        return true;
    }

    private void UseItemFunction(string itemUseType, List<string> useItemParamList)
    {
        // 안전하게 체크
        if(useItemParamList == null || useItemParamList.Count == 0)
        {
            return;
        }

        if(itemUseType == "RandomItemBox")
        {

        }
        else if (itemUseType == "StatChangeAtk")
        {
            if(useItemParamList.Count > 0)
            {
                string str = useItemParamList[0];
                int statChangeVal = int.Parse(str);
                var playerComponent = GetLocalPlayer();
                playerComponent.AddAtk(statChangeVal);

            }
        }
        else if (itemUseType == "StatChangeHp")
        {
            if (useItemParamList.Count > 0)
            {
                string str = useItemParamList[0];
                int statChangeVal = int.Parse(str);
                var playerComponent = GetLocalPlayer();
                playerComponent.AddHp(statChangeVal);


            }
        }
        else if (itemUseType == "SummonMonster")
        {
            if (useItemParamList.Count > 0)
            {
                string str = useItemParamList[0];
                var strArr = str.Split(":");
                if (strArr.Length > 1)
                {
                    string monsterDataId = strArr[0];
                    int monsterSummonCount = int.Parse(strArr[1]);

                    for(int i = 0; i < monsterSummonCount; i++)
                    {
                        var playerComponent = GetLocalPlayer();
                        DaniTechGameObjectManager.Inst.CreateMonsterObject(monsterDataId, playerComponent.transform).Forget();
                    }

                }
            }
        }
    }

    private bool RequestRemoveItem(bool isRemoveItemExist, int removeTargetIdx)
    {
        if (isRemoveItemExist == true)
        {
            _playerModel.ItemList.RemoveAt(removeTargetIdx);
            SaveData();
            return true;
        }

        return false;
    }

    public List<DaniTechItemModel> GetPlayerItemList()
    {
        // _playerModel이 Private이므로 외부에서 ItemList를 받아올 수 있게 Get함수를 사용한다
        return _playerModel.ItemList;
    }

    public MJ_2DPlayer GetLocalPlayer()
    {
        return DaniTechGameObjectManager.Inst.GetLocalPlayer();
    }



    //(06.04 추가)  엔딩 다이얼로그 -> 게임 클리어 UI 나오도록 추가 
    public void ProcessGameClear() 
    {
       var player = GetLocalPlayer();
        if (player != null)
        {
            player.enabled = false;
        }

        // 게임클리어UI 열지않고 다이얼 로그 먼저 시작
        string endingDialogueId = Random.Range(0, 2) == 0 ? "dialogue_ending_1_100" : "dialogue_ending_2_100";
        DaniTechUIManager.Instance.OpenDialogueUI(endingDialogueId, DialogueContextType.Ending);
    }


    //(5.29 추가) 게임 종료 기능 추가 
    //public void ProcessGameClear()
    //{
    //    if (DaniTechUIManager.Instance != null)
    //    {
    //        DaniTechUIManager.Instance.OpenSimplePopup("★ GAME CLEAR ★\n축하합니다! 포탈을 통해 무사히 탈출에 성공하셨습니다.");
    //    }

    //    DaniTechUIManager.Instance.OpenGameClearUI();
    //    //Invoke("QuitGame", 3f);
    //}


    public void ProcessGameOver()
    {
        var player = GetLocalPlayer();
        if (player != null)
        {
            player.enabled = false;
        }
        DaniTechUIManager.Instance.OpenGameOverUI();
    }


    // 게임클리어 UI 추가로 인한 폐기
    //private void QuitGame()
    //{
    //    Application.Quit();
    //}
}

