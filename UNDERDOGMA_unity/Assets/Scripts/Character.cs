using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class Character : MonoBehaviour
{
    [SerializeField] public GameObject _heartText;

    // 플레이어 생명력 설정. 
    private int _heart;
    public int Heart
    {
        get => _heart;
        set => _heart = value;
    }

    // 플레이어의 현재 위치를 저장. 
    private int _row, _col;

    public int Row
    {
        get => _row;
        set => _row = value;
    }

    public int Col
    {
        get => _col;
        set => _col = value;
    }

    // 타일, 적의 정보가 있는 딕셔너리들을 조금 더 사용하기 쉽게 변수 선언. 원래는 게임매니저에서 읽어온다. 
    private Dictionary<Vector2Int, List<int>> TileDictionary => StageManager.Instance._stageData.TileDictionary;
    private Dictionary<Vector2Int, GameObject> EnemyDictionary => StageManager.Instance.EnemyDictionary;
    private Dictionary<Vector2Int, GameObject> MeatDictionary => StageManager.Instance.MeatDictionary;

    // 상하좌우 방향을 편하게 관리하기 위해 선언. 
    private Vector2Int[] directionOffsets = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(1, 0)    // Right
        };

    private int _moveCount;
    private bool _executionInProgress => ExecutionManager.Instance.ExecutionInProgress;

    // Start is called before the first frame update
    // 플레이어의 정보들을 저장해준다. 
    public void Init(int row, int col, int heart)
    {
        // 게임이 재시작할때에도 해당 코드가 실행된다. 모든 변수들 초기화해두기. 
        this._row = row;
        this._col = col;
        this._heart = heart;
        _heartText.GetComponent<Text>().SetText(_heart);
        _moveCount = 0;
    }

    // Update is called once per frame
    // 캐릭터가 움직인 횟수를 체크해서 ExecutionManager에 넘겨주면 거기에서 처형 여부를 판단. 
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && !_executionInProgress)
        {
            StartCoroutine(CharacterMove(0));
            if (ExecutionManager.Instance.ExecutionCheck(_moveCount)) _moveCount = 0;
        }
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && !_executionInProgress)
        {
            StartCoroutine(CharacterMove(1));
            if (ExecutionManager.Instance.ExecutionCheck(_moveCount)) _moveCount = 0;
        }
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && !_executionInProgress)
        {
            StartCoroutine(CharacterMove(2));
            if (ExecutionManager.Instance.ExecutionCheck(_moveCount)) _moveCount = 0;
        }
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && !_executionInProgress)
        {
            StartCoroutine(CharacterMove(3));
            if (ExecutionManager.Instance.ExecutionCheck(_moveCount)) _moveCount = 0;
        }
    }

    IEnumerator CharacterMove(int direction)
    {
        // 다음으로 이동할 칸의 위치를 설정해준다. 
        Debug.Log("now row: " + _row + " now col: " + _col);
        Vector2Int targetPosition = new Vector2Int(_row, _col) + directionOffsets[direction];

        // 캐릭터가 이동하지 않는 경우에는 적들에게 데미지를 받는 등의 이벤트가 발생하면 안된다. 
        bool isMove = false;

        // 현재 칸에서 이동하려는 칸에 적이 있다면 해당 적을 처치한다. 그리고 함수 종료. 
        if (TileDictionary[targetPosition][0] == 1)
        {
            if (TileDictionary[targetPosition][2] == 1 && _heart > TileDictionary[targetPosition][4])
            {
                _moveCount++;
                StartCoroutine(CharacterAttack(targetPosition));
                _heart += TileDictionary[targetPosition][4];
                _heartText.GetComponent<Text>().SetText(_heart);
                yield return null;
            }
        }

        while (true)
        {
            Debug.Log("next row: " + targetPosition.x + " " + "next col: " + targetPosition.y);
            // 다음으로 이동할 위치를 지정해준다. 
            int tileType = TileDictionary[targetPosition][0];

            // 만약 해당 칸에 하트가 있다면 하트 오브젝트를 파괴하고 그만큼 체력을 회복한다.
            if (tileType == 2)
            {
                if (TileDictionary[targetPosition][1] == 1)
                {
                    AudioManager.Instance.PlaySfx(AudioManager.Sfx.Meat);

                    _heart += MeatDictionary[targetPosition].GetComponent<Meat>().Heart;
                    _heartText.GetComponent<Text>().SetText(_heart);
                    MeatDictionary[targetPosition].GetComponent<Meat>().EatMeat(targetPosition);
                }
            }

            // 벽이나 적을 만난 경우 while문을 종료해준다. 
            if (tileType == -1)
            {
                break;
            }
            // 적이 있고, 살아있는 경우 멈춘다. 
            else if (tileType == 1)
            {
                Debug.Log("Enemy is here");
                if (TileDictionary[targetPosition][2] == 1)
                {
                    Debug.Log("Enemy is alive");
                    break;
                }
                else
                {
                    Debug.Log("Enemy is dead");
                    isMove = true;
                    // transform.position += new Vector3(directionOffsets[direction].x, directionOffsets[direction].y, 0);
                    _row = targetPosition.x;
                    _col = targetPosition.y;

                    // 다음으로 이동할 칸을 계속 업데이트해줘야 한다. 
                    targetPosition += directionOffsets[direction];
                }
            }
            // 이외의 경우 해당 칸으로 한 칸 더 전진. 
            else
            {
                isMove = true;
                // transform.position += new Vector3(directionOffsets[direction].x, directionOffsets[direction].y, 0);
                _row = targetPosition.x;
                _col = targetPosition.y;

                // 다음으로 이동할 칸을 계속 업데이트해줘야 한다. 
                targetPosition += directionOffsets[direction];

                // while문이 무한루프에 빠지는 것을 방지하기 위해 범위를 제한해준다.
                if (_row > 100 || _row < -100 || _col > 100 || _col < -100) break;
            }
        }

        transform.DOMove(new Vector3(_row - 0.07f, _col + 0.35f, 0), 1.0f, false).SetEase(Ease.OutCirc);

        // while문을 탈출한 후, 즉 이동이 끝난 후 상하좌우에 적이 있다면 데미지를 입어야 한다.
        if (isMove)
        {
            _heart--;
            _heartText.GetComponent<Text>().SetText(_heart);

            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Move);

            _moveCount++;

            // 적의 턴을 진행하는 코드. 
            EnemyManager.Instance.EnemyTurn();
            if (_heart <= 0)
            {
                StartCoroutine(CharacterDeath());
            }
        }
    }

    public IEnumerator CharacterAttack(Vector2Int targetPosition)
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Eat);

        // 캐릭터가 공격하는 애니메이션 재생. 
        gameObject.GetComponent<Animator>().SetBool("IsAttack", true);

        yield return new WaitForSeconds(0.9f);

        gameObject.GetComponent<Animator>().SetBool("IsAttack", false);

        // 공격한 적을 죽인다. 
        EnemyManager.Instance.EnemyDeath(targetPosition);
    }

    // 캐릭터가 죽었을 경우 죽는 애니메이션을 보여주고 게임을 재시작한다. 
    public IEnumerator CharacterDeath()
    {
        _heart = 0;
        _heartText.GetComponent<Text>().SetText(_heart);

        StartCoroutine(FadeOut(gameObject.GetComponent<SpriteRenderer>(), 1.0f));

        yield return new WaitForSeconds(1.0f);

        // 죽는 모션 추가되면 수정할 예정. 
        // gameObject.GetComponent<Animator>().SetBool("IsDied", true);

        // yield return new WaitForSeconds(1.0f);

        // gameObject.GetComponent<Animator>().SetBool("IsDied", false);

        StageManager.Instance.ResetGame();
    }

    public IEnumerator FadeOut(SpriteRenderer spriteRenderer, float time)
    {
        Color color = spriteRenderer.color;
        while (color.a > 0f)
        {
            color.a -= Time.deltaTime / time;
            spriteRenderer.color = color;
            yield return null;
        }
    }
}
