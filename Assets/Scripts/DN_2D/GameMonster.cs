using UnityEngine;

public class GameMonster : MonsterBase
{

    private int _instanceId; // 게임 오브젝트 매니저에서 찾기 용 - 게임에 태어날때 부여되는 나의 주민번호
    private string _dataId; // 데이터드리븐용 - 나의 부가정보를 나중에 찾을 수 있는 Id


    public void InitMonster(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _dataId = dataId;
    }

}
