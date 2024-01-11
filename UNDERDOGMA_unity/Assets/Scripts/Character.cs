using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Character : MonoBehaviour
{
    [SerializeField] GameObject _heartText;
    [SerializeField] Sprite _devilDog;
    [SerializeField] Sprite _deadDog;

    private int _heart;
    public int Heart
    {
        get => _heart;
        set => _heart = value;
    }
    private int row, col;
    private Dictionary<Vector2Int, List<int>> TileDictionary => GameManager.Instance.TileDictionary;

    // 상하좌우 방향을 편하게 관리하기 위해 선언. 
    private Vector2Int[] directionOffsets = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(1, 0)    // Right
        };

    // Start is called before the first frame update
    void Start()
    {
        row = 0; col = 0;
        _heart = 6;

        gameObject.GetComponent<SpriteRenderer>().sprite = _devilDog;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            CharacterMove(0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            CharacterMove(1);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            CharacterMove(2);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            CharacterMove(3);
        }
    }

    void CharacterMove(int direction)
    {
        // 다음으로 이동할 칸의 위치를 설정해준다. 
        Vector2Int targetPosition = new Vector2Int(row, col) + directionOffsets[direction];

        // 캐릭터가 이동하지 않는 경우에는 적들에게 데미지를 받는 등의 이벤트가 발생하면 안된다. 
        bool isMove = false;

        // 현재 칸에서 이동하려는 칸에 적이 있다면 해당 적을 처치한다. 그리고 함수 종료. 
        if (TileDictionary[targetPosition][0] == 1 && _heart > TileDictionary[targetPosition][2])
        {
            Destroy(GameManager.Instance.EnemyDictionary[targetPosition]);
            GameManager.Instance.TileDictionary[targetPosition][0] = 0;
            return;
        }

        while (TileDictionary[new Vector2Int(row, col)][0] == 0)
        {
            // 다음으로 이동할 위치를 지정해준다. 
            int tileType = TileDictionary[targetPosition][0];

            // 벽이나 적을 만난 경우 while문을 종료해준다. 
            if (tileType == -1 || tileType == 1)
            {
                break;
            }
            // 이외의 경우 해당 칸으로 한 칸 더 전진. 
            else
            {
                isMove = true;
                transform.position += new Vector3(directionOffsets[direction].x, directionOffsets[direction].y, 0);
                row = targetPosition.x;
                col = targetPosition.y;

                // 다음으로 이동할 칸을 계속 업데이트해줘야 한다. 
                targetPosition += directionOffsets[direction];
            }
        }

        // while문을 탈출한 후, 즉 이동이 끝난 후 상하좌우에 적이 있다면 데미지를 입어야 한다.
        if (isMove)
        {
            checkEnemy();
        }
    }

    void checkEnemy()
    {
        // 상하좌우에 적이 있다면 데미지를 입는다. 
        foreach (Vector2Int direction in directionOffsets)
        {
            Vector2Int targetPosition = new Vector2Int(row, col) + direction;
            if (TileDictionary[targetPosition][0] == 1)
            {
                _heart -= TileDictionary[targetPosition][1];
                _heartText.GetComponent<TextMeshPro>().text = _heart.ToString();

                // 생명력이 0 이하이면 죽는 이벤트를 진행시킨다. 
                if (_heart <= 0)
                {
                    ChracterDeath();
                    break;
                }
            }
        }
    }

    void ChracterDeath()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _deadDog;
        _heart = 0;
        _heartText.GetComponent<TextMeshPro>().text = _heart.ToString();
    }
}
