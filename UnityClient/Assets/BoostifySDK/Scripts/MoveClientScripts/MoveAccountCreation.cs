using System.Collections;
using Aptos.HdWallet;
using Aptos.Unity.Rest;
using Aptos.Unity.Rest.Model;
using NBitcoin;
using UnityEditor;
using UnityEngine;


namespace Playsphere.AccountCreator{


public class MoveAccountCreation : MonoBehaviour
{
        public string EndPoint;
        private bool success;
        private ResponseInfo responseInfo;
        public string faucetEndpoint;

    public void WalletCreator(){
        Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
        Wallet wallet = new Wallet(mnemo);
        PlayerPrefs.SetString("Mnemonic", mnemo.ToString());
        Debug.Log("Wallet Created = " + wallet.Account.AccountAddress);
        Debug.Log("Mnemoic Created = " + mnemo);
        PlayerPrefs.SetString("Address", wallet.GetAccount(0).AccountAddress.ToString());
    }    
    
    public void SetAptosEndPoint(){
        RestClient.Instance.SetEndPoint(EndPoint);
    }

    public void FundAccount(){
        StartCoroutine(FundAccountEnum());
    }
    public IEnumerator FundAccountEnum(){
            Coroutine fundAliceAccountCor = StartCoroutine(FaucetClient.Instance.FundAccount((_success, _responseInfo) =>
            {
                success = _success;
                responseInfo = _responseInfo;
            }, PlayerPrefs.GetString("Address"), 100000000, faucetEndpoint));
            yield return fundAliceAccountCor;

            if (responseInfo.status != ResponseInfo.Status.Success)
            {
                Debug.LogError("Faucet funding for Alice failed: " + responseInfo.message);
                yield break;
            }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MoveAccountCreation))]
public class MoveAccountCreationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MoveAccountCreation controller = (MoveAccountCreation)target;

        if (GUILayout.Button("Create Wallet"))
        {
            controller.WalletCreator();
        }

        if (GUILayout.Button("Set Aptos Endpoint"))
        {
            controller.SetAptosEndPoint();
        }

        if (GUILayout.Button("Fund Account"))
        {
            controller.FundAccount();
        }
    }
}
#endif
}



