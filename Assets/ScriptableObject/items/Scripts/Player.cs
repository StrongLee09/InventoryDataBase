using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;

    private void OnTriggerEnter(Collider other)
    {
        //var C# javascript
        //var 
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            inventory.AddItem(new Item(item.item), 1);
            Destroy(other.gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inventory.Load();
        }
    }
    public void Save()
    {
        inventory.Save();
    }
    public void Load()
    {
        inventory.Load();
    }

    public void Clear()
    {
        inventory.Container.Items.Clear();
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Items.Clear();
    }
}
