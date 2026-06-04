using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
public class GameClearUI : DaniTechUIBase
{

    [Header("이미지")]
    [SerializeField] private RawImage RawImage_GameClearImg;

    [Header("버튼 등록 구간")]
    [SerializeField] private DaniTechUIButton Btn_GameQuit;

    private void OnEnable()
    {
        Btn_GameQuit.BindOnClickButtonEvent(OnClick_GameQuit);
        LoadAndSetGameClearImg();

        Btn_GameQuit.gameObject.SetActive(true);



    }

    private string GetRandomEndingDialogueId() //06.04 추가
    {
        int randomIndex = Random.Range(0, 2);
        return randomIndex == 0 ? "dialogue_ending_1_100" : "dialogue_ending_2_100";
    }



    private void LoadAndSetGameClearImg()
    {
        string texturePath = "Texture2D/Image_GameClear";

        DaniTechGameUtil.LoadAndSetTexture(RawImage_GameClearImg, texturePath).Forget();
    }


    public void OnClick_GameQuit()
    {
        Application.Quit();
        Debug.LogWarning("게임을 종료합니다.");
    }


}
