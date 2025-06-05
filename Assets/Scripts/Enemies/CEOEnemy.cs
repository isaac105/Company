using UnityEngine;

public class CEOEnemy : EnemyCharacter
{
    protected override void InitializeEnemy()
    {
        base.InitializeEnemy();
        
        enemyName = "김사장";
        enemyRank = "사장";
        maxHp = 400f;
        hp = maxHp;
        damage = 50f;
        baseDefenseChance = 0.65f;
        
        Debug.Log($"{enemyName} ({enemyRank}) 초기화 완료: HP {hp}/{maxHp}, 방어율 {baseDefenseChance * 100}%");
    }
} 