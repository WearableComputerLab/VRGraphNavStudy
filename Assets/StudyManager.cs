using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Increments tasks in a random order, i.e. {CF,SF,FF,PF}
public class StudyManager : MonoBehaviour {

	public int nextScene = 0;
	public int currentIndex = 0;
	public int[] scenes;


	// Use this for initialization
	private void Start () {
		scenes = new int[]{1,2,3,4};
		DontDestroyOnLoad (gameObject);
		Randomizer.Randomize(scenes);
		nextScene = scenes [currentIndex];
		SceneManager.LoadScene(nextScene);
	}

	private void Update () {
		// For PathFind and FeatureFind
		if(Input.GetKeyDown(KeyCode.N)){
			Recorder r = FindObjectOfType<Recorder>();
			r.outputToCSV();
			incrementScene();
		}
	}

	public void incrementScene(){
		if (currentIndex >= scenes.Length) {
			Application.Quit();
		}
		currentIndex++;
		nextScene = scenes [currentIndex];
		SceneManager.LoadScene(nextScene);
	}
				
}
#endregion

#region Randomizer function
public class Randomizer
{
	public static void Randomize<T>(T[] items)
	{
		System.Random rand = new System.Random();

		// For each spot in the array, pick
		// a random item to swap into that spot.
		for (int i = 0; i < items.Length - 1; i++)
		{
			int j = rand.Next(i, items.Length);
			T temp = items[i];
			items[i] = items[j];
			items[j] = temp;
		}
	}
}
#endregion
