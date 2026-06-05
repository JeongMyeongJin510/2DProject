using Cysharp.Threading.Tasks.Triggers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSlotMachineUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Button_ItemCreatSlot;
    [SerializeField] private Sprite[] Sprite_RamdomIcons; // AI 참고 추가

    // 사실 슬롯 게임은 동적생성은 필요 없다 => 이유는 슬롯 9개가 고정이기 때문에!
    // 미리 프리팹에서 지정해두고 그걸 돌려써도 된다
    [Header("슬롯 리스트")]
    [SerializeField] private List<WeaponSlotUI> _slotList;

    [Header("빙고 라인")]
    [SerializeField] private GameObject BingoLine_Row1;
    [SerializeField] private GameObject BingoLine_Row2;
    [SerializeField] private GameObject BingoLine_Row3;
    [SerializeField] private GameObject BingoLine_Col1;
    [SerializeField] private GameObject BingoLine_Col2;
    [SerializeField] private GameObject BingoLine_Col3;
    [SerializeField] private GameObject BingoLine_Diag1;
    [SerializeField] private GameObject BingoLine_Diag2;


    // 빙고 라인 인덱스 규칙 6.02 추가
    // 0 = 가로 {0,1,2} / 1 = 가로 {3,4,5} / 2 = 가로 {6,7,8}
    // 3 = 세로 {0,3,6} / 4 = 세로 {1,4,7} / 5 = 세로 {2,5,8}
    // 6 = 대각선 {0,4,8} / 7 = 대각선 {2,4,6}
    private Dictionary<int, GameObject> _bingoLineDict = new Dictionary<int, GameObject>();

    private bool _isSpinning = false;



    private void Awake()
    {
        _bingoLineDict.Add(0, BingoLine_Row1);
        _bingoLineDict.Add(1, BingoLine_Row2);
        _bingoLineDict.Add(2, BingoLine_Row3);
        _bingoLineDict.Add(3, BingoLine_Col1);
        _bingoLineDict.Add(4, BingoLine_Col2);
        _bingoLineDict.Add(5, BingoLine_Col3);
        _bingoLineDict.Add(6, BingoLine_Diag1);
        _bingoLineDict.Add(7, BingoLine_Diag2);

        HideAllBingoLine();
    }



    private void OnEnable()
    {
        Button_ItemCreatSlot.BindOnClickButtonEvent(OnClick_ItemCreatSlot);
    }

    public void OnClick_ItemCreatSlot()
    {
        // 스핀 중이면 무시
        if (_isSpinning == true)
        {
            return;
        }

        _isSpinning = true;
        Debug.LogWarning("스핀~");
        StartSpin();



        Invoke("ResetSpinCooldown", 2f);


        //for (int i = 0; i < 9; i++) // AI 참고 추가
        //{
        //    // CreatItemSlot();
        //}

    }

    private void ResetSpinCooldown()
    {
        _isSpinning = false;
    }

    private void StartSpin()
    {
        if (_slotList.Count != 9)
        {
            Debug.LogError("슬롯이 9개가 아닙니다!");
            return;
        }

        // 스핀 시작 시 빙고 라인 초기화
        HideAllBingoLine();

        List<int> turnResultIndices = new List<int>();

        foreach (var slot in _slotList)
        {
            int ramdomIndex = Random.Range(0, Sprite_RamdomIcons.Length);

            var ramdomSprite = Sprite_RamdomIcons[ramdomIndex];
            slot.SetWeaponIcon(ramdomSprite);

            turnResultIndices.Add(ramdomIndex);
        }

        SendResultToBattleMainUI(turnResultIndices);
    }

    private void SendResultToBattleMainUI(List<int> resultIndices)
    {

        var battleMainUI = GetComponentInParent<BattleMainUI>();

        if (battleMainUI != null)
        { 
            battleMainUI.ReceiveSlotMachineResult(resultIndices);
        }
        else
        {
            Debug.LogWarning("[슬롯머신 UI] 부모 오브젝트에서 'BattleMainUI' 스크립트를 찾을 수 없습니다.");
        }
    }

    public void ShowBingoLine(List<int> bingoLineIndices)
    {
        HideAllBingoLine();

        foreach (int index in bingoLineIndices)
        {
            if(_bingoLineDict.ContainsKey(index) == false)
            {
                continue;
            }

            _bingoLineDict[index].SetActive(true);
        }

        Invoke("HideAllBingoLine", 2f);
    }

    private void HideAllBingoLine()
    {
        foreach (var lineKv in _bingoLineDict)
        {
            lineKv.Value.SetActive(false);
        }
    }


    //private void CreatItemSlot()
    //{
    //    var gObj = Instantiate(Prefab_BattleSlot, Transform_ItemSlotUIRoot);
    //    if (gObj == null) return;

    //    var slotImage = gObj.GetComponentInChildren<Image>(); // AI 참고 추가

    //    if (slotImage != null && Sprite_RamdomIcons.Length > 0) // AI 참고 추가 + 좀 더 확인 후 수정예정
    //    {
    //        int ramdomIndex = Random.Range(0, Sprite_RamdomIcons.Length);

    //        slotImage.sprite = Sprite_RamdomIcons[ramdomIndex];
    //    }
    //}
}
