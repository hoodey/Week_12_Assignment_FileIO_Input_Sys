using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PauseHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputManager.controls.UI.Pause.performed += Pause;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause(InputAction.CallbackContext ctx)
    {
        bool isLoaded = SceneManager.GetSceneByName("PauseMenu").isLoaded;
        if (!isLoaded)
        {
            SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.UnloadSceneAsync("PauseMenu");
        }
    }


}
