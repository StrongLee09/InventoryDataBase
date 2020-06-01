using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using TMPro;
using System.Reflection;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject //ISerializationCallbackReceiver
{
    public string savePath;
    public ItemDatabaseObject database;
    public Inventory Container;

    //public InventoryObject(ItemDatabaseObject _database , Inventory _Container)
    //{
    //    this.database = _database;
    //    this.Container = _Container;
    //}

    //    private void OnEnable()
    //    {
    //#if UNITY_EDITOR
    //        database = (ItemDatabaseObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Database.asset", typeof(ItemDatabaseObject));
    //#else
    //        database = Resources.Load<ItemDatabaseObject>("Database");
    //#endif
    //    }


    public void AddItem(Item _item, int _amount)
    {
        if (Container.Items.Count != 0)
        {
            for (int i = 0; i < Container.Items.Count; i++)
            {
                if (Container.Items[i].item.Id == _item.Id)
                {
                    for (int j = 0; j < Container.Items[i].item.buffs.Length; j++)
                    {
                        for (int k = 0; j < _item.buffs.Length; k++)
                        {
                            // Container에 있는 아이템의 아이디와 버프의 속성이 추가될 아이템과 같으면 ...
                            if (Container.Items[i].item.buffs[j].attribute == _item.buffs[k].attribute)
                            {
                                // Container에 있는 아이템의 아이디와 버프의 속성과 버프의 값이 추가될 아이템과 같으면 ...
                                // Container의 Amount만 증가시킨다.
                                if (Container.Items[i].item.buffs[j].value == _item.buffs[k].value)
                                {
                                    Debug.Log("중복된 아이템입니다. amount 증가 . . !");
                                    Container.Items[i].AddAmount(_amount);
                                    return;
                                }
                                // Container에 있는 아이템의 아이디와 버프의 속성은 같지만 버프의 값이 추가될 아이템과 다르면...
                                // Container에 아이템을 추가하고 for문 종료...
                                else
                                {
                                    Debug.Log("아이디가 같음 아이디 : " + _item.Id + ":" + Container.Items[i].item.Id);
                                    Debug.Log("버프 속성이 같음 속성 : " + _item.buffs[k].attribute + " : " + Container.Items[i].item.buffs[j].attribute);
                                    Debug.Log("속성값이 다름 속성값 : " + _item.buffs[k].value + " : " + Container.Items[i].item.buffs[j].value);
                                    Container.Items.Add(new InventorySlot(_item.Id, _item, _amount));
                                    return;
                                }
                            }
                            // Container에 있는 아이템의 아이디와 버프의 속성이 추가될 아이템과 다르면 . . .
                            else
                            {
                                Debug.Log("아이디가 같음 아이디 : " + _item.Id + ":" + Container.Items[i].item.Id);
                                Debug.Log("버프 속성이 다름 속성 : " + _item.buffs[k].attribute + " : " + Container.Items[i].item.buffs[j].attribute);
                                Container.Items.Add(new InventorySlot(_item.Id, _item, _amount));
                                return;
                            }
                        }
                    }
                }
                // Container에 없는 즉, ID가 다른 아이템이 새로 추가될때 . . . 
                if (i == Container.Items.Count - 1)
                {
                    Debug.Log("인벤토리에 없는 신규 아이템 : " + _item.Id + ":" + Container.Items[i].item.Id);
                    Container.Items.Add(new InventorySlot(_item.Id, _item, _amount));
                    return;
                }
            }
        }
        else
        {
            Container.Items.Add(new InventorySlot(_item.Id, _item, _amount));
        }
    }

    [ContextMenu("Save")]
    public void Save()
    {
        //version 3
        Debug.Log("서버에 저장중 !");
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unitybossraidgame.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        //InventoryObject inventoryObject = new InventoryObject(database,Container);
        string saveData = JsonUtility.ToJson(this, true);
        reference.Child("Items").SetRawJsonValueAsync(saveData);
        Debug.Log("서버에 저장하기 성공 !");

        //version 1
        //string saveData = JsonUtility.ToJson(this, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file 
        //    = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //bf.Serialize(file, saveData);
        //file.Close();        


        //version 2
        //IFormatter formatter = new BinaryFormatter();
        //Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath)
        //    , FileMode.Create, FileAccess.Write);
        //Debug.Log(string.Concat(Application.persistentDataPath, savePath));
        //formatter.Serialize(stream, Container);
        //stream.Close();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        //version 3 
        //FirebaseDatabase reference = FirebaseDatabase.DefaultInstance;
        FirebaseDatabase.DefaultInstance.GetReferenceFromUrl("https://unitybossraidgame.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("로드");
        reference.GetValueAsync().ContinueWith(task =>
        {
            Debug.Log("로드중");
            
            if (task.IsFaulted)
            {
                //Handle the error....
                Debug.Log("Handle the error");
            }
            else if (task.IsCompleted)
            {
                
                Debug.Log("task 성공 ");
                Debug.Log("task Status : "+ task.Status);
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot);
                if (snapshot.HasChildren)
                {
                    Debug.Log("데이터 있음");
                    string saveData = snapshot.Child("Items").GetRawJsonValue();
                    Debug.Log("데이터 전환 성공");
                    Debug.Log(saveData);
                    JsonUtility.FromJsonOverwrite(saveData, this);
                    Debug.Log("데이터 Overwirte 성공");
                }
                else
                {
                    Debug.Log("데이터 없음");
                }

                // 데이터베이스에 키값이 여러개있을경우... 
                //foreach (DataSnapshot item in snapshot.Children)
                //{
                //    JsonUtility.FromJsonOverwrite(item.Value.ToString(), this);
                //    Debug.Log("성공");
                //}
            }
            Debug.Log("로드완료");
        });


        //version 1
        //if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        //{
        //    binaryformatter bf = new binaryformatter();
        //    filestream file
        //        = file.open(string.concat(application.persistentdatapath, savepath)
        //        , filemode.open);
        //    jsonutility.fromjsonoverwrite(bf.deserialize(file).tostring(), this);
        //    file.close();
        //}
        //version 2
        //IFormatter formatter = new BinaryFormatter();
        //Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath),
        //    FileMode.Open, FileAccess.Read);
        //Container = (Inventory)formatter.Deserialize(stream);
        //stream.Close();
    }

    private void HandleChanged(object sender, ValueChangedEventArgs args)
    {
        if(args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        Container = new Inventory();
    }

    //public void OnBeforeSerialize()
    //{
    //}

    //public void OnAfterDeserialize()
    //{
    //    for (int i = 0; i < Container.Items.Count; i++)
    //    {
    //        Container.Items[i].item = database.GetItem[Container.Items[i].item.];
    //    }
    //}
}
[System.Serializable]
public class Inventory
{
     public List<InventorySlot> Items = new List<InventorySlot>();
    //public InventorySlot[] Items = new InventorySlot[24];
   
}



[System.Serializable]
public class InventorySlot
{
    public int ID;
    public Item item;
    public int amount;
    public InventorySlot(int _id, Item _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }

}


