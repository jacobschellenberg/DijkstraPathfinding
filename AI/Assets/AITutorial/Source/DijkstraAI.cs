using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DijkstraAI : MonoBehaviour {
	
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 10;
    [SerializeField] private bool randomStartingPosition = true;
    [SerializeField] private bool randomEndingPosition = true;
    [SerializeField] private PathNode _pathNodePrefab;

    [SerializeField] private Color startColor = Color.green;
    [SerializeField] private Color endColor = Color.red;
    [SerializeField] private Color unvisitedColor = Color.white;
    [SerializeField] private Color visitedColor = Color.gray;
    [SerializeField] private Color processingColor = Color.yellow;
    [SerializeField] private Color pathColor = Color.green;
    [SerializeField] private LineRenderer lineRenderer;

	private PathNode startingPathNode;
    private PathNode endingPathNode;

    private List<PathNode> pathNodes = new List<PathNode>();
    private List<PathNode> path = new List<PathNode>();

    private bool pathCompleted;

    private void Start () 
    {
		CreatePathNodes(width,height);

		pathNodes = GameObject.FindObjectsOfType<PathNode>().ToList();

        if(randomStartingPosition)
        {
            startingPathNode = pathNodes[Random.Range(0, pathNodes.Count)];
        }
        else
        {
            startingPathNode = pathNodes[0];
        }

        if(randomEndingPosition)
        {
            endingPathNode = pathNodes[Random.Range (0, pathNodes.Count)];
        }
        else
        {
            endingPathNode = pathNodes[pathNodes.Count - 1];
        }

		foreach(PathNode node in pathNodes)
        {
			if(node == startingPathNode)
            {
				node.weight = 0;
				node.GetComponent<Renderer>().material.color = startColor;
			}
			else if(node == endingPathNode)
            {
				node.weight = Mathf.Infinity;
				node.GetComponent<Renderer>().material.color = endColor;
			}
			else
            {
				node.weight = Random.Range(0.0f,10.0f);
				node.GetComponent<Renderer>().material.color = unvisitedColor;
			}
		}

		pathNodes.Remove(startingPathNode);
		
		StartCoroutine(FindPath());
	}

    private void Update()
    {
		if(pathCompleted || Input.GetKeyDown(KeyCode.R))
        {
			Application.LoadLevel(0);
			pathCompleted = false;
		}

		lineRenderer.SetVertexCount(path.Count);

		for(int i = 0; i < path.Count; i++)
        {
			lineRenderer.SetPosition(i, path[i].transform.localPosition);
		}
	}

    private IEnumerator FindPath()
    {
		PathNode currentNode = startingPathNode;
		bool pathFound = false;
		float nodeSearchDistance = 0;
		float total = 0;
		List<PathNode> processingNodes = new List<PathNode>();

		while(!pathFound)
        {
			for(int i = 0; i < pathNodes.Count; i++)
            {
				float distance = Vector3.Distance(currentNode.transform.position, pathNodes[i].transform.position); // A* like
				//float distance = Vector3.Distance(startingPathNode.transform.position, pathNodes[i].transform.position); //Dijkstra like
				
				if(distance < nodeSearchDistance)
                {
					processingNodes.Add(pathNodes[i]);
                    if (currentNode != startingPathNode)
                    {
                        currentNode.GetComponent<Renderer>().material.color = processingColor;
                    }
				}
			}

			if(processingNodes.Count > 0)
            {
				float smallestWeight = Mathf.Infinity;
				foreach(PathNode node in processingNodes)
                {
					if(node == endingPathNode)
                    {
						pathFound = true;
						break;
					}
					
					node.GetComponent<Renderer>().material.color = visitedColor;

					if(node.weight < smallestWeight)
                    {
						smallestWeight = node.weight;
						currentNode = node;
						//nodeSearchDistance = 0; //uncomment for a*
						pathNodes.Remove(currentNode);
					}
				}

				path.Add(currentNode);
				processingNodes.Clear();
			}
			
			nodeSearchDistance += 0.5f;
			yield return new WaitForSeconds(0.05f);
		}

		foreach(PathNode pathNode in path)
        {
			pathNode.GetComponent<Renderer>().material.color = pathColor;
		}

		path.Add(endingPathNode);
	
		StartCoroutine(TravelPath ());

		yield return null;
	}

    private IEnumerator TravelPath()
    {
		int index = 0;
		while(index < path.Count)
        {
			startingPathNode.transform.position = Vector3.MoveTowards(startingPathNode.transform.position, path[index].transform.position, 10f * Time.deltaTime);

            if (startingPathNode.transform.position == path[index].transform.position)
            {
                index++;
            }

			yield return new WaitForSeconds(0.01f);
		}

		pathCompleted = true;
	}

    private void CreatePathNodes(int x, int y)
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject node = Instantiate(_pathNodePrefab.gameObject);
				node.transform.position = new Vector3(i,j);
                PathNode pathNode = node.GetComponent<PathNode>();
				node.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			}
		}
	}
}