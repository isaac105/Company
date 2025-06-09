using UnityEngine;

public class DirectorEnemy : EnemyCharacter
{
    protected override void InitializeEnemy()
    {
        base.InitializeEnemy();
        
        enemyName = "정이사";
        enemyRank = "이사";
        maxHp = 250f;
        hp = maxHp;
        damage = 35f;
        baseDefenseChance = 0.55f;
        
        Debug.Log($"{enemyName} ({enemyRank}) 초기화 완료: HP {hp}/{maxHp}, 방어율 {baseDefenseChance * 100}%");
    }
} 