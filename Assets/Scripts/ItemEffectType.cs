using UnityEngine;
using System.Collections.Generic;

public enum ItemEffectType
{
    None,
    BlockNextDefense,     // 다음 방어 봉인
    DoubleAttack,         // 두 번 공격
    ReduceEnemyDefense,   // 적 방어력 감소
    HighDamage           // 고데미지 (이미 DamageCoefficient로 처리됨)
}

[System.Serializable]
public class ItemEffectData
{
    public ItemEffectType effectType;
    public float effectValue; // 효과 강도 (방어력 감소량, 공격 횟수 등)
    public string effectDescription;
    
    public ItemEffectData(ItemEffectType type, float value, string description)
    {
        effectType = type;
        effectValue = value;
        effectDescription = description;
    }
}

public static class ItemEffectProcessor
{
    public static void ApplyEffect(ItemEffectType effectType, float effectValue, PlayerCharacter player, EnemyCharacter enemy, CombatManager combat)
    {
        switch (effectType)
        {
            case ItemEffectType.BlockNextDefense:
                Debug.Log("적의 다음 방어를 봉인합니다!");
                if (enemy != null)
                {
                    enemy.BlockNextDefense();
                }
                break;
                
            case ItemEffectType.DoubleAttack:
                Debug.Log("연속 공격을 실행합니다! (공격 횟수: " + effectValue + ")");
                // 연속공격은 CombatManager에서 직접 처리됨
                break;
                
            case ItemEffectType.ReduceEnemyDefense:
                Debug.Log("적의 방어 확률을 " + (effectValue * 100) + "% 감소시킵니다!");
                if (enemy != null)
                {
                    enemy.ReduceDefenseChance(effectValue);
                }
                break;
                
            case ItemEffectType.HighDamage:
                Debug.Log("강력한 데미지를 가합니다!");
                // 이미 DamageCoefficient로 처리됨
                break;
                
            default:
                break;
        }
    }
    
    public static string GetEffectDescription(ItemEffectType effectType, float effectValue)
    {
        switch (effectType)
        {
            case ItemEffectType.BlockNextDefense:
                return "방어 봉인";
            case ItemEffectType.DoubleAttack:
                return ((int)effectValue) + "회 공격";
            case ItemEffectType.ReduceEnemyDefense:
                return "방어력 " + (effectValue * 100) + "% 감소";
            case ItemEffectType.HighDamage:
                return "고데미지";
            default:
                return "";
        }
    }
}