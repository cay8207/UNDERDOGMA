using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DamagedState : BaseState
{
    // 원래 캐릭터의 위치를 저장하기 위한 변수. 만약 위치가 바뀌었으면, 즉 kickboss에게 차이거나 했으면 한번 더 데미지를 받아야 한다. 
    Vector2Int characterOriginPosition;

    public DamagedState(Character character) : base(character)
    {

    }

    public override void OnStateEnter()
    {
        characterOriginPosition = new Vector2Int(_character.Row, _character.Col);

        EnemyTurn();
    }

    public override void OnStateUpdate()
    {
        if (_character.IsCharacterCoroutineRunning == false)
        {
            // 이동이 끝나고, 적의 턴까지 끝났을때에 몇가지 경우의 수가 존재한다.
            // 1. 만약 hp가 0 이하라면 죽는다.
            if (_character.Heart <= 0)
            {
                _character.ChangeState(Character.State.Death);
                return;
            }
            // 2. 만약 죽지 않았고
            else
            {
                // 2.1. 캐릭터의 위치가 바뀌었다면, 즉 kickboss에게 차이거나 했으면 한번 더 데미지를 받아야 한다.
                if (characterOriginPosition != new Vector2Int(_character.Row, _character.Col))
                {
                    _character.ChangeState(Character.State.Damaged);
                }
                else
                {
                    // 2.2. 처형 턴이라면 ExecutionState로 넘어간다. 
                    if (_character.MoveCount == ExecutionManager.Instance.ExecutionCount)
                    {
                        _character.ChangeState(Character.State.Execution);
                    }
                    // 2.3. 그렇지 않다면 IdleState로 넘어간다.
                    else
                    {
                        _character.ChangeState(Character.State.Idle);
                    }
                }
            }
        }
    }

    public override void OnStateExit()
    {

    }

    public void EnemyTurn()
    {
        Vector2Int PlayerPosition = new Vector2Int(StageManager.Instance._character.GetComponent<Character>().Row,
                                                    StageManager.Instance._character.GetComponent<Character>().Col);
        // 적의 턴이 시작되면 적들의 action을 모두 실행. 
        foreach (var gameObject in StageManager.Instance.GameObjectDictionary)
        {
            int enemyRow = gameObject.Key.x;
            int enemyCol = gameObject.Key.y;
            int amount = 0;

            if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].Type == TileType.Enemy)
            {
                // 0번 타입 적인경우
                if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].EnemyData.EnemyType == EnemyType.NormalEnemy)
                {
                    Type type = gameObject.Value.GetComponent<NormalEnemy>().GetType();
                    amount = gameObject.Value.GetComponent<NormalEnemy>().CheckCharacterDamaged(PlayerPosition.x, PlayerPosition.y);
                    if (amount > 0)
                    {
                        _character._keyDownQueue.Clear();
                        _character.EnqueueCoroutine(_character.CharacterDamaged(amount));
                    }
                }
                // 1번 타입 적인 경우
                else if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].EnemyData.EnemyType == EnemyType.Chaser)
                {
                    amount = gameObject.Value.GetComponent<Chaser>().CheckCharacterDamaged(PlayerPosition.x, PlayerPosition.y);
                    if (amount > 0)
                    {
                        _character._keyDownQueue.Clear();
                        _character.EnqueueCoroutine(_character.CharacterDamaged(amount));
                    }
                }
                // 2번 타입 적인 경우
                else if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].EnemyData.EnemyType == EnemyType.MiniBoss)
                {
                    amount = gameObject.Value.GetComponent<MiniBoss>().CheckCharacterDamaged(PlayerPosition.x, PlayerPosition.y);
                    if (amount > 0)
                    {
                        _character._keyDownQueue.Clear();
                        _character.EnqueueCoroutine(_character.CharacterDamaged(amount));
                    }
                }
                else if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].EnemyData.EnemyType == EnemyType.StrongAttack)
                {
                    amount = gameObject.Value.GetComponent<StrongAttack>().CheckCharacterDamaged(PlayerPosition.x, PlayerPosition.y);
                    if (amount > 0)
                    {
                        _character._keyDownQueue.Clear();
                        _character.EnqueueCoroutine(_character.CharacterDamaged(amount));
                    }
                }
                else if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].EnemyData.EnemyType == EnemyType.AllDirectionsAttack)
                {
                    amount = gameObject.Value.GetComponent<AllDirection>().CheckCharacterDamaged(PlayerPosition.x, PlayerPosition.y);
                    if (amount > 0)
                    {
                        _character._keyDownQueue.Clear();
                        _character.EnqueueCoroutine(_character.CharacterDamaged(amount));
                    }
                }
                else if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].EnemyData.EnemyType == EnemyType.Angel)
                {
                    amount = gameObject.Value.GetComponent<Angel>().CheckCharacterDamaged(PlayerPosition.x, PlayerPosition.y);
                    if (amount > 0)
                    {
                        _character._keyDownQueue.Clear();
                        _character.EnqueueCoroutine(_character.CharacterDamaged(amount));
                    }
                }
                else if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].EnemyData.EnemyType == EnemyType.KickBoss)
                {
                    amount = gameObject.Value.GetComponent<KickBoss>().CheckCharacterDamaged(PlayerPosition.x, PlayerPosition.y);
                    if (amount > 0)
                    {
                        _character._keyDownQueue.Clear();
                        _character.EnqueueCoroutine(_character.CharacterDamaged(amount));
                    }
                }
            }
        }

        EnemyManager.Instance.ChaserUpdate();
        EnemyManager.Instance.AngelUpdate();
    }
}
