using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class Item : ScriptableObject
{
    public string Name;
    public Sprite image;
    public enum ItemType{Health, Shield}
    public ItemType type;
    public int RestoreAmount;
}
