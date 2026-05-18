using UnityEngine;

public class MiniPJ_MainUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_GameClose;
    [SerializeField] private DaniTechUIButton Btn_GameBack;
    [SerializeField] private DaniTechUIButton Btn_GameOption;

    private void OnEnable()
    {
        Btn_GameClose.BindOnClickButtonEvent(OnClick_GameClose);
        Btn_GameBack.BindOnClickButtonEvent(OnClick_GameBack);
        Btn_GameOption.BindOnClickButtonEvent(OnClick_GameOption);
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
