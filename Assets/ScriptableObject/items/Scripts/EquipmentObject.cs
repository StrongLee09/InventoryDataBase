using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Object"
                , menuName = "Inventory System/Items/Equipment")]

public class EquipmentObject : ItemObject
{
   
    public void Awake()
    {
        type = ItemType.Equipment;
    }
}
