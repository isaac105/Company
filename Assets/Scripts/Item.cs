using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("아이템 기본 속성")]
    [SerializeField] protected float damageCoefficient = 1.0f;
    
    public float DamageCoefficient
    { 
        get { return damageCoefficient; } 
        set { damageCoefficient = value; } 
    }
    
    // virtual Start 메서드 (자식 클래스에서 오버라이드 가능)
    protected virtual void Start()
    {
        Debug.Log($"기본 아이템 초기화: {gameObject.name} - 데미지 계수: {damageCoefficient}");
    }
    
    // 아이템 정보 출력
    public virtual string GetItemInfo()
    {
        return $"데미지 계수: {damageCoefficient}";
    }
    
    // 아이템 사용 기능
    public virtual void UseItem()
    {
        Debug.Log($"아이템 사용: {gameObject.name} - {GetItemInfo()}");
    }
    
    // 마우스 클릭 시 아이템 정보 출력
    private void OnMouseDown()
    {
        Debug.Log($"아이템 클릭: {GetItemInfo()}");
        UseItem();
    }
}