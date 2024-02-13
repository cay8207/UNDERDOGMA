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
    private int _round;
    public int Round
    {
        get => _round;
        set => _round = value;
    }
    private int _pattern;
    public int Pattern
    {
        get => _pattern;
        set => _pattern = value;
    }
    private TileDirection _tileDirection;
    public TileDirection TileDirection
    {
        get => _tileDirection;
        set => _tileDirection = value;
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

    // 1. Type이 Wall인 경우 생성자. Type만 저장해주면 된다. 
    public TileObject(TileType type)
    {
        this._type = type;
        switch (type)
        {
            case TileType.Wall:
                break;
            case TileType.Empty:
                break;
        }
    }

    // 2. Type이 Empty인 경우 생성자. 타일에 대한 정보를 저장해준다. 
    public TileObject(TileType type, int round, int pattern, TileDirection tileDirection)
    {
        this._type = type;
        this._round = round;
        this._pattern = pattern;
        this._tileDirection = tileDirection;
        switch (type)
        {
            case TileType.Wall:
                break;
            case TileType.Empty:
                break;
        }
    }

    // 3. Type이 Enemy인 경우 생성자. 타일에 대한 정보와 적에 대한 정보를 저장해준다.
    public TileObject(TileType type, int round, int pattern, TileDirection tileDirection, EnemyData enemyData)
    {
        this._type = type;
        this._round = round;
        this._pattern = pattern;
        this._tileDirection = tileDirection;
        this._enemyData = enemyData;
    }

    // 4. Type이 Meat인 경우 생성자. 타일에 대한 정보와 고기에 대한 정보를 저장해준다.
    public TileObject(TileType type, int round, int pattern, TileDirection tileDirection, MeatData meatData)
    {
        this._type = type;
        this._round = round;
        this._pattern = pattern;
        this._tileDirection = tileDirection;
        this._meatData = meatData;
    }

    // 5. 복사 생성자. 
    public TileObject(TileObject other)
    {
        Type = other.Type;
        Round = other.Round;
        Pattern = other.Pattern;
        TileDirection = other.TileDirection;
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
    private int attack;
    public int Attack
    {
        get => attack;
        set => attack = value;
    }
    private int heart;
    public int Heart
    {
        get => heart;
        set => heart = value;
    }
    private AttackDirection attackDirection;
    public AttackDirection AttackDirection
    {
        get => attackDirection;
        set => attackDirection = value;
    }

    public EnemyData(EnemyType enemyType, bool isAlive, int attack, int heart, AttackDirection attackDirection)
    {
        this.enemyType = enemyType;
        this.isAlive = isAlive;
        this.attack = attack;
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
    ChaserEnemy,
    MiniBoss,
}

public enum AttackDirection
{
    Up,
    Down,
    Left,
    Right,
}