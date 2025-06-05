using UnityEngine;

public class ManagerEnemy : EnemyCharacter
{
    protected override void InitializeEnemy()
    {
        base.InitializeEnemy();
        
        enemyName = "박과장";
        enemyRank = "과장";
        maxHp = 120f;
        hp = maxHp;
        damage = 18f;
        baseDefenseChance = 0.35f;
        
        Debug.Log($"{enemyName} ({enemyRank}) 초기화 완료: HP {hp}/{maxHp}, 방어율 {baseDefenseChance * 100}%");
    }
} 