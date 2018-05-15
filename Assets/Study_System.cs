using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Study_System : MonoBehaviour {

    public Text PID;
    public Dropdown NavigationTasks;
    public Dropdown Tasks;
    public Dropdown DataOrder;
    public Dropdown HandOrientation;
    public string Navigation;
    public string Task;
    public string Data;
    public string Hand;
    public string[] NavArray;
    public string[] TaskArray;
    public int[] TaskArrayInt;
    public string[] DataArray;
    public int[] DataArrayInt;
    public string[] HandOrientArray;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Navigation = NavigationTasks.options[NavigationTasks.value].text;
        Task = Tasks.options[Tasks.value].text;
        Data = DataOrder.options[DataOrder.value].text;
        Hand = HandOrientation.options[HandOrientation.value].text;
        NavArray = Navigation.Split(',');
        TaskArray = Task.Split(',');
        TaskArrayInt = Array.ConvertAll(TaskArray, int.Parse); 
        DataArray = Data.Split(',');
        DataArrayInt = Array.ConvertAll(DataArray, int.Parse);
        HandOrientArray = Hand.Split(',');
    }

    // Load the Study
    public void LoadStudy()
    {
        SceneManager.LoadScene(1);
    }
}
