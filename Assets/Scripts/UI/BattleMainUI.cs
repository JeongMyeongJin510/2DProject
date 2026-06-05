using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMainUI : DaniTechUIBase
{
    [Header("시스템 메뉴 버튼")]
    [SerializeField] private DaniTechUIButton Btn_GameClose;
    [SerializeField] private DaniTechUIButton Btn_GameBack;
    [SerializeField] private DaniTechUIButton Btn_GameOption;
    [SerializeField] private DaniTechUIButton Btn_GameBook;

    [Header("하단 정보창 UI 리소스")]
    [SerializeField] private Slider Slider_PlayerHpBar;
    [SerializeField] private Slider Slider_MonsterHpBar;
    [SerializeField] private Text Text_PlayerHpDisplay;
    [SerializeField] private Text Text_PlayerAtkDisplay; // 하단 정보창 공격력 텍스트
    [SerializeField] private Text Text_PlayerDefDisplay; // 하단 정보창 방어력 텍스트
    [SerializeField] private Text Text_MonsterHpDisplay;

    [Header("우측 몬스터 이미지 리소스")]
    [SerializeField] private Image Image_MonsterImage;


    [Header("실시간 런타임 변동 데이터")]
    private int _targetMonsterInstanceId;
    private string _targetMonsterName;

    // 몬스터 전투 스텟
    private int _currentMonsterHp;
    private int _maxMonsterHp;
    private int _monsterBaseAtk; // 몬스터 기본 공 (반격 사용 예정)

    // 플레이어 전투 스텟
    private int _currentPlayerHp;
    private int _maxPlayerHp;
    private int _playerCurrentAtk;
    private int _playerCurrentDef;

    // 전투 종료 중복 처리 방지
    private bool _isBattleOver;

    // 한번만 찾아서 저장해두는 캐싱(매번 GetComponentInChildren 탐색방지)
    private BattleSlotMachineUI _slotMachineUI;

    private void OnEnable()
    {
        Btn_GameClose.BindOnClickButtonEvent(OnClick_GameClose);
        Btn_GameBack.BindOnClickButtonEvent(OnClick_GameBack);
        Btn_GameOption.BindOnClickButtonEvent(OnClick_GameOption);
        Btn_GameBook.BindOnClickButtonEvent(OnClick_OpenGameBook);
    }


    //전투가 시작될 때 오브젝트 매니저로부터 진짜 플레이어 본체 컴포넌트를 넘겨받아 실시간으로 기준 스탯을 주입(동기화)
    public void InitBattleWindow(int monsterInstanceId, string monsterDataId)
    {
        _targetMonsterInstanceId = monsterInstanceId;
        _isBattleOver = false;

        // 자식에서 SlotMachineUI 한번만 찾아서 캐싱
        _slotMachineUI = GetComponentInChildren<BattleSlotMachineUI>();

        // 부딪힌 몬스터 컴포넌트 실체를 가져온다
        var monsterComponent = DaniTechGameObjectManager.Inst.GetMonsterObjectByInstanceId(monsterInstanceId);
        if (monsterComponent != null)
        {
            _currentMonsterHp = monsterComponent._baseHp;
            _maxMonsterHp = monsterComponent._baseHp;
            _monsterBaseAtk = monsterComponent._baseAtk;

            //_targetMonsterName = monsterComponent.gameObject.name; // 승리 시 팝업에 몬스터Id(클론)이 나옴

            //06.05 승리 시 팝업에 몬스터 이름이 나오도록 수정
            var monsterData = DaniTechGameDataManager.Instance.GetDNMonsterData(monsterDataId);
            if (monsterData != null)
            {
                _targetMonsterName = monsterData.Name;
            }


            var monsterSpriteRenderer = monsterComponent.GetComponentInChildren<SpriteRenderer>();

            if (monsterSpriteRenderer != null && Image_MonsterImage != null)
            {
                //필드에서 돌아다니던 몬스터의 진짜 외형 이미지를 UI 이미지 슬롯에 복사
                Image_MonsterImage.sprite = monsterSpriteRenderer.sprite;
            }
        }

        var playerComponent = DaniTechGameObjectManager.Inst.GetLocalPlayer();
        if (playerComponent != null)
        {
            _maxPlayerHp = playerComponent.GetPlayerHp();
            _currentPlayerHp = playerComponent.GetPlayerHp();
            _playerCurrentAtk = playerComponent.GetPlayerBaseAtk();
            _playerCurrentDef = playerComponent.GetPlayerBaseDef();
        }
        // 초기 UI 갱신
        RefreshBattleStatsWindow();
    }

    //스핀 정산 시작 직전, 이전 턴의 증가 수치를 지우고 플레이어 본체의 원래 기본 체급 수치로 덮어씌워 리셋
    private void ResetTurnStats() 
    {
        var playerComponent = DaniTechGameObjectManager.Inst.GetLocalPlayer();
        if (playerComponent != null)
        {
            // 일반 Get 함수를 통해 기본 수치로 완전 초기화
            _playerCurrentAtk = playerComponent.GetPlayerBaseAtk();
            _playerCurrentDef = playerComponent.GetPlayerBaseDef();
        }
    }

    // 슬롯머신 스핀이 완료되었을 때 결과 번호 가방(9개)을 전송받아 연산
    public void ReceiveSlotMachineResult(List<int> resultIndices)
    {

        if (_isBattleOver) return;
        if (resultIndices == null || resultIndices.Count < 9) return;

        ResetTurnStats();

        int pureAddAtk = CalPureAddAttack(resultIndices);
        int pureAddDef = CalPureAddDefense(resultIndices);

        _playerCurrentAtk += pureAddAtk;
        _playerCurrentDef += pureAddDef;

        CheckLineMatchBingoBouns(resultIndices);
        ExecuteAutoAttackToMonster();

        if (_currentMonsterHp > 0)
        {
            ExecuteMonsterCounterAttackToPlayer();
        }

        RefreshBattleStatsWindow();

        //전투 종료
        if (_currentMonsterHp <= 0)
        {
            ProcessBattleVictory();
        }
        else if (_currentPlayerHp <= 0)
        {
            ProcessBattleDefeat();
        }
    }
    
    private int CalPureAddAttack(List<int> resultIndices)
    {
        int totalAtk = 0;
        foreach (int index in resultIndices)
        {
            if (index >= 0 && index <= 4) totalAtk += 5;
        }
        return totalAtk;
    }

    private int CalPureAddDefense(List<int> resultIndices)
    {
        int totalDef = 0;
        foreach (int index in resultIndices)
        {
            if (index >= 5 && index <= 8) totalDef += 2;
        }
        return totalDef;
    }

    private void CheckLineMatchBingoBouns(List<int> resultIndices)
    {
        int bingoCount = 0;

        List<int> bingoLineIndices = new List<int>();  // 06.02 빙고 표시를 위해 리스트 추가 

        int[,] linesToMatch = new int[8, 3]
        {
            { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 },   //가로 3줄(인덱스 0,1,2)
            { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 },   //세로 3줄 (인덱스 3,4,5)
            { 0, 4, 8 }, { 2, 4, 6 }                 // 대각 2줄 (인덱스 6,7)
        };

        for (int i = 0; i < 8; i++)
        {
            int idxA = linesToMatch[i, 0];
            int idxB = linesToMatch[i, 1];
            int idxC = linesToMatch[i, 2];

            if (resultIndices[idxA] == resultIndices[idxB] && resultIndices[idxB] == resultIndices[idxC])
            {
                bingoCount++;
                bingoLineIndices.Add(i);
            }
        }

        if (bingoCount > 0)
        {
            float multiplier = (bingoCount == 1) ? 1.5f : 2.0f;
            _playerCurrentAtk = (int)(_playerCurrentAtk * multiplier);
            _playerCurrentDef = (int)(_playerCurrentDef * multiplier);

            Debug.LogWarning($"{bingoCount}줄 빙고 성공! 공격력 {_playerCurrentAtk} / 방어력 {_playerCurrentDef} ({multiplier}배 적용)");
            DaniTechUIManager.Instance.OpenSimplePopup($"{bingoCount}줄 빙고 성공! 공격력 {_playerCurrentAtk} / 방어력 {_playerCurrentDef} ({multiplier}배 적용)");

            if (_slotMachineUI != null)
            {
                _slotMachineUI.ShowBingoLine(bingoLineIndices);
            }

        }
        else
        {
            Debug.LogWarning($"이번턴에는 빙고가 완성되지 않았습니다. 공격력 {_playerCurrentAtk} / 방어력 {_playerCurrentDef} (기본배율 1배)");
            DaniTechUIManager.Instance.OpenSimplePopup($"이번턴에는 빙고가 완성되지 않았습니다. 공격력 {_playerCurrentAtk} / 방어력 {_playerCurrentDef} (기본배율 1배)");

        }
    }

    private void ExecuteAutoAttackToMonster()
    {
        //아무리 쎄게 맞아도 체력이 0밑으로 내려가지 않게 함
        _currentMonsterHp = Mathf.Max(0, _currentMonsterHp - _playerCurrentAtk);
    }

    // 몬스터 반격 기능 (살아있을 경우만)
    private void ExecuteMonsterCounterAttackToPlayer()
    {
        int reducedDamage = Mathf.Max(1, _monsterBaseAtk - _playerCurrentDef);
        _currentPlayerHp = Mathf.Max(0, _currentPlayerHp - reducedDamage);
    } 

    private void RefreshBattleStatsWindow()
    {
        //플레이어 공격력 업데이트
        if (Text_PlayerAtkDisplay != null)
        {
            Text_PlayerAtkDisplay.text = $"공격력 : {_playerCurrentAtk}";
        }

        //플레이어 방어력 업데이트
        if (Text_PlayerDefDisplay != null)
        {
            Text_PlayerDefDisplay.text = $"방어력 : {_playerCurrentDef}";
        }

        // 플레이어 체력 업데이트
        if (Text_PlayerHpDisplay != null)
        {
            Text_PlayerHpDisplay.text = $"{_currentPlayerHp} / {_maxPlayerHp}";
        }

        //플레이어 HP 슬라이더 업데이트 (0으로 나누기 방지용 조건 추가)
        if (Slider_PlayerHpBar != null && _maxPlayerHp > 0)
        {
            Slider_PlayerHpBar.value = (float)_currentPlayerHp / _maxPlayerHp;
        }

        //몬스터 HP 슬라이더 업데이트
        if (Slider_MonsterHpBar != null && _maxMonsterHp > 0)
        {
            Slider_MonsterHpBar.value = (float)_currentMonsterHp / _maxMonsterHp;
        }

        //몬스터 HP 텍스트 업데이트
        if (Text_MonsterHpDisplay != null)
        {
            Text_MonsterHpDisplay.text = $"{_currentMonsterHp} / {_maxMonsterHp}";
        }
    }

    private void ProcessBattleVictory()
    {
        _isBattleOver = true;

        if (DaniTechGameObjectManager.Inst != null)
        {
            DaniTechGameObjectManager.Inst.RequestDestroyMonsterObject(_targetMonsterInstanceId);
            DaniTechGameObjectManager.Inst.FinishBattleState();

            DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.BattleMainUI); //5.28
        }
        //this.gameObject.SetActive(false); 비활성화만 시키니깐 다음 전투에서 베틀메인UI가 뜨지 않는 버그 발생

        DaniTechUIManager.Instance.OpenSimplePopup($"{_targetMonsterName}와의 전투에서 승리하였습니다!");

    }

    private void ProcessBattleDefeat()
    {
        _isBattleOver = true;

        //플레이어가 죽었으니 전투 창 닫기 // TODO 플레이어 사망 시 게임종료 추가해야함
        Debug.LogWarning("패배");
        //this.gameObject.SetActive(false);
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.BattleMainUI); //5.28


        //게임ㅇ오버UI 
        DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.GameOverUI);

    }







    public void OnClick_OpenGameBook()
    {
        DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.GameBookUI);
    }
    public void OnClick_GameClose()
    {
        Application.Quit();
        Debug.LogWarning("눌러짐");
    }

    public void OnClick_GameBack()
    {
        DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.LobbyUI);
        Debug.LogWarning("로비로 돌아가기");
    }

    public void OnClick_GameOption()
    {
        DaniTechUIManager.Instance.OpenGameOption();
        Debug.LogWarning("게임 옵션 눌러짐");
    }
}
