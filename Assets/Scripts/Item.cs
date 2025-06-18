using UnityEngine;
using System.Collections.Generic;

public class Item : MonoBehaviour
{
    [Header("아이템 기본 속성")]
    [SerializeField] protected float damageCoefficient = 1.0f;
    [SerializeField] protected string itemName = "기본 아이템";
    
    [Header("아이템 특수 효과")]
    [SerializeField] protected List<ItemEffectData> effects = new List<ItemEffectData>();
    
    public float DamageCoefficient
    { 
        get { return damageCoefficient; } 
        set { damageCoefficient = value; } 
    }
    
    public string ItemName
    {
        get { return itemName; }
        set { itemName = value; }
    }
    
    public List<ItemEffectData> Effects
    {
        get { return effects; }
        set { effects = value; }
    }
    
    // virtual Start 메서드 (자식 클래스에서 오버라이드 가능)
    protected virtual void Start()
    {
        Debug.Log($"기본 아이템 초기화: {itemName} - 데미지 계수: {damageCoefficient}");
    }
    
    // 아이템 정보 출력 (효과 포함)
    public virtual string GetItemInfo()
    {
        string info = $"현재 아이템: {itemName} - 데미지: x{damageCoefficient}";
        
        if (effects.Count > 0)
        {
            info += " (";
            for (int i = 0; i < effects.Count; i++)
            {
                info += ItemEffectProcessor.GetEffectDescription(effects[i].effectType, effects[i].effectValue);
                if (i < effects.Count - 1) info += ", ";
            }
            info += ")";
        }
        
        return info;
    }
    
    // 아이템 효과 설명만 반환
    public string GetEffectsDescription()
    {
        if (effects.Count == 0)
        {
            return "효과 없음";
        }

        string description = "";
        for (int i = 0; i < effects.Count; i++)
        {
            description += ItemEffectProcessor.GetEffectDescription(effects[i].effectType, effects[i].effectValue);
            if (i < effects.Count - 1)
            {
                description += ", ";
            }
        }
        return description;
    }
    
    // 아이템 사용 기능
    public virtual void UseItem()
    {
        Debug.Log($"아이템 사용: {GetItemInfo()}");
    }
    
    // 아이템 효과 적용 (CombatManager에서 호출)
    public virtual void ApplyItemEffects(PlayerCharacter player, EnemyCharacter enemy, CombatManager combat)
    {
        foreach (var effect in effects)
        {
            ItemEffectProcessor.ApplyEffect(effect.effectType, effect.effectValue, player, enemy, combat);
        }
    }
    
    // 특정 효과가 있는지 확인
    public bool HasEffect(ItemEffectType effectType)
    {
        return effects.Exists(e => e.effectType == effectType);
    }
    
    // 특정 효과의 값 가져오기
    public float GetEffectValue(ItemEffectType effectType)
    {
        var effect = effects.Find(e => e.effectType == effectType);
        return effect != null ? effect.effectValue : 0f;
    }
    
    // 효과 추가
    protected void AddEffect(ItemEffectType effectType, float effectValue, string description)
    {
        effects.Add(new ItemEffectData(effectType, effectValue, description));
    }
    
    // 마우스 클릭 시 아이템 정보 출력 (UI Button 사용 시에는 사용되지 않음)
    private void OnMouseDown()
    {
        Debug.Log($"아이템 클릭: {GetItemInfo()}");
        UseItem();
    }
}