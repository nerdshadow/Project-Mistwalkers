using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "SO/Inventory")]
public class PlayerInventory : ScriptableObject
{
    public int baseSize = 25;
    public int currentSize = 25;
    [SerializeField]
    public List<ScriptableObject> items;
}
