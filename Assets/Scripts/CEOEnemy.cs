using UnityEngine;

public class CEOEnemy : EnemyCharacter
{
    protected override void InitializeEnemy()
    {
        // 사장 기본 스탯 설정
        enemyName = "김사장";
        enemyRank = "사장";
        maxHp = 400f;
        damage = 50f;
        attackCoefficient = 2.0f;
        defenseCoefficient = 1.8f;
        baseDefenseChance = 0.65f; // 사장: 65% 방어 확률 (최고)
        
        base.InitializeEnemy();
        
        Debug.Log("사장급 적 초기화 완료");
    }
    
    protected override void OnEnemyDefeated()
    {
        Debug.Log("사장이 쓰러졌습니다.");
        Debug.Log("사장 처치 완료! 게임 클리어!");
    }
}