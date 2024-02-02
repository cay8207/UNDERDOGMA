using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Enemy : MonoBehaviour
{
    #region Variables
    [SerializeField] GameObject _attackText;
    [SerializeField] GameObject _heartText;

    private MainCamera _mainCamera;

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

    // 상하좌우 체크를 위한 배열. 
    public Vector2Int[] directionOffsets = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(1, 0)    // Right
        };

    #endregion

    // Start is called before the first frame update
    public virtual void Start()
    {
        _attackText.GetComponent<Text>().SetText(_attack);
        _heartText.GetComponent<Text>().SetText(_heart);

        _mainCamera = Camera.main.GetComponent<MainCamera>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual IEnumerator EnemyAction(int playerRow, int playerCol)
    {

        // 캐릭터를 공격하는 애니메이션 진행. 
        gameObject.GetComponent<Animator>().SetBool("IsAttack", true);

        yield return new WaitForSeconds(1.0f);

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Enemy_Attack);

        _mainCamera.Shake(0.5f);

        gameObject.GetComponent<Animator>().SetBool("IsAttack", false);
    }

    public IEnumerator EnemyDeath(Vector2Int targetPosition)
    {
        // 나중에 죽는 애니메이션 추가해야 함. 
        // gameObject.GetComponent<SpriteRenderer>().sprite = _deadDog;

        gameObject.GetComponent<Animator>().SetBool("IsDied", true);

        StartCoroutine(FadeOut(gameObject.GetComponent<SpriteRenderer>(), 1.0f));
        StartCoroutine(FadeOut(gameObject.transform.GetChild(5).GetComponent<SpriteRenderer>(), 1.0f));

        yield return new WaitForSeconds(1.0f);

        gameObject.GetComponent<Animator>().SetBool("IsDied", false);

        Destroy(EnemyManager.Instance.EnemyDictionary[targetPosition]);
        StageManager.Instance.TempTileDictionary[targetPosition][2] = 0;
        EnemyManager.Instance.EnemyDictionary.Remove(targetPosition);

        StageManager.Instance.StageClearCheck();

        yield return null;
    }

    public IEnumerator FadeIn(SpriteRenderer spriteRenderer, float time)
    {
        Color color = spriteRenderer.color;
        while (color.a < 1f)
        {
            color.a += Time.deltaTime / time;
            spriteRenderer.color = color;
            yield return null;
        }
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
