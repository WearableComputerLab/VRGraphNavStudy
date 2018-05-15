using UnityEngine;
using System.Collections;
using System.Linq; 
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using System.Xml.Linq;

public class UtilMath {

	//constants
	public static float MAXIMUM_LATITUDE = 90.0f;
	public static float MINIMUM_LATITUDE = -90.0f;
	public static float MAXIMUM_LONGITUDE = 180.0f;
	public static float MINIMUM_LONGITUDE = -180.0f;

	//Unity max vertices count per mesh
	public static int MAXIMUM_VERTICES_COUNT = 65534; 

	public static Vector3 getCentroid(List<Vector3> plots)
	{
		Vector3 center = new Vector3();

		foreach (Vector3 item in plots) {

			center+=item;
		}
		center/=(plots.Count);
		return center;

	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
		Vector3 dir = point - pivot; // get point direction relative to pivot
		dir = Quaternion.Euler(angles) * dir; // rotate it
		point = dir + pivot; // calculate rotated point
		return point; // return it
	}

	// scsale data between 2 spaces
	public static float normaliseValue(float value, float i0, float i1, float j0, float j1)
	{
		float L = (j0 - j1) / (i0 - i1);
		return (j0 - (L * i0) + (L * value));
	}

	public static float animateSlowInSlowOut(float t)
	{
		if (t <= 0.5f)
			return 2.0f * t * t;

		else
			return 1.0f - 2.0f * (1.0f - t) * (1.0f - t);            
	}

	//format fileName : @"C:\Users\maxc\Documents\Maxime\DATA FOR VISUALISATION\TEST.BIN"
	public static void SerializeVector3(Vector3[] data, string fileName)
	{
		using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
		{
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, data);
		}
	}

	//format fileName : @"C:\Users\maxc\Documents\Maxime\DATA FOR VISUALISATION\TEST.BIN"
	public static Vector3[] DeserializeVector3(string fileName)
	{
		using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
		{

			BinaryFormatter bf = new BinaryFormatter();
			Vector3[] result = (Vector3[])bf.Deserialize(fs);		
			
			return result;
		}
	}

	//format fileName : @"C:\Users\maxc\Documents\Maxime\DATA FOR VISUALISATION\TEST.BIN"
	public static void SerializeInt(int[] data, string fileName)
	{
		using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
		{
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, data);
		}
	}
	
	//format fileName : @"C:\Users\maxc\Documents\Maxime\DATA FOR VISUALISATION\TEST.BIN"
	public static int[] DeserializeInt(string fileName)
	{
		using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
		{
			
			BinaryFormatter bf = new BinaryFormatter();
			int[] result = (int[])bf.Deserialize(fs);		
			
			return result;
		}
	}




	/*========================================== FILE PARSING =================================================*/
	
	public static string openFile(string filePath)
	{
		StreamReader sr = new StreamReader(filePath);

		string s ="";
		string info="";
		
		while((s = sr.ReadLine())!=null)
		{
			info+=s;
		}
		
		sr.Close();
		
		return info;
	}

	/*========================================== GEOMETRY =====================================================*/

	/*public Vector3 rotateAround(Vector3 point, Vector3 Axis, Vector3 reference)
	{
		return point*Rotate
	}*/

	/// <summary>
	/// Projects a point on sphere.
	/// </summary>
	/// <returns>The on sphere.</returns>
	/// <param name="center">Center.</param>
	/// <param name="radius">Radius.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public static Vector3 projectOnSphere (Vector3 center, float radius, float x, float y)
	{
		float theta = 2f * Mathf.PI * x;
		float phi = Mathf.Acos(2f * y - 1f);
		
		float xS = center.x + (radius * Mathf.Sin(theta) * Mathf.Cos(phi));
		float yS = center.y + (radius * Mathf.Sin(phi) * Mathf.Sin(theta));
		float zS = ( center.z + (radius * Mathf.Cos(theta)));
		
		return new Vector3(xS,yS,zS);
	}


	/*public static float animateFastInFastOut(float t)
	{
		if (t <= 0.5f)
			return Mathf.Pow(2.0f * t, 0.75f) / 2f;
		else
			return 1.0f - (Mathf.Pow((2.0f * (1.0 - t)), 0.75f)) / 2f;
	}*/

}
