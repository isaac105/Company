using UnityEngine;

public class DepartmentHeadEnemy : EnemyCharacter
{
    protected override void InitializeEnemy()
    {
        // 부장 기본 스탯 설정
        enemyName = "최부장";
        enemyRank = "부장";
        maxHp = 180f;
        damage = 25f;
        attackCoefficient = 1.5f;
        defenseCoefficient = 1.3f;
        baseDefenseChance = 0.45f; // 부장: 45% 방어 확률 (높음)
        
        base.InitializeEnemy();
        
        Debug.Log("부장급 적 초기화 완료");
    }
    
    protected override void OnEnemyDefeated()
    {
        Debug.Log("부장이 쓰러졌습니다.");
        Debug.Log("부장 처치 완료! 다음 스테이지로 이동 가능");
    }
}