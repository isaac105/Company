using UnityEngine;

public class DirectorEnemy : EnemyCharacter
{
    protected override void InitializeEnemy()
    {
        // 이사 기본 스탯 설정
        enemyName = "정이사";
        enemyRank = "이사";
        maxHp = 250f;
        damage = 35f;
        attackCoefficient = 1.8f;
        defenseCoefficient = 1.5f;
        
        base.InitializeEnemy();
        
        Debug.Log("이사급 적 초기화 완료");
    }
    
    protected override void OnEnemyDefeated()
    {
        Debug.Log("이사가 쓰러졌습니다.");
        Debug.Log("이사 처치 완료! 다음 스테이지로 이동 가능");
    }
}