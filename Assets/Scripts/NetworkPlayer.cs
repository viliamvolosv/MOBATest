using UnityEngine;
using System.Collections;

public class NetworkPlayer : MonoBehaviour {


 
    // Use this for initialization
    void Start () {
        StartCoroutine(SendData());
	}
	
	// Update is called once per frame
	void Update () {
  
	}

    IEnumerator SendData()
    {
        while (true)
        {
                Messages message = new Messages();
                message.type = Messages.Type.Position;
                PlayerState state = new PlayerState();
                state.position = new Vector3Serializer();
                state.position.Fill(transform.position);
                state.rotation = new QuaternionSerializer();
                state.rotation.Fill(transform.rotation);
                message.content = state;
                NetworkManager.Instance.SendGameMsg(message);
                yield return new WaitForSeconds(0.06f);
        }
    }
}
