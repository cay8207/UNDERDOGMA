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

    // 적의 공격력과 생명력. 
    private int _attack;
    public int Attack
    {
        get => _attack;
        set => _attack = value;
    }

    private int _heart;

    public int Heart
    {
        get => _heart;
        set => _heart = value;
    }

    private AttackDirection _enemyAttackDirection;
    public AttackDirection EnemyAttackDirection
    {
        get => _enemyAttackDirection;
        set => _enemyAttackDirection = value;
    }

    // 상하좌우 체크를 위한 배열. 
    public Dictionary<AttackDirection, Vector2Int> directionOffsetDictionary = new Dictionary<AttackDirection, Vector2Int>
    {
        { AttackDirection.Up, new Vector2Int(0, 1) },
        { AttackDirection.Down, new Vector2Int(0, -1) },
        { AttackDirection.Left, new Vector2Int(-1, 0) },
        { AttackDirection.Right, new Vector2Int(1, 0) }
    };

    #endregion

    public void SetAttributes(EnemyData enemyData)
    {
        _attack = enemyData.Attack;
        _heart = enemyData.Heart;
        _enemyAttackDirection = enemyData.AttackDirection;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        _attackText.GetComponent<Text>().SetText(_attack);
        _heartText.GetComponent<Text>().SetText(_heart);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual IEnumerator EnemyAttackAnimation()
    {

        // 캐릭터를 공격하는 애니메이션 진행. 
        gameObject.GetComponent<Animator>().SetBool("IsAttack", true);

        yield return new WaitForSeconds(1.0f);

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Enemy_Attack);

        gameObject.GetComponent<Animator>().SetBool("IsAttack", false);
    }

    // 첫번째 인자는 해당 위치에 있는 적을 찾기 위해서, 두번째 인자는 적이 처형될때에 잠시 대기하기 위해서. 
    public IEnumerator EnemyDeath(Vector2Int targetPosition, bool deathByExecution)
    {
        // 나중에 죽는 애니메이션 추가해야 함. 
        // gameObject.GetComponent<SpriteRenderer>().sprite = _deadDog;
        StageManager.Instance.TempTileDictionary[targetPosition].EnemyData.IsAlive = false;

        if (deathByExecution == true)
        {
            yield return new WaitForSeconds(2.5f);
        }

        gameObject.GetComponent<Animator>().SetBool("IsDied", true);

        gameObject.GetComponent<SpriteRenderer>().DOFade(0, 0.5f);
        gameObject.transform.GetChild(5).GetComponent<SpriteRenderer>().DOFade(0, 0.5f);

        yield return new WaitForSeconds(0.5f);

        gameObject.GetComponent<Animator>().SetBool("IsDied", false);

        Debug.Log("(Enemy.cs) 적이 죽었습니다. 데이터 변경하기!");

        Destroy(StageManager.Instance.GameObjectDictionary[targetPosition]);
        StageManager.Instance.GameObjectDictionary.Remove(targetPosition);

        yield return null;
    }
}
