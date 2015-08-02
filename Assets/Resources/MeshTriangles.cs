using UnityEngine;
using System.Collections;

public class MeshTriangles
{
	public static  void Explode (GameObject obj)
	{
		MeshFilter MF = obj.GetComponent<MeshFilter>();
		MeshRenderer MR = obj.GetComponent<MeshRenderer>();
		Mesh M = MF.mesh;
		Vector3[] verts = M.vertices;
		Vector3[] normals = M.normals;
		Vector2[] uvs = M.uv;
	
		for (int submesh = 0; submesh < M.subMeshCount; submesh++)
		{
			int[] indices = M.GetTriangles(submesh);
			for (int i = 0; i < indices.Length; i += 3)
			{
				if(i % 7 == 0)
				{
					Vector3[] newVerts = new Vector3[3];
					Vector3[] newNormals = new Vector3[3];
					Vector2[] newUvs = new Vector2[3];
					for (int n = 0; n < 3; n++)
					{
						int index = indices[i + n];
						newVerts[n] = verts[index];
						newUvs[n] = uvs[index];
						newNormals[n] = normals[index];
					}
					Mesh mesh = new Mesh();
					mesh.vertices = newVerts;
					mesh.normals = newNormals;
					mesh.uv = newUvs;
					
					mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };
					
					GameObject GO = new GameObject("Triangle " + (i / 3));
					GO.transform.position = obj.transform.position;
					GO.transform.rotation = obj.transform.rotation;
					GO.AddComponent<MeshRenderer>().material = MR.materials[submesh];
					GO.AddComponent<MeshFilter>().mesh = mesh;
					//GO.AddComponent<BoxCollider>();
					GO.AddComponent<Rigidbody>().AddExplosionForce(100f + Random.Range(0f, 500f), obj.transform.position, 30);
					GO.GetComponent<Rigidbody>().useGravity = false;
					GameObject.Destroy(GO, 5f + Random.Range(0.0f, 5f));
				}
			}
		}
		MR.enabled = false;
		
		//Time.timeScale = 0.2f;
		//yield return new WaitForSeconds(0.8f);
		//Time.timeScale = 1.0f;
		//GameObject.Destroy(obj);
	}
	//Like explode, but way more chilled out
	public static  void Fracture (GameObject obj)
	{
		MeshFilter MF = obj.GetComponent<MeshFilter>();
		MeshRenderer MR = obj.GetComponent<MeshRenderer>();
		Mesh M = MF.mesh;
		Vector3[] verts = M.vertices;
		Vector3[] normals = M.normals;
		Vector2[] uvs = M.uv;
		
		for (int submesh = 0; submesh < M.subMeshCount; submesh++)
		{
			int[] indices = M.GetTriangles(submesh);
			for (int i = 0; i < indices.Length; i += 3)
			{
				if(i % 20 == 0)
				{
					Vector3[] newVerts = new Vector3[3];
					Vector3[] newNormals = new Vector3[3];
					Vector2[] newUvs = new Vector2[3];
					for (int n = 0; n < 3; n++)
					{
						int index = indices[i + n];
						newVerts[n] = verts[index];
						newUvs[n] = uvs[index];
						newNormals[n] = normals[index];
					}
					Mesh mesh = new Mesh();
					mesh.vertices = newVerts;
					mesh.normals = newNormals;
					mesh.uv = newUvs;
					
					mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };
					
					GameObject GO = new GameObject("Triangle " + (i / 3));
					GO.transform.position = obj.transform.position;
					GO.transform.rotation = obj.transform.rotation;
					GO.AddComponent<MeshRenderer>().material = MR.materials[submesh];
					GO.AddComponent<MeshFilter>().mesh = mesh;
					//GO.AddComponent<BoxCollider>();
					GO.AddComponent<Rigidbody>();//.AddExplosionForce(100f + Random.Range(0f, 500f), obj.transform.position, 30);
					GO.GetComponent<Rigidbody>().velocity = obj.transform.parent.gameObject.GetComponent<Rigidbody>().velocity;
					GO.GetComponent<Rigidbody>().useGravity = false;
					//yield return null;
					GameObject.Destroy(GO, 5f + Random.Range(5f, 25f));
				}
			}
		}
		MR.enabled = false;
		
		//Time.timeScale = 0.2f;
		//yield return new WaitForSeconds(0.8f);
		//Time.timeScale = 1.0f;
		//GameObject.Destroy(obj);
	}
	
}