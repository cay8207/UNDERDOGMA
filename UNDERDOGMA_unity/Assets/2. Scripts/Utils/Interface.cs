using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public interface IEnemyAttributesSetter
{
    void SetAttributes(EnemyData enemyData);
}

public interface IEnemyPositionSetter
{
    int Row { get; set; }
    int Col { get; set; }
}