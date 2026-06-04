using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DaniTech_DialogueUI : DaniTechUIBase
{
    [SerializeField] private GameObject Layout_CharacterName;
    [SerializeField] private Text Text_Character;
    [SerializeField] private Text Text_Description;
    [SerializeField] private DaniTechUIButton Button_Next;

    [SerializeField] private RawImage RawImage_IntroImg; // 06.02 추가

    // 대사의 목적 상태를 보관할 멤버변수
    private DialogueContextType _curContextType = DialogueContextType.None;

    private string _currentDialogueId;
    private Queue<string> _descriptionQueue = new Queue<string>();

    private void OnEnable()
    {
        Button_Next.BindOnClickButtonEvent(OnClick_Next);
    }

    // 다이얼로그에서 Next 버튼이 눌러질때 호출된다
    public void OnClick_Next()
    {
        // 다음 대사가 있는지 체크한다
        bool isNextDescriptionExist = CheckAndSetDescription();

        if (isNextDescriptionExist)
        {
            return;
        }

        // 대사가 없다면, 다음으로 이어지는 다이얼로그가 있는지 체크한다
        bool isNextDialogueExist = CheckAndStartNextDialogue();
        if(isNextDialogueExist == false)
        {
            // 모든 연쇄 대사가 완전히 종료되었다면 정산
            ProcessDialogue();
        }

 
    }

    //06.02 추가 대사 종료 후 다음 UI 서순 제어 목적
    private void ProcessDialogue()
    {
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.DNDialogueUI);


        //대사 목적에 따라 서순 분기
        switch (_curContextType)
        {
            case DialogueContextType.Intro:
                DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.LobbyUI);
                break;
            case DialogueContextType.InGameSpot:
                Debug.LogWarning("인게임 스팟 출력");
                break;
            case DialogueContextType.Ending:  //06.04 추가
                DaniTechUIManager.Instance.OpenGameClearUI();
                break;
        }

        _curContextType = DialogueContextType.None;
    }

    private bool CheckAndStartNextDialogue()
    {
        var dialogueData = DaniTechGameDataManager.Instance.GetDNDialogueData(_currentDialogueId);
        if (dialogueData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 {dialogueData}");
            return false;
        }

        // 현재 데이터를 기준으로 다음 다이얼로그가 있는지 체크해보고, 있다면 다음 다이얼로그를 시작한다!
        string nextDialogueId = dialogueData.NextDialogueId;
        if (string.IsNullOrEmpty(nextDialogueId) == false)
        {
            StartDialogue(nextDialogueId);
            return true;
        }

        return false;
    }

    // 다이얼로그를 시작하는 메서드 (외부에서 UIManager를 통해 다이얼로그 시작을 요청할때도 쓴다!)
    public void StartDialogue(string dialogeId, DialogueContextType contextType = DialogueContextType.None)
    {
        //contextType이 None이 아닐때만 덮어씀 (연쇄 대상에서 초기화 방지)
        if (contextType != DialogueContextType.None)
        {
            _curContextType = contextType;
        }

        var dialogueData = DaniTechGameDataManager.Instance.GetDNDialogueData(dialogeId);
        if (dialogueData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 {dialogueData}");
            return;
        }

        // 현재 진행중인 다이얼로그 Id는 다음 다이얼로그가 있는지 체크할 때 쓸 수 있도록 보관한다
        _currentDialogueId = dialogeId;

        // 혹시 현재 대사가 너무 길거나 다음 페이지 처리가 필요할 때 <np> 키워드로 잘라주자!
        if (dialogueData.Description.Contains("<np>"))
        {
            string[] dialogueDescriptionList = dialogueData.Description.Split("<np>");
            foreach(string desc in dialogueDescriptionList)
            {
                _descriptionQueue.Enqueue(desc);
            }
            CheckAndSetDescription();
        }
        else
        {
            // Np 태그가 없다면 바로 다이얼로그 UI를 세팅하자
            SetCurrentDialogueDescription(dialogueData.Description);
        }

        SetCharacterName(dialogueData.CharacterDataId, dialogueData.SpeakerName); //06.04 추가
        SetIntroImage(dialogueData.TexturePath);
    }

    private bool CheckAndSetDescription()
    {
        bool isNextDescriptionExsist = (_descriptionQueue.Count > 0);
        if (isNextDescriptionExsist)
        {
            string desc = _descriptionQueue.Dequeue();
            SetCurrentDialogueDescription(desc);
        }

        return isNextDescriptionExsist;
    }

    private void SetCharacterName(string characterDataId, string speakerName)  //06.04 추가
    {
        if (string.IsNullOrEmpty(characterDataId) == false)
        {
            var characterData = DaniTechGameDataManager.Instance.GetCharacterData(characterDataId);
            if (characterData != null)
            {
                Layout_CharacterName.SetActive(true);
                Text_Character.text = characterData.Name;
                return;
            }
            
        }

        // CharacterDataId가 없으며ㅕ녀 SpeakerName 직접 사용 //06.04 추가

        bool isActive = string.IsNullOrEmpty(speakerName) == false;
        Layout_CharacterName.SetActive(isActive);
        if (isActive)
        {
            Text_Character.text = speakerName;
        }


        // 캐릭터 정보가 있다면 말하는 이의 추가 정보를 표기해줄 수 있도록 연동하는 부분
        // 기존 코드 06.04 스피커네임을 추가하면서 주석처리
        //bool isActive = (string.IsNullOrEmpty(characterDataId) == false);
        //Layout_CharacterName.SetActive(isActive);

        //if (isActive)
        //{
        //    var characterData = DaniTechGameDataManager.Instance.GetCharacterData(characterDataId);
        //    if(characterData != null)
        //    {
        //        Text_Character.text = characterData.Name;
        //    }
        //}
    }

    private void SetIntroImage(string texturePath)
    {
        if (string.IsNullOrEmpty(texturePath) == true)
        {
            RawImage_IntroImg.gameObject.SetActive(false);
            return;
        }
        
        RawImage_IntroImg.gameObject.SetActive(true);
        DaniTechGameUtil.LoadAndSetTexture(RawImage_IntroImg, texturePath).Forget();
    }


    private void SetCurrentDialogueDescription(string description)
    {
        Text_Description.text = description;
    }
}
