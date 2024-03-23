using System;
using Playsphere.RandomSDK.U64;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.Events;
public class u64_ResourcesConverter : MonoBehaviour
{
   
      public static UnityAction<ulong, ulong> RNRandom; 
     void OnEnable()
     {
        RNMoveU64ClientController.resourcesResponse += OnResponseEvent;
     }
     void OnDisable()
     {
        RNMoveU64ClientController.resourcesResponse -= OnResponseEvent;
     }

    private void OnResponseEvent(string data)
    {
      print(data);
      try{

        var jdata = JObject.Parse(data);
        RNRandom?.Invoke(ulong.Parse(jdata["data"]["value"].ToString()), ulong.Parse(jdata["data"]["max_value"].ToString()));
      }
        catch(Exception e){
         Debug.Log(e);
        }

    }
}