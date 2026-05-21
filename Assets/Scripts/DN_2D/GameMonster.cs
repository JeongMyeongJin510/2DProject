using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameMonster : MonsterBase
{
    [Header("몬스터 프리팹에서 미리 세팅할 데이터")]
    public float SkillCooltime;
    public GameObject Prefab_MonsterSkillObject;
    [SerializeField] private SpriteRenderer SpriteRenderer_Monster;



    [Header("데이터를 확인할 수 있도록 임시로 열어줌")]
    public int _instanceId; // 게임 오브젝트 매니저에서 찾기 용 - 게임에 태어날때 부여되는 나의 주민번호
    public string _dataId; // 데이터드리븐용 - 나의 부가정보를 나중에 찾을 수 있는 Id

    [Header("받아왔는데 전투에서 필요한 데이터")]
    private DNMonsterData _monsterData;
    public int _baseHp;
    public int _baseAtk;
    public bool _isAlive = true;
    private bool _lookRight = true;

    private Vector3 _moveDirection;


    private void OnDisable()
    {
        _isAlive = false;
    }

    public void InitMonster(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _dataId = dataId;


        var monsterData = DaniTechGameDataManager.Instance.GetDNMonsterData(dataId);
        if (monsterData != null)
        {
            _monsterData = monsterData;
            _baseHp = _monsterData.BaseHp;
            _baseAtk = _monsterData.BaseAtk;
        }

        StartCoroutine(CheckAndUseSkill());
    }
    
    private int GetFianlNormalAtkDamage(int baseAtk, float normalAtkMultiple)
    {
        return GetFinalSkillDamage(baseAtk, normalAtkMultiple);
    }

    private int GetFinalSkillDamage(int baseAtk, float skillMultiple)
    {
        return (int)(baseAtk * skillMultiple);
    }


    IEnumerator CheckAndUseSkill()
    {
        while (_isAlive)
        {
            yield return new WaitForSeconds(SkillCooltime);

            if (_isAlive == false)
            {
                break;
            }

            ChangeMonsterDirection();
            UesSkill();
        }
    }

    void ChangeMonsterDirection()
    {
        _lookRight = !_lookRight;
        _moveDirection = new Vector3(_lookRight ? 1 : -1, 0, 0);
        SetMeshDirectionByMoveDirection((int)_moveDirection.x);
    }


    void SetMeshDirectionByMoveDirection(int x)
    {
        // + 디테일을 살리기 위해 방향에 따라 캐릭터 리소스를 뒤집는다
        // 역시 중요한 로직은 아니다!
        SpriteRenderer_Monster.flipX = (x < 0);
    }

    private void UesSkill()
    {
        var gObj = Instantiate(Prefab_MonsterSkillObject, DaniTechGameObjectManager.Inst.transform);
        if (gObj == null) return;

        var skillProjectileComponent = gObj.GetComponent<SkillProjectile>();
        if (skillProjectileComponent == null) return;

        float skillMultiple = _monsterData.SkillAtkMultipleList.Count > 0 ? _monsterData.SkillAtkMultipleList[0] : 0;
        int finalSkillDamage = GetFinalSkillDamage(_baseAtk, skillMultiple);
        skillProjectileComponent.InitSkillObject(_instanceId, _lookRight, this.transform.position, finalSkillDamage);
    }




}
