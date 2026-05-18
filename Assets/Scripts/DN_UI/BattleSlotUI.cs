using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class BattleSlotUI : DaniTechUIBase
{
    [SerializeField] private GameObject Prefab_BattleSlot;
    [SerializeField] private Transform Transform_ItemSlotUIRoot;
    [SerializeField] private DaniTechUIButton Button_ItemCreatSlot;
    [SerializeField] private Sprite[] Sprite_RamdomIcons; // AI 참고 추가

    private void Awake()
    {
        DestroySlotOnAwake();
    }

    private void DestroySlotOnAwake()
    {
        var destroyTargetComponentList = Transform_ItemSlotUIRoot.GetComponentsInChildren<BattleSlotUI>();
        foreach (var childTransform in destroyTargetComponentList)
        {
            if (childTransform.gameObject != null)
            {
                Destroy(childTransform.gameObject);
            }
        }
    }


    private void OnEnable()
    {
        Button_ItemCreatSlot.BindOnClickButtonEvent(OnClick_ItemCreatSlot);
    }

  

    public void OnClick_ItemCreatSlot()
    {
        Debug.LogWarning("스핀~");
        DestroySlotOnAwake();
        for (int i = 0; i < 9; i++) // AI 참고 추가
        {
            CreatItemSlot();
        }
    }

    private void CreatItemSlot()
    {
        var gObj = Instantiate(Prefab_BattleSlot, Transform_ItemSlotUIRoot);
        if (gObj == null) return;

        var slotImage = gObj.GetComponentInChildren<Image>(); // AI 참고 추가

        if (slotImage != null && Sprite_RamdomIcons.Length > 0) // AI 참고 추가 + 좀 더 확인 후 수정예정
        {
            int ramdomIndex = Random.Range(0, Sprite_RamdomIcons.Length);

            slotImage.sprite = Sprite_RamdomIcons[ramdomIndex];
        }
    }
}
