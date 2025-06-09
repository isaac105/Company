using UnityEngine;

public class AssistantManagerEnemy : EnemyCharacter
{
    protected override void InitializeEnemy()
    {
        base.InitializeEnemy();
        
        enemyName = "김대리";
        enemyRank = "대리";
        maxHp = 80f;
        hp = maxHp;
        damage = 12f;
        baseDefenseChance = 0.25f;
        
        Debug.Log($"{enemyName} ({enemyRank}) 초기화 완료: HP {hp}/{maxHp}, 방어율 {baseDefenseChance * 100}%");
    }
} 