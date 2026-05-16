using UnityEngine;

public class GameOptionPopup : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Button_CloseSelfAllArea;

    private void OnEnable()
    {
        Button_CloseSelfAllArea.BindOnClickButtonEvent(OnClick_ClosePopup);
    }
    public void OnClick_ClosePopup()
    {
        DaniTechUIManager.Instance.ClosePopupUI(DaniTechUIType.Popup_GameOption);
    }
}
