using UnityEngine;

/// Описание продукта/предмета: префаб, иконка, название.
/// Create -> Kitchen -> Kitchen Object
[CreateAssetMenu(menuName = "Kitchen/Kitchen Object")]
public class KitchenObjectSO : ScriptableObject
{
    public KitchenObject prefab;
    public Sprite sprite;
    public string objectName;
}
