using UnityEngine;

public class MainUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_MyProfile;
    [SerializeField] private DaniTechUIButton Btn_StartBattle;
    [SerializeField] private DaniTechUIButton Btn_MonsterSpawn;
    [SerializeField] private DaniTechUIButton Btn_OpenInventory;
    [SerializeField] private DaniTechUIButton Btn_GameOption;
    [SerializeField] private DaniTechUIButton Btn_GameClose;

    private void OnEnable()
    {
        Btn_MyProfile.BindOnClickButtonEvent(OnClick_OpenMyProfile);
        Btn_StartBattle.BindOnClickButtonEvent(OnClick_StartBattle);
        Btn_MonsterSpawn.BindOnClickButtonEvent(OnClicK_MonsterSpawn);
        Btn_OpenInventory.BindOnClickButtonEvent(OnClick_OpenInventory);
        Btn_GameOption.BindOnClickButtonEvent(OnClick_GameOption);
        Btn_GameClose.BindOnClickButtonEvent(OnClick_GameClose);
    }

    public void OnClick_OpenInventory()
    {
        DaniTechUIManager.Instance.OpenInventoryPopup();
        DaniTechGameManager.Inst.SaveData();
    }

    public void OnClick_OpenMyProfile()
    {
        DaniTechUIManager.Instance.OpenMyProfilePopup("character_hellena_01");
        Debug.LogWarning("프로필 오픈");
    }

    public void OnClick_StartBattle()
    {
        DaniTechUIManager.Instance.OpenSimplePopup("배틀 스타트!");
        Debug.LogWarning("배틀 스타트");
    }

    public void OnClicK_MonsterSpawn()
    {
        Debug.LogWarning("몬스터 스폰");
    }

    public void OnClick_GameOption() 
    {
        DaniTechUIManager.Instance.OpenGameOption();
        Debug.LogWarning("게임옵션 오픈");

    }

    public void OnClick_GameClose()
    {

        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit(); // 빌드한 경우는 정상작동
        Debug.LogWarning("게임종료!!");
    }



}
