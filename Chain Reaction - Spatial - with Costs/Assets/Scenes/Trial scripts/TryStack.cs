using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryStack : MonoBehaviour {

    Stack<int> trialStack;
	// Use this for initialization
	void Start () {
        trialStack = new Stack<int>();
        for(int i = 0; i < 20; i++)
        {
            trialStack.Push(i);
        }
        for(int i = 0; i < trialStack.Count; i++)
        {
            Debug.Log(trialStack.Pop().ToString());
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
