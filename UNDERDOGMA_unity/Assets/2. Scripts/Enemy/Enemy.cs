using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public abstract class Enemy : MonoBehaviour, IEnemyAttributesSetter, IEnemyPositionSetter
{
    #region Variables
    [SerializeField] GameObject _attackText;
    [SerializeField] GameObject _heartText;

    // 모든 적은 기본적으로 현재 위치를 가진다. 
    private int _row;
    public int Row
    {
        get => _row;
        set => _row = value;
    }

    private int _col;
    public int Col
    {
        get => _col;
        set => _col = value;
    }

    // 적의 생명력. 
    private int _heart;

    public int Heart
    {
        get => _heart;
        set => _heart = value;
    }

    private SpriteDirection _enemyAttackDirection;
    public SpriteDirection EnemyAttackDirection
    {
        get => _enemyAttackDirection;
        set => _enemyAttackDirection = value;
    }

    // 상하좌우 체크를 위한 배열. 
    public Dictionary<SpriteDirection, Vector2Int> directionOffsetDictionary = new Dictionary<SpriteDirection, Vector2Int>
    {
        { SpriteDirection.Up, new Vector2Int(0, 1) },
        { SpriteDirection.Down, new Vector2Int(0, -1) },
        { SpriteDirection.Left, new Vector2Int(-1, 0) },
        { SpriteDirection.Right, new Vector2Int(1, 0) }
    };

    #endregion

    public void SetAttributes(EnemyData enemyData)
    {
        _heart = enemyData.Heart;
        _enemyAttackDirection = enemyData.AttackDirection;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        _heartText.GetComponent<Text>().SetText(_heart);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual IEnumerator EnemyAttackAnimation(Vector2Int targetPosition)
    {
        // 캐릭터를 공격하는 애니메이션 진행. 
        gameObject.GetComponent<Animator>().SetBool("IsAttack", true);

        Sequence CharacterAttackSequence = DOTween.Sequence();

        CharacterAttackSequence
                .Append(
                    transform.DOMove(new Vector2(Row + (targetPosition.x - Row) * 0.5f, Col + (targetPosition.y - Col) * 0.5f) + new Vector2(-0.06f, 0.3f), 0.3f, false)
                )
                .Append(
                    transform.DOMove(new Vector2(Row, Col) + new Vector2(-0.07f, 0.35f), 0.3f, false)
                );

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Enemy_Attack);

        yield return new WaitForSeconds(0.65f);

        gameObject.GetComponent<Animator>().SetBool("IsAttack", false);
    }

    // 첫번째 인자는 해당 위치에 있는 적을 찾기 위해서, 두번째 인자는 적이 처형될때에 잠시 대기하기 위해서. 
    public virtual IEnumerator EnemyDeath(Vector2Int targetPosition, bool deathByExecution)
    {
        // 나중에 죽는 애니메이션 추가해야 함. 
        // gameObject.GetComponent<SpriteRenderer>().sprite = _deadDog;

        if (deathByExecution == true)
        {
            yield return new WaitForSeconds(2.2f);
        }

        StageManager.Instance.TempTileDictionary[targetPosition].EnemyData.IsAlive = false;

        gameObject.GetComponent<Animator>().SetBool("IsDied", true);

        gameObject.GetComponent<SpriteRenderer>().DOFade(0, 0.5f);

        yield return new WaitForSeconds(0.5f);

        gameObject.GetComponent<Animator>().SetBool("IsDied", false);

        Debug.Log("(Enemy.cs) 적이 죽었습니다. 데이터 변경하기!");

        Destroy(StageManager.Instance.GameObjectDictionary[targetPosition]);
        StageManager.Instance.GameObjectDictionary.Remove(targetPosition);

        yield return null;
    }
}
