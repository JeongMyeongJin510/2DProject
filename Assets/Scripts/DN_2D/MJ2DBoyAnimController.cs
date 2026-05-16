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
        _curState = newState;
        ResetAllAnimParameters();

        switch (_curState)
        {
            case AnimState.Idle:
                break;
            case AnimState.Walk:
                Animator_2DPlayer.SetBool("IsWalk", true);
                break;
            case AnimState.Jump:
                Animator_2DPlayer.SetBool("IsJump", true);
                break;
            case AnimState.Atk:
                Animator_2DPlayer.SetBool("IsAtk", true);
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
        Animator_2DPlayer.SetBool("IsJump", false);
        Animator_2DPlayer.SetBool("IsAtk", false);


    }
}

