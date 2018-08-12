using UnityEngine;

public class HandSubTriggers : MonoBehaviour {
	Palm hand;

	void Awake() {
		hand = transform.root.GetComponent<Palm> ();
	}

    void OnTriggerStay(Collider col) {
        if (!col.tag.Equals("Palm")) {
            hand.ReceiveTriggers(col.name, gameObject.name);
        }
	}
}
