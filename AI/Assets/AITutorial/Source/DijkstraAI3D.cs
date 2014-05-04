using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DijkstraAI3D : MonoBehaviour {
	
	public Color startColor = Color.green;
	public Color endColor = Color.red;
	public Color unvisitedColor = Color.white;
	public Color visitedColor = Color.gray;
	public Color processingColor = Color.yellow;
	public Color pathColor = Color.green;
	public LineRenderer lineRenderer;
	public GameObject mainCamera;

	int width = 20;
	int height = 10;
	int depth = 10;

	PathNode startingPathNode;
	PathNode endingPathNode;

	List<PathNode> pathNodes = new List<PathNode>();
	List<PathNode> path = new List<PathNode>();

	bool pathCompleted;
	bool inProgress;
	bool isFirstSetup = true;

	// Use this for initialization
	void Start () {
		SetupPathNodes(width,height, depth);
		mainCamera.transform.position = new Vector3(width / 2, height / 2, -depth);
	}

	void Update(){
		if(pathCompleted || Input.GetKeyDown(KeyCode.R)){
			StopAllCoroutines();
			SetupPathNodes(width, height, depth);
		}

		lineRenderer.SetVertexCount(path.Count);
		for(int i = 0; i < path.Count; i++){
			lineRenderer.SetPosition(i, path[i].transform.localPosition);
		}

		mainCamera.transform.RotateAround(new Vector3(width / 2, height / 2, depth / 2), Vector3.up, 20 * Time.deltaTime);
	}

	IEnumerator FindPath(){
		PathNode currentNode = startingPathNode;
		bool pathFound = false;
		float nodeSearchDistance = 0;
		float total = 0;
		List<PathNode> processingNodes = new List<PathNode>();

		inProgress = true;

		while(!pathFound && inProgress){
			for(int i = 0; i < pathNodes.Count; i++){

				float distance = Vector3.Distance(currentNode.transform.position, pathNodes[i].transform.position); // A* like
//				float distance = Vector3.Distance(startingPathNode.transform.position, pathNodes[i].transform.position); //Dijkstra like
				
				if(distance < nodeSearchDistance){
					processingNodes.Add(pathNodes[i]);
					if(currentNode != startingPathNode)
						currentNode.renderer.material.color = processingColor;
				}
			}

			if(processingNodes.Count > 0){
				float smallestWeight = Mathf.Infinity;
				foreach(PathNode node in processingNodes){
					if(node == endingPathNode){
						pathFound = true;
						break;
					}
					
					node.renderer.material.color = visitedColor;
					if(node.weight < smallestWeight){
						smallestWeight = node.weight;
						currentNode = node;
//						nodeSearchDistance = 0; //uncomment for a*
						pathNodes.Remove(currentNode);
					}
				}

				path.Add(currentNode);
				processingNodes.Clear();
			}
			
			nodeSearchDistance += 0.5f;
			yield return new WaitForSeconds(0.05f);
		}

		foreach(PathNode pathNode in path){
			pathNode.renderer.material.color = pathColor;
		}

		path.Add(endingPathNode);
	
		StartCoroutine(TravelPath ());

		yield return null;
	}

	IEnumerator TravelPath(){
		int index = 0;
		while(index < path.Count){
			startingPathNode.transform.position = Vector3.MoveTowards(startingPathNode.transform.position, path[index].transform.position, 10f * Time.deltaTime);

			if(startingPathNode.transform.position == path[index].transform.position)
				index++;

			yield return new WaitForSeconds(0.01f);
		}

		pathCompleted = true;
	}

	// Used for generating the path nodes
	void SetupPathNodes(int x, int y, int z){
		inProgress = false;

		path.Clear();
		pathNodes.Clear();
		pathCompleted = false;

		if(isFirstSetup){
			for(int i = 0; i < x; i++){
				for(int j = 0; j < y; j++){
					for(int k = 0; k < z; k++){
						GameObject node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
						node.transform.position = new Vector3(i, j, k);
						PathNode pathNode = node.AddComponent<PathNode>();
						node.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
					}
				}
			}

			isFirstSetup = false;
		}

		pathNodes = GameObject.FindObjectsOfType<PathNode>().ToList();

		// Uncomment to make the starting node and ending node the first and last node in the array.
		//		startingPathNode = pathNodes[0];
		//		endingPathNode = pathNodes[pathNodes.Count - 1];
		
		// Uncomment to make the starting node and ending node random
		startingPathNode = pathNodes[Random.Range(0, pathNodes.Count -1)];
		endingPathNode = pathNodes[Random.Range(0, pathNodes.Count -1)];
		
		foreach(PathNode node in pathNodes){
			node.renderer.material.shader = Shader.Find("Transparent/Diffuse");
			
			if(node == startingPathNode){
				node.weight = 0;
				node.renderer.material.color = startColor;
			}
			else if(node == endingPathNode){
				node.weight = Mathf.Infinity;
				node.renderer.material.color = endColor;
			}
			else{
				node.weight = Random.Range(0.0f,10.0f);
				node.renderer.material.color = unvisitedColor;
			}
		}
		
		pathNodes.Remove(startingPathNode);
		
		StartCoroutine(FindPath());
	}
}





