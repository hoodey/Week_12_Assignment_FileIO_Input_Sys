using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using Unity.VisualScripting.FullSerializer;

public class SaveHandler : MonoBehaviour
{
    public string path;

    // Start is called before the first frame update
    void Start()
    {
        path = Application.dataPath + "/testSave.json";
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {

        }

        if (Keyboard.current.lKey.wasPressedThisFrame)
        {

        }

    }

    void Save()
    {
        SaveData sd = new SaveData();
        sd.playerPosition = GameObject.Find("Player").transform.position;
        
        string jsonText = JsonUtility.ToJson(sd);
        File.WriteAllText(path, jsonText);
    }

    void Load()
    {
        if (!File.Exists(path))
        {
            Debug.Log("There is no save file to load. Save first.");
            return;
        }
        string saveText = File.ReadAllText(path);
        SaveData myData = JsonUtility.FromJson<SaveData>(saveText);
        GameObject.Find("Player").transform.position = myData.playerPosition;
    }
}

public class SaveData
{
    public Vector3 playerPosition;
}
