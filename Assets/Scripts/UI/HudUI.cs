using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class HudUI : DaniTechUIBase
{
    [SerializeField] private GameObject Prefab_Hub_Slot;
    [SerializeField] private Transform Transform_Slot_Root;


   
    private Dictionary<int, HudSlotUI> _hubSlotList = new Dictionary<int, HudSlotUI>();

    public void AddHudSlot(int instanceId, Transform targetTransform)
    {
        CreateHubSlot(instanceId, targetTransform);
    }

    public void CreateHubSlot(int instanceId, Transform targetTransform)
    {
        var gObj = Instantiate(Prefab_Hub_Slot, Transform_Slot_Root);
        if (gObj == null) return;

        var slotComponent = gObj.GetComponent<HudSlotUI>();
        if (slotComponent == null) return;

        slotComponent.InitSlot(instanceId, targetTransform);

        _hubSlotList.Add(instanceId, slotComponent);
    }

    public void RemoveHubSlost(int instanceId)
    {
        // 생성이 된게 맞다면
        if (_hubSlotList.ContainsKey(instanceId) == true)
        {
            var slot = _hubSlotList[instanceId];
            //Destroy는 컴포넌트인 slot이 아니라 slot.gameObject

            Destroy(slot.gameObject);

            _hubSlotList.Remove(instanceId);
        }
    }
}
