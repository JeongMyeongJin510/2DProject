using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSlotUI : MonoBehaviour
{
    [SerializeField] private Text Text_InteractionTitle;
    [SerializeField] private Text Text_KeyName;



    // 인터렉션 UI의 특징은 - 1개
    private int _instanceId;

    // 참조형을 기록(캐싱)
    private Transform _targetTransform;



    public void InitSlot(int instanceId, string interactionKey, string interactionTitle, Transform targetTransform)
    {
        _instanceId = instanceId;
        _targetTransform = targetTransform;

        Text_KeyName.text = interactionKey;
        Text_InteractionTitle.text = interactionTitle;
    }
}
