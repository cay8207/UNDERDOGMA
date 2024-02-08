using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class Character : MonoBehaviour
{
    #region variables
    // 1. 유니티 인스펙터 창에서 설정할 변수들. 프리팹과 오브젝트 등을 담아준다.
    [SerializeField] public GameObject _heartText;

    // 2. 기본적인 변수들. 생명력, 행, 열 등을 저장. 
    private int _heart;
    public int Heart
    {
        get => _heart;
        set => _heart = value;
    }

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

    // 3. 타일, 적의 정보가 있는 딕셔너리들을 조금 더 사용하기 쉽게 변수 선언. 원래는 게임매니저에서 읽어온다. 
    private Dictionary<Vector2Int, GameObject> EnemyDictionary => StageManager.Instance.EnemyDictionary;
    private Dictionary<Vector2Int, GameObject> MeatDictionary => StageManager.Instance.MeatDictionary;

    // 4. 코드의 간결성을 위해 설정한 변수들. 
    // 4.1. 상하좌우 방향을 편하게 관리하기 위해 directionOffsets를 선언. 
    public Vector2Int[] directionOffsets = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(1, 0)    // Right
        };

    // 5. 스테이트 머신과 관련된 설정. enum, fsm 등을 선언한다. 

    public enum State
    {
        Idle,
        Move,
        Attack,
        Damaged,
        Meat,
        Execution,
        Death,
        Reset,
    }

    private State _curState;
    private FSM _fsm;

    // 6. 캐릭터의 이동을 관리하기 위한 변수들. 
    private int _moveCount;
    public int MoveCount
    {
        get => _moveCount;
        set => _moveCount = value;
    }
    private Queue<KeyCode> _keyDownQueue = new Queue<KeyCode>();

    #endregion



    private void Start()
    {
        _curState = State.Idle;
        _fsm = new FSM(new IdleState(this));
    }

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
    private void Update()
    {
        switch (_curState)
        {
            case State.Idle:
                // TODO: 현재와 같은 방식이면 벽이 있는 방향으로 여러번 클릭시 여러 프레임 이후에야 이동이 가능.
                // 이를 방지하기 위해 while문을 돌려줘야 할 것 같다.
                if (_keyDownQueue.Count > 0)
                {
                    KeyCode key = _keyDownQueue.Dequeue();
                    if (WhatIsNextPosition(key) == TileType.Enemy)
                    {
                        ChangeState(State.Attack);
                    }
                    else if (WhatIsNextPosition(key) == TileType.Wall)
                    {

                    }
                    else
                    {
                        ChangeState(State.Move);
                    }
                }
                break;
            case State.Move:
                break;
            case State.Attack:
                break;
            case State.Damaged:
                break;
            case State.Meat:
                break;
            case State.Execution:
                break;
            case State.Death:
                break;
            case State.Reset:
                break;
        }

        if (_moveCount + _keyDownQueue.Count < ExecutionManager.Instance.ExecutionCount)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                _keyDownQueue.Enqueue(KeyCode.W);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                _keyDownQueue.Enqueue(KeyCode.S);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _keyDownQueue.Enqueue(KeyCode.A);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                _keyDownQueue.Enqueue(KeyCode.D);
            }
        }
    }

    public void ChangeState(State nextState)
    {
        _curState = nextState;
        switch (_curState)
        {
            case State.Idle:
                _fsm.ChangeState(new IdleState(this));
                break;
            case State.Damaged:
                _fsm.ChangeState(new DamagedState(this));
                break;
            case State.Execution:
                _fsm.ChangeState(new ExecutionState(this));
                break;
            case State.Death:
                _fsm.ChangeState(new DeathState(this));
                break;
            case State.Reset:
                _fsm.ChangeState(new ResetState(this));
                break;
        }
    }

    // 이동하는 경우에 어떤 방향으로 이동하는지에 대한 정보도 필요하다. 
    public void ChangeState(State nextState, KeyCode key)
    {
        _curState = nextState;
        switch (_curState)
        {
            case State.Move:
                _fsm.ChangeState(new MoveState(this, key));
                break;
        }
    }

    // AttackState, Meat의 경우 어떤 위치의 오브젝트와 상호작용해야 하는지에 대한 정보도 필요하다. 
    public void ChangeState(State nextState, Vector2Int targetPosition)
    {
        _curState = nextState;
        switch (_curState)
        {
            case State.Attack:
                _fsm.ChangeState(new AttackState(this, targetPosition));
                break;
            case State.Meat:
                _fsm.ChangeState(new MeatState(this, targetPosition));
                break;
        }
    }

    // 캐릭터의 다음 칸에 어떤 오브젝트가 있는지 반환한다. 
    private TileType WhatIsNextPosition(KeyCode key)
    {
        Vector2Int targetPosition = new Vector2Int(_row, _col);
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
        TileObject tileObject = StageManager.Instance.TempTileDictionary[targetPosition];

        if (tileObject.Type == TileType.Enemy)
        {
            if (tileObject.EnemyData.IsAlive == true && _heart > tileObject.EnemyData.Heart)
            {
                ChangeState(State.Attack, targetPosition);
                return TileType.Enemy; ;
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

    public void UpdatePosition(int row, int col)
    {
        _row = row;
        _col = col;
    }

    // 캐릭터의 체력 변화를 관리하는 함수. 
    public void HeartChange(int amount)
    {
        _heart += amount;
        _heartText.GetComponent<Text>().SetText(_heart);
    }
}
