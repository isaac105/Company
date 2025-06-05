using UnityEngine;

public class DepartmentHeadEnemy : EnemyCharacter
{
    protected override void InitializeEnemy()
    {
        base.InitializeEnemy();
        
        enemyName = "최부장";
        enemyRank = "부장";
        maxHp = 180f;
        hp = maxHp;
        damage = 25f;
        baseDefenseChance = 0.45f;
        
        Debug.Log($"{enemyName} ({enemyRank}) 초기화 완료: HP {hp}/{maxHp}, 방어율 {baseDefenseChance * 100}%");
    }
} 