using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudyText : MonoBehaviour {

    public StudyObject study;
    public Text currentTask;
    public Text taskInfo;
    public Text taskStatus;
    public GameObject panel;

	// Use this for initialization
	void Start () {
		panel.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(study.studyStage == 1) {
            currentTask.text = "Task 1";
            taskInfo.text = "Travel to the next red node";
            taskStatus.text = study.taskNum + " / 10";
        }
        if(study.studyStage == 2) {
            currentTask.text = "Task 2";
            taskInfo.text = "Travel to the next red node";
            taskStatus.text = study.taskNum + " / 10";
        }
        if(study.studyStage == 3) {
            currentTask.text = "Task 3";
            taskInfo.text = "Count circles associated with red node";
            taskStatus.text = study.taskNum + " / 10";
        }
        if(study.studyStage == 4) {
            currentTask.text = "Task 4";
            taskInfo.text = "Count edges associated with red node";
            taskStatus.text = study.taskNum + " / 10";
        }
        if(study.stageI > 4) {
            currentTask.text = "Well done!";
            taskInfo.text = "You completed all the tasks!";
            taskStatus.text = study.taskNum + "";
        }
        Quaternion q = Quaternion.LookRotation(panel.transform.position - Camera.main.transform.position);
        panel.transform.rotation = new Quaternion(q.x*0.5f, q.y, q.z * 0.5f, q.w);
	}

    public void moveUI(Vector3 position) {
        panel.transform.position = position;
        panel.gameObject.SetActive(true);
    }
}
