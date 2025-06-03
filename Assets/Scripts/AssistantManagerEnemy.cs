using UnityEngine;

public class AssistantManagerEnemy : EnemyCharacter
{
    protected override void InitializeEnemy()
    {
        // 대리 기본 스탯 설정
        enemyName = "김대리";
        enemyRank = "대리";
        maxHp = 80f;
        damage = 12f;
        attackCoefficient = 1.0f;
        defenseCoefficient = 1.0f;
        
        base.InitializeEnemy();
        
        Debug.Log("대리급 적 초기화 완료");
    }
    
    protected override void OnEnemyDefeated()
    {
        Debug.Log("대리가 쓰러졌습니다.");
        Debug.Log("대리 처치 완료! 다음 스테이지로 이동 가능");
    }
}