using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkOptions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GlobalParameters.IsHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }

}
