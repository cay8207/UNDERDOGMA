using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Drawing;
using UnityEditor.SceneManagement;

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
        EndingDialogueState,
        Kick
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

    public Vector2 characterBias = new Vector2(-0.07f, 0.35f);

    // 7. 캐릭터의 코루틴을 관리하기 위한 변수. 큐로 관리하므로 하나의 변수만 필요.

    private bool _isCharacterCoroutineRunning;
    public bool IsCharacterCoroutineRunning
    {
        get => _isCharacterCoroutineRunning;
        set => _isCharacterCoroutineRunning = value;
    }

    private bool _isCharacterExecutionCoroutineRunning;
    public bool IsCharacterExecutionCoroutineRunning
    {
        get => _isCharacterExecutionCoroutineRunning;
        set => _isCharacterExecutionCoroutineRunning = value;
    }

    private bool _isCharacterResetCoroutineRunning = false;
    public bool IsCharacterResetCoroutineRunning
    {
        get => _isCharacterResetCoroutineRunning;
        set => _isCharacterResetCoroutineRunning = value;
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
            switch (_curState)
            {
                case State.Idle:
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
                        else if (WhatIsNextPosition(key) == TileType.Ball)
                        {
                            ChangeState(State.Kick, FindNextPosition(key, new Vector2Int(_row, _col)), key);
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
                case State.EndingDialogueState:
                    break;
                case State.Kick:
                    break;
            }
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
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Reset);
            ChangeState(State.Reset);
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
            case State.EndingDialogueState:
                _fsm.ChangeState(new EndingDialogueState(this));
                break;
        }
    }

    // moveState의 경우에는 어느 방향인지 키에 대한 정보도 필요하다. 
    public void ChangeState(State nextState, KeyCode key)
    {
        _curState = nextState;
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
        _curState = nextState;
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

    // KickState의 경우 어느 방향으로 차줬는지 kick까지 필요하다. 
    public void ChangeState(State nextState, Vector2Int targetPosition, KeyCode key)
    {
        _curState = nextState;
        switch (nextState)
        {
            case State.Kick:
                _fsm.ChangeState(new KickState(this, targetPosition, key));
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
                return TileType.Enemy;
            }
            else if (tileObject.EnemyData.IsAlive == false)
            {
                return TileType.Empty;
            }
        }
        else if (tileObject.Type == TileType.Ball)
        {
            return TileType.Ball;
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

    public Vector3 SetPositionSpriteToUI(float x, float y)
    {
        float resultx = (x - _mainCamera.transform.position.x) * 1920.0f / 14.0f;
        float resulty = (y - _mainCamera.transform.position.y) * 1080.0f / 8.0f;
        return new Vector3(resultx, resulty, 0.0f);
    }

    // 3. 애니메이션과 관련된 함수들. 
    #region Animation
    public void EnqueueCoroutine(IEnumerator coroutine)
    {
        _coroutineController.EnqueueCoroutine(coroutine);
    }

    // 캐릭터가 이동하는 애니메이션을 보여준다.
    public IEnumerator CharacterMoveCoroutine(Vector2Int targetPosition)
    {
        // 1. 코루틴이 실행되는 동안 다른 코루틴이 실행되지 않도록, isCharacterCoroutineRunning을 true로 설정한다.
        _isCharacterCoroutineRunning = true;



        // 3. 캐릭터가 이동하는 애니메이션을 재생한다.
        transform.DOMove(new Vector2(targetPosition.x, targetPosition.y) + characterBias, 0.5f, false).SetEase(Ease.OutCirc);

        // 4. 캐릭터가 시작하는 타일과 도착하는 타일이 살짝 흔들리는 애니메이션을 재생한다.
        Sequence StartTileMoveSequence = DOTween.Sequence();

        StartTileMoveSequence
            .Append(StageManager.Instance.TileObjectDictionary[new Vector2Int(_row, _col)].transform
                .DOMove(new Vector2(_row, _col) + characterBias - new Vector2(targetPosition.x - _row, targetPosition.y - _col) * 0.1f, 0.1f, false)
                .SetEase(Ease.OutCirc))
        .Append(StageManager.Instance.TileObjectDictionary[new Vector2Int(_row, _col)].transform
                .DOMove(new Vector2(_row, _col) + characterBias + new Vector2(targetPosition.x - _row, targetPosition.y - _col) * 0.1f, 0.1f, false)
            .SetEase(Ease.OutCirc));

        Sequence EndTileMoveSequence = DOTween.Sequence();

        EndTileMoveSequence
            .Append(StageManager.Instance.TileObjectDictionary[targetPosition].transform
            .DOMove(new Vector2(targetPosition.x, targetPosition.y) + characterBias + new Vector2(targetPosition.x - _row, targetPosition.y - _col) * 0.1f, 0.1f, false)
            .SetEase(Ease.OutCirc)
        )
        .Append(StageManager.Instance.TileObjectDictionary[targetPosition].transform
            .DOMove(new Vector2(targetPosition.x, targetPosition.y) + characterBias - new Vector2(targetPosition.x - _row, targetPosition.y - _col) * 0.1f, 0.1f, false)
            .SetEase(Ease.OutCirc)
        );


        // 2. 위치값을 업데이트 해준다. 
        int row = targetPosition.x;
        int col = targetPosition.y;

        // 5. 캐릭터가 이동하는 시간동안 다른 코루틴이 실행되지 않도록, 0.5초간 대기한다.
        yield return new WaitForSeconds(0.5f);

        _isCharacterCoroutineRunning = false;
    }

    // 캐릭터가 공격하는 애니메이션을 보여준다. 
    public IEnumerator CharacterAttack(Vector2Int targetPosition)
    {
        _isCharacterCoroutineRunning = true;

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Eat);

        Sequence CharacterAttackSequence = DOTween.Sequence();

        CharacterAttackSequence
                .AppendInterval(0.3f)
                .Append(
                    transform.DOMove(new Vector2(Row + (targetPosition.x - Row) * 0.5f, Col + (targetPosition.y - Col) * 0.5f) + characterBias, 0.3f, false)
                )
                .Append(
                    transform.DOMove(new Vector2(Row, Col) + characterBias, 0.3f, false)
                );

        // 캐릭터가 공격하는 애니메이션 재생.
        if (targetPosition.x > Row) transform.GetComponent<SpriteRenderer>().flipX = true;
        GetComponent<Animator>().SetBool("IsAttack", true);

        yield return new WaitForSeconds(0.9f);

        GetComponent<Animator>().SetBool("IsAttack", false);
        if (targetPosition.x > Row) transform.GetComponent<SpriteRenderer>().flipX = false;

        // 공격한 적을 죽인다. 
        EnemyManager.Instance.EnemyDeath(targetPosition, false);

        yield return new WaitForSeconds(0.5f);

        HeartChange(StageManager.Instance.TempTileDictionary[targetPosition].EnemyData.Heart);

        _isCharacterCoroutineRunning = false;
    }

    public IEnumerator CharacterDamaged(int amount)
    {
        _isCharacterCoroutineRunning = true;

        _mainCamera.Shake(0.5f);

        Sequence CharacterDamagedSequence = DOTween.Sequence();

        CharacterDamagedSequence
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[60].GetComponent<RectTransform>()
                    .DOLocalMove(SetPositionSpriteToUI(Row, Col), 0.0f, false)
                )
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[60].GetComponent<UnityEngine.UI.Image>()
                    .DOFade(1.0f, 0.05f)
                )
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[60].GetComponent<UnityEngine.UI.Image>()
                    .DOFade(0.0f, 0.6f)
                );


        yield return new WaitForSeconds(0.65f);

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
    public IEnumerator ExecutionEvent(Dictionary<Vector2Int, GameObject> executionTarget)
    {
        _isCharacterCoroutineRunning = true;

        _isCharacterExecutionCoroutineRunning = true;

        // 1. 처형 효과음 재생.
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Execute);

        // 2. 처형 애니메이션 재생. 
        Sequence ExecutionEffectSequence = DOTween.Sequence();
        Sequence ExecutionWolfSequence = DOTween.Sequence();

        Debug.Log("(Character.cs) Execution Sequence start!");
        ExecutionEffectSequence
            .Append(
                ExecutionManager.Instance.ExecutionEffect
                .DOFade(1.0f, 0.5f))
            .AppendInterval(0.5f)
            .Append(
                ExecutionManager.Instance.ExecutionEffect
                .DOFade(0.0f, 0.5f)
                );

        ExecutionWolfSequence
            .Append(
                ExecutionManager.Instance.ExecutionWolf
                .DOFade(1.0f, 0.5f))
            .AppendInterval(0.5f)
            .Append(
                ExecutionManager.Instance.ExecutionWolf
                .DOFade(0.0f, 0.5f)
                );

        yield return new WaitForSeconds(1.5f);

        _isCharacterExecutionCoroutineRunning = false;

        int count = 0;

        List<Sequence> ExecutionClawSequenceList = new List<Sequence>();

        foreach (var enemy in executionTarget)
        {
            ExecutionClawSequenceList.Add(DOTween.Sequence());

            ExecutionClawSequenceList[count]
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[count].GetComponent<RectTransform>()
                    .DOLocalMove(SetPositionSpriteToUI(enemy.Key.x, enemy.Key.y), 0.0f, false)
                )
                .AppendCallback(() => AudioManager.Instance.PlaySfx(AudioManager.Sfx.Enemy_Attack))
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[count].GetComponent<UnityEngine.UI.Image>()
                    .DOFade(1.0f, 0.05f)
                )
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[count].GetComponent<UnityEngine.UI.Image>()
                    .DOFade(0.0f, 0.6f)
                );

            count++;



            ExecutionClawSequenceList.Add(DOTween.Sequence());

            ExecutionClawSequenceList[count]
                .AppendInterval(0.25f)
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[count].GetComponent<RectTransform>()
                    .DOLocalMove(SetPositionSpriteToUI(enemy.Key.x, enemy.Key.y), 0.0f, false)
                )
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[count].GetComponent<RectTransform>()
                    .DORotate(new Vector3(0.0f, 0.0f, 90.0f), 0.0f)
                )
                .AppendCallback(() => AudioManager.Instance.PlaySfx(AudioManager.Sfx.Enemy_Attack))
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[count].GetComponent<UnityEngine.UI.Image>()
                    .DOFade(1.0f, 0.05f)
                )
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[count].GetComponent<UnityEngine.UI.Image>()
                    .DOFade(0.0f, 0.6f)
                )
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[count].GetComponent<RectTransform>()
                    .DORotate(new Vector3(0.0f, 0.0f, 0.0f), 0.0f)
                );

            count++;



            ExecutionClawSequenceList.Add(DOTween.Sequence());

            ExecutionClawSequenceList[count]
                .AppendInterval(0.5f)
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[count].GetComponent<RectTransform>()
                    .DOLocalMove(SetPositionSpriteToUI(enemy.Key.x, enemy.Key.y), 0.0f, false)
                )
                .AppendCallback(() => AudioManager.Instance.PlaySfx(AudioManager.Sfx.Enemy_Attack))
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[count].GetComponent<UnityEngine.UI.Image>()
                    .DOFade(1.0f, 0.05f)
                )
                .Append(
                    ExecutionManager.Instance.ExecutionClawObjectList[count].GetComponent<UnityEngine.UI.Image>()
                    .DOFade(0.0f, 0.6f)
                );

            count++;
        }


        // 3. 애니메이션을 1.2초간 재생. 
        yield return new WaitForSeconds(1.2f);

        // 4. 처형 이벤트 종료. 캐릭터는 다시 움직일 수 있다. 
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

    public IEnumerator CharacterKick(Vector2Int targetPosition, KeyCode key)
    {
        _isCharacterCoroutineRunning = true;

        // ToDo: Kick 소리 찾으면 변경 필요. 
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Eat);

        // 캐릭터가 공격하는 애니메이션 재생.
        GetComponent<Animator>().SetBool("IsAttack", true);

        yield return new WaitForSeconds(0.9f);

        GetComponent<Animator>().SetBool("IsAttack", false);

        BallManager.Instance.Kick(targetPosition, key);

        yield return new WaitForSeconds(0.5f);

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

        yield return new WaitForSeconds(0.5f);

        _isCharacterResetCoroutineRunning = true;

        GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(1).transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(1).transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(2.0f);

        _isCharacterResetCoroutineRunning = false;

        _isCharacterCoroutineRunning = false;
    }

    public IEnumerator StageClear()
    {
        Debug.Log("StageClear");

        Sequence ClearSequence = DOTween.Sequence();

        yield return new WaitForSeconds(1.0f);

        ClearSequence
            .Append(
                StageManager.Instance.Clear.GetComponent<UnityEngine.UI.Image>()
                .DOFade(1.0f, 1.0f));


        yield return new WaitForSeconds(2.0f);

        int stageId = GameManager.Instance.World * 100 + GameManager.Instance.Stage;
        Debug.Log(stageId);

        // 만약 111이면 World1BossClear 씬으로, 112 이상 116 이하면 World1 씬으로,
        // 만약 212 이상 226 이하면 World2 씬으로, 이외에는 stageId+1 씬으로 이동.
        switch (stageId)
        {
            case 111:
                SceneManager.LoadScene("World1BossClear");
                break;
            case 112:
            case 113:
            case 114:
            case 115:
            case 116:
                SceneManager.LoadScene("World1");
                break;
            case 212:
            case 213:
            case 214:
            case 215:
            case 216:
                SceneManager.LoadScene("World2");
                break;
            default:
                Debug.Log("Load Next Stage. World: " + (stageId + 1) / 100 + " Stage: " + (stageId + 1) % 100);
                LoadingManager.Instance.LoadNextStage((stageId + 1) / 100, (stageId + 1) % 100);
                break;
        }
    }

    #endregion
}
