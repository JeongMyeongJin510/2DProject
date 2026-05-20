using UnityEngine;

public class MiniPJ_MainUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_GameClose;
    [SerializeField] private DaniTechUIButton Btn_GameBack;
    [SerializeField] private DaniTechUIButton Btn_GameOption;
    [SerializeField] private DaniTechUIButton Btn_GameBook;

    [Header("스킬 버튼")]
    [SerializeField] private DaniTechUIButton Btn_NormalAttack;
    [SerializeField] private DaniTechUIButton Btn_FirstSkill; //Btn_CircleAttack;
    [SerializeField] private DaniTechUIButton Btn_SecondSkill; //Btn_RayAttack;
    [SerializeField] private DaniTechUIButton Btn_ThridSkill; //Btn_ProjectileAttack;


    private void OnEnable()
    {
        Btn_GameClose.BindOnClickButtonEvent(OnClick_GameClose);
        Btn_GameBack.BindOnClickButtonEvent(OnClick_GameBack);
        Btn_GameOption.BindOnClickButtonEvent(OnClick_GameOption);
        Btn_GameBook.BindOnClickButtonEvent(OnClick_OpenGameBook);

        Btn_NormalAttack.BindOnClickButtonEvent(OnClick_UseNormalAttack);
        Btn_FirstSkill.BindOnClickButtonEvent(OnClick_UseFirstSkill);
        Btn_SecondSkill.BindOnClickButtonEvent(OnClick_UseSecondSkill);
        Btn_ThridSkill.BindOnClickButtonEvent(OnClick_UseThirdSkill);
    }

    public void OnClick_UseNormalAttack()
    {
        DaniTechGameManager.Inst.LocalPlayer.UseNormalAttack();
        Debug.LogWarning("평타");
    }

    public void OnClick_UseFirstSkill()
    {
        DaniTechGameManager.Inst.LocalPlayer.UseFirstSkill();
    }

    public void OnClick_UseSecondSkill()
    {
        DaniTechGameManager.Inst.LocalPlayer.UseSecondSkill();
    }

    public void OnClick_UseThirdSkill()
    {
        DaniTechGameManager.Inst.LocalPlayer.UseThirdSkill();
    }

    public void OnClick_OpenGameBook()
    {
        DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.GameBookUI);
    }
    public void OnClick_GameClose()
    {
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
