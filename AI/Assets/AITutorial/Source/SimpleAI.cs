using UnityEngine;
using System.Collections;

public class SimpleAI : MonoBehaviour {

	public GameObject goTo;
	UnityEngine.AI.NavMeshAgent navMeshAgent;

	// Use this for initialization
	void Start () {
		navMeshAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
	}

	void Update(){
		navMeshAgent.SetDestination(goTo.transform.position);
	}
}
