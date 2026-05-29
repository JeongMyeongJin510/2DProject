using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
public class GameClearUI : DaniTechUIBase
{

    [Header("이미지")]
    [SerializeField] private RawImage RawImage_GameOverImg;

    [Header("버튼 등록 구간")]
    [SerializeField] private DaniTechUIButton Btn_GameQuit;

    private void OnEnable()
    {
        Btn_GameQuit.BindOnClickButtonEvent(OnClick_GameQuit);
        LoadAndSetGameOverImg();

    }

    private void LoadAndSetGameOverImg()
    {
        string texturePath = "Texture2D/Image_GameClear";

        DaniTechGameUtil.LoadAndSetTexture(RawImage_GameOverImg, texturePath).Forget();
    }


    public void OnClick_GameQuit()
    {
        Application.Quit();
        Debug.LogWarning("게임을 종료합니다.");
    }


}
