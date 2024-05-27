using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 타입과 데이터들을 저장하는 클래스. 
public class TileObject
{
    private TileType _type;
    public TileType Type
    {
        get => _type;
        set => _type = value;
    }

    private EnemyData _enemyData;
    public EnemyData EnemyData
    {
        get => _enemyData;
        set => _enemyData = value;
    }
    private MeatData _meatData;
    public MeatData MeatData
    {
        get => _meatData;
        set => _meatData = value;
    }

    // 1. Type이 Wall, Ball, Empty 등인 경우 생성자. Type만 저장해주면 된다. 
    public TileObject(TileType type)
    {
        this._type = type;
    }

    // 2. Type이 Enemy인 경우 생성자. 타일에 대한 정보와 적에 대한 정보를 저장해준다.
    public TileObject(TileType type, EnemyData enemyData)
    {
        this._type = type;
        this._enemyData = enemyData;
    }

    // 3. Type이 Meat인 경우 생성자. 타일에 대한 정보와 고기에 대한 정보를 저장해준다.
    public TileObject(TileType type, MeatData meatData)
    {
        this._type = type;
        this._meatData = meatData;
    }

    // 4. 복사 생성자. 
    public TileObject(TileObject other)
    {
        Type = other.Type;
        EnemyData = other.EnemyData;
        MeatData = other.MeatData;
    }
}

// 적의 경우 어떤 적인지, 살아있는지, 공격력, 체력, 공격방향을 저장한다.
public class EnemyData
{
    private EnemyType enemyType;
    public EnemyType EnemyType
    {
        get => enemyType;
        set => enemyType = value;
    }
    private bool isAlive;
    public bool IsAlive
    {
        get => isAlive;
        set => isAlive = value;
    }
    private int heart;
    public int Heart
    {
        get => heart;
        set => heart = value;
    }
    private SpriteDirection attackDirection;
    public SpriteDirection AttackDirection
    {
        get => attackDirection;
        set => attackDirection = value;
    }

    public EnemyData(EnemyType enemyType, bool isAlive, int heart, SpriteDirection attackDirection)
    {
        this.enemyType = enemyType;
        this.isAlive = isAlive;
        this.heart = heart;
        this.attackDirection = attackDirection;
    }
}

// 고기의 경우 체력을 얼마나 회복시켜주는지, 현재 맵에 존재하는지를 저장한다. 
public class MeatData
{
    private int amount;
    public int Amount
    {
        get => amount;
        set => amount = value;
    }
    private bool isExist;
    public bool IsExist
    {
        get => isExist;
        set => isExist = value;
    }

    public MeatData(int amount, bool isExist)
    {
        this.amount = amount;
        this.isExist = isExist;
    }
}

// enum들을 저장. 
// Tile의 경우 Wall, Empty, Enemy, Meat이 있다.
public enum TileType
{
    Wall,
    Empty,
    Enemy,
    Meat,
    Ball,
    Invalid
}

public enum TileDirection
{
    Up,
    Down,
    Left,
    Right,
    None
}

// 적의 경우 NormalEnemy, ChaserEnemy, MiniBoss가 있다. 
public enum EnemyType
{
    NormalEnemy,
    Chaser,
    MiniBoss,
    StrongAttack,
    AllDirectionsAttack,
    Angel,
    KickBoss
}

public enum SpriteDirection
{
    None,
    Up,
    Down,
    Left,
    Right,
}