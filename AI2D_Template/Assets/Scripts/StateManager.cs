/*
StateManager

Manages the overall 
state of the application
across all scenes.

Copyright John M. Quick
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour {

    /*
    This script implements the singleton 
    design pattern, meaning that only one 
    instance of it can ever exist. Other 
    scripts can easily reference this 
    script by calling StateManager.Instance.
	
	For example, any script can tell the 
	StateManager to switch to the Game scene 
	by issuing the command:
	StateManager.Instance.SwitchSceneTo("Game");
    */

    //singleton instance
    private static StateManager _Instance;

    //singleton accessor
    public static StateManager Instance {

        //create instance via getter
        get {

            //check for existing instance
            //if no instance
            if (_Instance == null) {

                //create game object
                GameObject StateManagerObj = new GameObject();
                StateManagerObj.name = "StateManager";

                //create instance
                _Instance = StateManagerObj.AddComponent<StateManager>();
            }

            //return the instance
            return _Instance;
        }
    }

    //awake
    void Awake() {

        //prevent this script from being destroyed
        DontDestroyOnLoad(this);
    }

    //switch scene by name
    public void SwitchSceneTo(string theScene) {

        //load scene
        SceneManager.LoadScene(theScene);
    }

} //end class