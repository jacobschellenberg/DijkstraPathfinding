using UnityEngine;
using System.Collections;

public class SimpleAI : MonoBehaviour {

	public GameObject goTo;
	NavMeshAgent navMeshAgent;

	// Use this for initialization
	void Start () {
		navMeshAgent = this.GetComponent<NavMeshAgent>();
	}

	void Update(){
		navMeshAgent.SetDestination(goTo.transform.position);
	}
}
