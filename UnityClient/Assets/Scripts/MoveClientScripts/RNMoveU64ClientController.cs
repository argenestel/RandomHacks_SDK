using System;
using System.Collections;
using System.Collections.Generic;
using Aptos.Accounts;
using Aptos.HdWallet;
using Aptos.Unity.Rest;
using Aptos.Unity.Rest.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Playsphere.RandomSDK.U64
{
    public class RNMoveU64ClientController : MonoBehaviour
    {
        [SerializeField] private string _functionName;
        [SerializeField] private string _resources;
        public static UnityAction<string> resourcesResponse;


        public IEnumerator SubmitRandomTransaction(){
            RestClient.Instance.SetEndPoint(Constants.RANDOMNET);

            LedgerInfo ledgerInfo = new LedgerInfo();
            ResponseInfo responseInfo = new ResponseInfo();
            Coroutine ledgerInfoCor = StartCoroutine(RestClient.Instance.GetInfo((_ledgerInfo, _responseInfo) =>
            {
                ledgerInfo = _ledgerInfo;
                responseInfo = _responseInfo;
            }));
            yield return ledgerInfoCor;

            if(responseInfo.status != ResponseInfo.Status.Success)
            {
                Debug.LogError(responseInfo.message);
                yield break;
            }

            Debug.Log("Chain ID: " + ledgerInfo.ChainId);

            Transaction transferTxn = new Transaction();


             TransactionPayload txpayload = new TransactionPayload {
                Type = Constants.ENTRY_FUNCTION_PAYLOAD,
                Function = _functionName,
                TypeArguments = new string[] {  },
                Arguments = new Arguments()
                {
                    ArgumentStrings = new string[] {  }
                }
            };
            Account account = new Wallet(PlayerPrefs.GetString("Mnemonic")).Account;
            Coroutine Transact = StartCoroutine(RestClient.Instance.SubmitTransaction((_transaction, _responseInfo) =>
            {
                transferTxn = _transaction;
                responseInfo = _responseInfo;
            }, account, txpayload));

            yield return Transact;

            if(responseInfo.status != ResponseInfo.Status.Success)
            {
                Debug.LogWarning("Transfer failed: " + responseInfo.message);
                yield break;
            }

            Debug.Log("Transfer Response: " + responseInfo.message);
            string transactionHash2 = transferTxn.Hash;
            Debug.Log("Transfer Response Hash: " + transferTxn.Hash);

            bool waitForTxnSuccess2 = false;
            Coroutine waitForTransactionCor2 = StartCoroutine(
                RestClient.Instance.WaitForTransaction((_pending, _responseInfo) =>
                {
                    waitForTxnSuccess2 = _pending;
                    responseInfo = _responseInfo;
                }, transactionHash2)
            );
            yield return waitForTransactionCor2;

            if(!waitForTxnSuccess2)
            {
                Debug.LogWarning("Transaction was not found. Breaking out of example", gameObject);
                yield break;
            }
        }



        public IEnumerator CallRandomNumberEnum(){
            StartCoroutine(SubmitRandomTransaction());
            ResponseInfo responseInfo = new ResponseInfo();
            Transaction transferTxn = new Transaction();
            Account account = new Wallet(PlayerPrefs.GetString("Mnemonic")).Account;

            Coroutine Transact = StartCoroutine(RestClient.Instance.GetAccountResource((_isSuccess, _long, _responseInfo) =>
            {
                print(_responseInfo);
                if(_isSuccess){
                resourcesResponse?.Invoke(_responseInfo);
                }
            }, account.AccountAddress , _resources));
            yield return Transact;
           
            if(responseInfo.status != ResponseInfo.Status.Success)
            {
                Debug.LogWarning("Transfer failed: " + responseInfo.message);
                yield break;
            }
        }


// EDITOR Functionality
        public void CallRandomNumber(){
            StartCoroutine(CallRandomNumberEnum());
        
        }
        public void OnCallRandomFunctionEditor()
        {
             StartCoroutine(SubmitRandomTransaction()); 
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RNMoveU64ClientController))]
    public class MoveAccountCreationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            RNMoveU64ClientController controller = (RNMoveU64ClientController)target;
            if (GUILayout.Button("Submit Transaction"))
            {
                controller.OnCallRandomFunctionEditor();
            }

            if(GUILayout.Button("Call Random Number"))
            {
                controller.CallRandomNumber();
            }
        }
    }
#endif
        
}