using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using Unity.VisualScripting.FullSerializer;

public class SaveHandler : MonoBehaviour
{
    string path;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        path = Application.dataPath + "/testSave.json";
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {

        }

        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            Load();
        }

    }

    public void Save()
    {
        SaveData sd = new SaveData();
        sd.playerPosition = player.transform.position;
        sd.health = player.GetComponent<Damageable>().GetHP();
        sd.ammo = player.GetComponent<FPSController>().currentGun.GetAmmo();
        sd.currentGun = player.GetComponent<FPSController>().currentGun;
        sd.view = player.transform.rotation;
        
        string jsonText = JsonUtility.ToJson(sd);
        Debug.Log(jsonText);
        File.WriteAllText(path, jsonText);
    }

    public void Load()
    {
        if (!File.Exists(path))
        {
            Debug.Log("There is no save file to load. Save first.");
            return;
        }
        string saveText = File.ReadAllText(path);
        Debug.Log(saveText);
        SaveData myData = JsonUtility.FromJson<SaveData>(saveText);
        CharacterController cc = player.GetComponent<CharacterController>();

        cc.enabled = false;
        player.transform.position = myData.playerPosition;
        player.transform.rotation = myData.view;
        cc.enabled = true;
        player.GetComponent<Damageable>().SetHP(myData.health);
        player.GetComponent<FPSController>().EquipGun(myData.currentGun);
        player.GetComponent<FPSController>().currentGun.SetAmmo(myData.ammo);
    }
}

public class SaveData
{
    public Vector3 playerPosition;
    public float health;
    public int ammo;
    public Gun currentGun;
    public Quaternion view;
}
