using UnityEngine;


public enum AnimState
{
    None = 0,
    Idle,
    Walk,
    Jump,
    Atk
}

public class MJ2DBoyAnimController : MonoBehaviour
{
    [SerializeField] private Animator Animator_2DPlayer;

    private AnimState _curState;

    public void SetAnimState(AnimState newState)
    {
        if (newState == AnimState.Idle && _curState == AnimState.Idle)
        {
            return;
        }


        _curState = newState;

        switch (_curState)
        {
            case AnimState.Idle:
                ResetAllAnimParameters();
                break;
            case AnimState.Walk:
                Animator_2DPlayer.SetBool("IsWalk", true);
                break;
            case AnimState.Jump:
                Animator_2DPlayer.SetTrigger("IsJump");
                break;
            case AnimState.Atk:
                Animator_2DPlayer.SetTrigger("IsAtk");
                break;

            default:
                ResetAllAnimParameters();
                Debug.LogWarning("올바르지 않은, 작동되지 않은 상태입니다.");
                break;
        }
    }

    private void ResetAllAnimParameters()
    {
        Animator_2DPlayer.SetBool("IsWalk", false);
        

    }
}

