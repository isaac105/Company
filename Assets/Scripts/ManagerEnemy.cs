using UnityEngine;

public class ManagerEnemy : EnemyCharacter
{
    protected override void InitializeEnemy()
    {
        // 과장 기본 스탯 설정
        enemyName = "박과장";
        enemyRank = "과장";
        maxHp = 120f;
        damage = 18f;
        attackCoefficient = 1.2f;
        defenseCoefficient = 1.1f;
        
        base.InitializeEnemy();
        
        Debug.Log("과장급 적 초기화 완료");
    }
    
    protected override void OnEnemyDefeated()
    {
        Debug.Log("과장이 쓰러졌습니다.");
        Debug.Log("과장 처치 완료! 다음 스테이지로 이동 가능");
    }
}