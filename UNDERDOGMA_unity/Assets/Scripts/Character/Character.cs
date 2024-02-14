using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

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

    // 3. 게임 오브젝트 딕셔너리를 조금 더 사용하기 쉽게 변수 선언. 원래는 게임매니저에서 읽어온다. 
    private Dictionary<Vector2Int, GameObject> GameObjectDictionary => StageManager.Instance.GameObjectDictionary;

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
        Clear,
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
    public Queue<KeyCode> _keyDownQueue = new Queue<KeyCode>();

    // 7. 캐릭터의 코루틴을 관리하기 위한 변수. 큐로 관리하므로 하나의 변수만 필요.

    private bool _isCharacterCoroutineRunning;
    public bool IsCharacterCoroutineRunning
    {
        get => _isCharacterCoroutineRunning;
        set => _isCharacterCoroutineRunning = value;
    }

    CoroutineController _coroutineController;

    private MainCamera _mainCamera;

    #endregion


    // 1. 기본 Start, Init, Update와 관련된 함수들. 
    private void Start()
    {
        _curState = State.Idle;
        _fsm = new FSM(new IdleState(this));
        _coroutineController = GetComponent<CoroutineController>();
        _mainCamera = Camera.main.GetComponent<MainCamera>();
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
        if (!_isCharacterCoroutineRunning && !DialogueManager.Instance._isDialogueRunning)
        {
            Debug.Log("Character Coroutine doesn't running");
            switch (_curState)
            {
                case State.Idle:
                    if (_moveCount == Execution.Instance.ExecutionCount)
                    {
                        ChangeState(State.Execution);
                        _moveCount = 0;
                        _keyDownQueue.Clear();
                        break;
                    }

                    for (int i = 0; i < _keyDownQueue.Count; i++)
                    {
                        Debug.Log("keydownQueue: " + _keyDownQueue.ToArray()[i]);
                    }

                    // TODO: 현재와 같은 방식이면 벽이 있는 방향으로 여러번 클릭시 여러 프레임 이후에야 이동이 가능.
                    // 이를 방지하기 위해 while문을 돌려줘야 할 것 같다.
                    if (_keyDownQueue.Count > 0)
                    {
                        KeyCode key = _keyDownQueue.Dequeue();
                        Debug.Log("Next Position: " + WhatIsNextPosition(key));
                        if (WhatIsNextPosition(key) == TileType.Enemy)
                        {
                            if (Heart > StageManager.Instance.TempTileDictionary[FindNextPosition(key, new Vector2Int(_row, _col))].EnemyData.Heart)
                            {
                                ChangeState(State.Attack, FindNextPosition(key, new Vector2Int(_row, _col)));
                            }
                            else
                            {

                            }
                        }
                        else if (WhatIsNextPosition(key) == TileType.Wall)
                        {

                        }
                        else
                        {
                            ChangeState(State.Move, key);
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
                case State.Clear:
                    break;
            }

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
            else if (Input.GetKeyDown(KeyCode.R))
            {
                ChangeState(State.Reset);
            }
        }

        _coroutineController.ExecuteCoroutine();

        _fsm.UpdateState();

        // 처형당할 적을 표시해주는 코드. 
    }

    // 2. state machine과 관련된 함수들. 
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
            case State.Clear:
                _fsm.ChangeState(new ClearState(this));
                break;
        }
    }

    // moveState의 경우에는 어느 방향인지 키에 대한 정보도 필요하다. 
    public void ChangeState(State nextState, KeyCode key)
    {
        switch (nextState)
        {
            case State.Move:
                _fsm.ChangeState(new MoveState(this, key));
                break;
        }
    }

    // AttackState, MeatState의 경우에는 어느 위치에 있는 오브젝트에 대해 상호작용해야 하는지에 대한 정보도 필요하다. 
    public void ChangeState(State nextState, Vector2Int targetPosition)
    {
        switch (nextState)
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
        // 키의 방향에 따라 다음 위치가 어딘지를 반환한다. 
        Vector2Int targetPosition = FindNextPosition(key, new Vector2Int(_row, _col));

        TileObject tileObject = StageManager.Instance.TempTileDictionary[targetPosition];

        if (tileObject.Type == TileType.Enemy)
        {
            if (tileObject.EnemyData.IsAlive == true)
            {
                // ChangeState(State.Attack, targetPosition);
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


    // 3. 애니메이션과 관련된 함수들. 
    #region Animation
    public void EnqueueCoroutine(IEnumerator coroutine)
    {
        _coroutineController.EnqueueCoroutine(coroutine);
    }

    public IEnumerator CharacterMoveCoroutine(Vector2Int targetPosition)
    {
        _isCharacterCoroutineRunning = true;

        int row = targetPosition.x;
        int col = targetPosition.y;

        transform.DOMove(new Vector3(row, col, 0) + new Vector3(-0.07f, 0.35f, 0), 0.5f, false).SetEase(Ease.OutCirc);

        yield return new WaitForSeconds(0.5f);

        _isCharacterCoroutineRunning = false;
    }

    // 캐릭터가 공격하는 애니메이션을 보여준다. 
    public IEnumerator CharacterAttack(Vector2Int targetPosition)
    {
        _isCharacterCoroutineRunning = true;

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Eat);

        // 캐릭터가 공격하는 애니메이션 재생. 
        GetComponent<Animator>().SetBool("IsAttack", true);

        yield return new WaitForSeconds(0.9f);

        GetComponent<Animator>().SetBool("IsAttack", false);

        // 공격한 적을 죽인다. 
        EnemyManager.Instance.EnemyDeath(targetPosition);

        HeartChange(StageManager.Instance.TempTileDictionary[targetPosition].EnemyData.Heart);

        _isCharacterCoroutineRunning = false;
    }

    public IEnumerator CharacterDamaged(int amount)
    {
        _isCharacterCoroutineRunning = true;

        _mainCamera.Shake(0.5f);

        yield return new WaitForSeconds(0.5f);

        HeartChange(-amount);

        // TODO: 캐릭터가 피해를 입는 애니메이션 완성되면 넣어야 함. 
        // GetComponent<Animator>().SetBool("IsDamaged", true);

        // yield return new WaitForSeconds(0.9f);

        // GetComponent<Animator>().SetBool("IsDamaged", false);

        _isCharacterCoroutineRunning = false;
    }

    // 캐릭터가 죽었을 경우 죽는 애니메이션을 보여주고 게임을 재시작한다. 
    public IEnumerator CharacterDeath()
    {
        _isCharacterCoroutineRunning = true;

        gameObject.GetComponent<SpriteRenderer>().DOFade(0, 1.0f);

        yield return new WaitForSeconds(1.0f);

        // 죽는 모션 추가되면 수정할 예정. 
        // gameObject.GetComponent<Animator>().SetBool("IsDied", true);

        // yield return new WaitForSeconds(1.0f);

        // gameObject.GetComponent<Animator>().SetBool("IsDied", false);

        _isCharacterCoroutineRunning = false;
    }

    // 처형되는 애니메이션을 재생한다. 
    public IEnumerator ExecutionEvent()
    {
        _isCharacterCoroutineRunning = true;

        // 1. 공격, 피격 등의 애니메이션이 끝나기를 기다리기 위해 1초간 기다린다. 
        yield return new WaitForSeconds(1.0f);

        // 2. 처형 효과음 재생.
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Execute);

        // 3. 애니메이션을 3초간 재생. 
        Execution.Instance.ExecutionObject.GetComponent<Animator>().SetBool("InExecution", true);

        yield return new WaitForSeconds(3.0f);

        // 4. 애니메이션 종료.
        Execution.Instance.ExecutionObject.GetComponent<Animator>().SetBool("InExecution", false);

        Execution.Instance.ExecutionObject.GetComponent<SpriteRenderer>().sprite = null;

        // 5. 처형 이벤트 종료. 캐릭터는 다시 움직일 수 있다. 
        _isCharacterCoroutineRunning = false;
    }

    // 플레이어가 고기를 먹는 애니메이션을 재생한다.
    public IEnumerator CharacterEatMeat(Vector2Int targetPosition)
    {
        // 1. 코루틴이 실행되는 중인지를 체크하는 변수를 설정한다. 
        _isCharacterCoroutineRunning = true;

        // 2. 사운드를 재생한다. 
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Eat);

        // 3. 애니메이션을 재생한다.  
        GetComponent<Animator>().SetBool("IsAttack", true);

        yield return new WaitForSeconds(0.9f);

        GetComponent<Animator>().SetBool("IsAttack", false);

        // 4. 플레이어의 체력을 변화시키고, 고기를 사라지는 애니메이션을 실행한다. 
        Meat meat = StageManager.Instance.GameObjectDictionary[targetPosition].GetComponent<Meat>();

        HeartChange(meat.Amount);

        MeatManager.Instance.EatMeat(targetPosition);

        // 5. 코루틴이 끝났다는 의미로 변수의 값을 변경해준다. 
        _isCharacterCoroutineRunning = false;
    }

    public IEnumerator ResetAnimation()
    {
        _isCharacterCoroutineRunning = true;

        Sequence ResetUpSideSequence = DOTween.Sequence();
        Sequence ResetDownSideSequence = DOTween.Sequence();

        ResetUpSideSequence
            .Append(
                StageManager.Instance.ResetAnimationUpSide.GetComponent<RectTransform>()
                .DOLocalMove(new Vector3(0.0f, 333.0f, 0.0f), 0.5f, false)
                .SetEase(Ease.InQuart))
            .AppendInterval(1.0f)
            .Append(
                StageManager.Instance.ResetAnimationUpSide.GetComponent<RectTransform>()
                .DOLocalMove(new Vector3(0.0f, 1100.0f, 0.0f), 1.0f, false));

        ResetDownSideSequence
            .Append(
                StageManager.Instance.ResetAnimationDownSide.GetComponent<RectTransform>()
                .DOLocalMove(new Vector3(0.0f, -333.0f, 0.0f), 0.5f, false)
                .SetEase(Ease.InQuart))
            .AppendInterval(1.0f)
            .Append(
                StageManager.Instance.ResetAnimationDownSide.GetComponent<RectTransform>()
                .DOLocalMove(new Vector3(0.0f, -1100.0f, 0.0f), 1.0f, false));

        yield return new WaitForSeconds(2.5f);

        _isCharacterCoroutineRunning = false;
    }

    public IEnumerator StageClear()
    {
        Debug.Log("StageClear");

        // 승리 애니메이션 추가되면 수정할 예정. 
        // _character.GetComponent<Animator>().SetBool("StageClear", true);

        // yield return new WaitForSeconds(2.0f);

        // _character.GetComponent<Animator>().SetBool("StageClear", false);

        yield return new WaitForSeconds(2.0f);

        if (StageManager.Instance.stage == 10)
        {
            SceneManager.LoadScene("Ending");
        }
        else
        {
            SceneManager.LoadScene("Stage" + (StageManager.Instance.stage + 1).ToString());
        }
    }

    #endregion
}
