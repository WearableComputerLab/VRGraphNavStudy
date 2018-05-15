using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StudyAssign : MonoBehaviour {

    // System Reference
    public Study_System sys;
    private StudyObject s_obj;
    public string ParticipantID;
    public string[] NavArray;
    public int[] TaskArray;
    public int[] DataArray;
    public string[] HandOrientArray;
    public int stringIndex = 0;
    public string currentNavigation;
    // Indexes
    public int NavIndex = 0;
    public int TaskIndex = 0;
    public int DataIndex = 0;
    public int initialLoad = 0;

    // Use this for initialization
    void Start () {

        // Don't destroy this object on load
        DontDestroyOnLoad(gameObject);
        currentNavigation = NavArray[stringIndex];
        s_obj = FindObjectOfType<StudyObject>();

    }
	
	// Update is called once per frame
	void Update () {
        if (NavArray.Length > 0)
        {
            currentNavigation = NavArray[stringIndex];
        }
        
        if (s_obj == null && initialLoad > 1)
        {
            try
            {
                s_obj = FindObjectOfType<StudyObject>();
            } catch (Exception e) {
                print("can't find s_obj right now");
            }
        }
        if (sys != null)
        {
            this.ParticipantID = sys.PID.text;
            this.NavArray = sys.NavArray;
            this.TaskArray = sys.TaskArrayInt;
            this.DataArray = sys.DataArrayInt;
            this.HandOrientArray = sys.HandOrientArray;
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            print("Level Reload");
            SceneManager.LoadScene(1);
            if (initialLoad > 1)
            {
                s_obj.createCSV();
                stringIndex++;
                if (stringIndex > -1)
                {
                    currentNavigation = NavArray[stringIndex];
                }
            }
        }
	}

    private void OnLevelWasLoaded(int level)
    {
        initialLoad++;
    }

}
