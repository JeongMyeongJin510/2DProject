using System;
using UnityEngine;


public class SkillProjectile : SkillBase
{
    [SerializeField] private SpriteRenderer SpriteRenderer_Effect;
    [SerializeField] private float ProjectileSpeed = 5.0f;
    

    private int _damage;
    private int _ownerInstanceId;


    private Vector3 _moveDirection = new Vector3(1, 0, 0);


    private event Action<int, int> _onSkillCollision;

    private void OnDisable()
    {
        _onSkillCollision = null;
    }



    public void InitSkillObject(int ownerInstanceId, bool isDirRight, Vector3 playerPos, int damage, string parentTag, Action<int, int> onSkillCollision = null)
    {
        this.transform.position = playerPos;

        // 사이드뷰 기준 x값만 좌우가 수정
        _moveDirection = isDirRight ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
        SpriteRenderer_Effect.flipX = !isDirRight;
        SpriteRenderer_Effect.flipY = !isDirRight;

        _damage = damage;
        _ownerInstanceId = ownerInstanceId;

        //콜백이라 그냥 1대1로 구독 +=이여도 상관은 없다.
        _onSkillCollision = onSkillCollision;

        this.gameObject.tag = parentTag;
    }

    private void Update()
    {
        transform.position += _moveDirection * ProjectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollision(collision.collider);
    }

    private void CheckCollision(Collider2D collision)
    {
        bool isOwnerPlayer = (_ownerInstanceId == 0);

        if (collision.CompareTag("Player") && (isOwnerPlayer == false))
        {
            // 1번 방식 = 플레이어한테 직접 투사체가 데미지를 줬음=
            //var player = DaniTechGameObjectManager.Inst.GetLocalPlayer();
            //player.TakeDamage(_damage);

            // 2번 방식 - 투사체가 직접 데미지를 주는게 아니라 부모에게 충돌체의 ID를 이름
            _onSkillCollision?.Invoke(0, _damage); //0? -> LocalPlayer는 0번이니까 그냥 하드코딩


            Destroy(this.gameObject);
        }
        else if (collision.CompareTag("Enemy") && (isOwnerPlayer))
        {
            var gObj = collision.gameObject;
            if (gObj == null) return;

            var monsterComponent = gObj.GetComponent<GameMonster>();
            if (monsterComponent == null) return;

            //1번 방식 - 투사체가 직접 몬스터에게 데미지를 입힌다 : 다만, 게임오브젝트매니저를 통하는게 조금 더 최종폼
            // monsterComponent.TakeDamage(skillDamage);

            int instId = monsterComponent.GetMonsterInstanceId();
            _onSkillCollision?.Invoke(instId, _damage);

            Destroy(this.gameObject);

        }
    }


}
