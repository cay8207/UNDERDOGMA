using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DamagedState : BaseState
{
    public DamagedState(Character character) : base(character)
    {

    }

    public override void OnStateEnter()
    {
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
            else
            {
                _character.ChangeState(Character.State.Clear);

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
                    amount = gameObject.Value.GetComponent<NormalEnemy>().CheckCharacterDamaged(PlayerPosition.x, PlayerPosition.y);
                    if (amount > 0)
                    {
                        _character.EnqueueCoroutine(_character.CharacterDamaged(amount));
                    }
                }
                // 1번 타입 적인 경우
                else if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].EnemyData.EnemyType == EnemyType.ChaserEnemy)
                {
                    amount = gameObject.Value.GetComponent<ChaserEnemy>().CheckCharacterDamaged(PlayerPosition.x, PlayerPosition.y);
                    if (amount > 0)
                    {
                        _character.EnqueueCoroutine(_character.CharacterDamaged(amount));
                    }
                }
                // 2번 타입 적인 경우
                else if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].EnemyData.EnemyType == EnemyType.MiniBoss)
                {
                    amount = gameObject.Value.GetComponent<MiniBoss>().CheckCharacterDamaged(PlayerPosition.x, PlayerPosition.y);
                    if (amount > 0)
                    {
                        _character.EnqueueCoroutine(_character.CharacterDamaged(amount));
                    }
                }
            }
        }

        EnemyManager.Instance.ChaserEnemyUpdate();
    }
}
