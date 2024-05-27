using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Ball : Enemy
{
    public Vector2Int[] directionOffsets = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(1, 0)    // Right
        };

    public IEnumerator Kick(Vector2Int nowPosition, KeyCode key)
    {
        TileType tileType = WhatIsNextPosition(nowPosition, key);

        if (tileType == TileType.Empty)
        {
            Vector2Int targetPosition = CheckBallWhereToGo(nowPosition, key);

            // 1. 공을 이동시킨다. 
            transform.DOMove(new Vector3(targetPosition.x, targetPosition.y, 0), 0.5f);

            // 2. 공의 위치를 업데이트한다. 
            StageManager.Instance.TempTileDictionary[nowPosition].Type = TileType.Empty;
            StageManager.Instance.TempTileDictionary[targetPosition].Type = TileType.Ball;

            StageManager.Instance.GameObjectDictionary[targetPosition] = gameObject;
            StageManager.Instance.GameObjectDictionary.Remove(nowPosition);
        }

        yield return null;
    }

    public Vector2Int CheckBallWhereToGo(Vector2Int nowPosition, KeyCode key)
    {
        Vector2Int targetPosition = FindNextPosition(key, nowPosition);

        while (true)
        {
            TileObject tileObject = StageManager.Instance.TempTileDictionary[targetPosition];

            // 1. 이동하려는 칸에 벽이 있는 경우 멈춘다.
            if (tileObject.Type == TileType.Wall || (tileObject.Type == TileType.Enemy && tileObject.EnemyData.IsAlive == true)
                || (tileObject.Type == TileType.Meat) || (tileObject.Type == TileType.Ball))
            {
                // 1.1. targetPosition에 벽이 있으므로, 이전 위치로 돌아가야 한다. 
                targetPosition -= returnDirection(key);
                break;
            }

            // 4. 다음으로 이동할 칸을 계속 업데이트해줘야 한다. 
            targetPosition += returnDirection(key);

            // 5. while문이 무한루프에 빠지는 것을 방지하기 위해 범위를 제한해준다.
            if (targetPosition.x > 100 || targetPosition.x < -100 || targetPosition.y > 100 || targetPosition.y < -100)
            {
                Debug.Log("캐릭터 이동 관련해서 무한루프 발생! 맵 오브젝트(적, 벽, 고기 등)의 위치를 확인해주세요! (MoveState.cs)");
                break;
            }
        }

        return targetPosition;
    }

    private TileType WhatIsNextPosition(Vector2Int targetPosition, KeyCode key)
    {
        // 키의 방향에 따라 다음 위치가 어딘지를 반환한다. 
        Vector2Int newTargetPosition = FindNextPosition(key, targetPosition);

        TileObject tileObject = StageManager.Instance.TempTileDictionary[newTargetPosition];

        if (tileObject.Type == TileType.Enemy)
        {
            if (tileObject.EnemyData.IsAlive == true)
            {
                return TileType.Enemy;
            }
            else if (tileObject.EnemyData.IsAlive == false)
            {
                return TileType.Empty;
            }
        }
        else if (tileObject.Type == TileType.Wall)
        {
            return TileType.Wall;
        }
        else
        {
            return TileType.Empty;
        }

        return TileType.Invalid;
    }

    private Vector2Int FindNextPosition(KeyCode key, Vector2Int targetPosition)
    {
        switch (key)
        {
            case KeyCode.W:
                targetPosition += directionOffsets[0];
                break;
            case KeyCode.S:
                targetPosition += directionOffsets[1];
                break;
            case KeyCode.A:
                targetPosition += directionOffsets[2];
                break;
            case KeyCode.D:
                targetPosition += directionOffsets[3];
                break;
        }
        return targetPosition;
    }

    private Vector2Int returnDirection(KeyCode key)
    {
        if (key == KeyCode.W)
        {
            return new Vector2Int(0, 1);
        }
        else if (key == KeyCode.S)
        {
            return new Vector2Int(0, -1);
        }
        else if (key == KeyCode.A)
        {
            return new Vector2Int(-1, 0);
        }
        else if (key == KeyCode.D)
        {
            return new Vector2Int(1, 0);
        }
        else
        {
            return new Vector2Int(0, 0);
        }
    }
}
