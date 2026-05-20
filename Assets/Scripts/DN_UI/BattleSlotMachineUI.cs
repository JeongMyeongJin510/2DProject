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
    [SerializeField] private List<WeaponSlotUI> _slotList;

    private void OnEnable()
    {
        Button_ItemCreatSlot.BindOnClickButtonEvent(OnClick_ItemCreatSlot);
    }

    public void OnClick_ItemCreatSlot()
    {
        Debug.LogWarning("스핀~");
        //for (int i = 0; i < 9; i++) // AI 참고 추가
        //{
        //    // CreatItemSlot();
        //}

        StartSpin();
    }

    private void StartSpin()
    {
        if (_slotList.Count != 9)
        {
            Debug.LogError("슬롯이 9개가 아닙니다!");
            return;
        }

        foreach (var slot in _slotList)
        {
            int ramdomIndex = Random.Range(0, Sprite_RamdomIcons.Length);

            var ramdomSprite = Sprite_RamdomIcons[ramdomIndex];
            slot.SetWeaponIcon(ramdomSprite);
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
