using UnityEngine;

public class MJ_2DNpc : MonoBehaviour
{
    [Header("애니메이터")]
    [SerializeField] private DaniTech_2DAnimatorController AnimatorController_Entity;


    public void ChangeNpcState(DaniTech_EntityAnimState newState)
    {
        // 이런 곳에 UI나 플레이어의 별도 처리를 넣어줄 수도 있다


        // 우선 애니메이션만 바꿔 봅시다
        AnimatorController_Entity.SetState(newState);
    }


}
