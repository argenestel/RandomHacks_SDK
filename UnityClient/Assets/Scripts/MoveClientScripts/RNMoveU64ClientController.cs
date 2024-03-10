using System.Collections;
using System.Collections.Generic;
using Aptos.Accounts;
using Aptos.HdWallet;
using Aptos.Unity.Rest;
using Aptos.Unity.Rest.Model;
using UnityEngine;

namespace Playsphere.RandomSDK.U64
{
    public class RNMoveU64ClientController : MonoBehaviour
    {
        [SerializeField] private string _functionName;

        private void Start()
        {
            RestClient.Instance.SetEndPoint("https://fullnode.random.aptoslabs.com/v1");
            // StartCoroutine(OnCallRandomFunction());
        }

        public IEnumerator OnCallRandomFunction()
        {
            TransactionPayload payload = new TransactionPayload()
            {
                Type = Constants.ENTRY_FUNCTION_PAYLOAD,
                Function = _functionName,
                TypeArguments = new string[] { },
                Arguments = new Arguments()
                {
                    ArgumentStrings = new string[] { }
                }
            };

            ResponseInfo responseInfo = new ResponseInfo();
            Transaction transferTxn = new Transaction();
            Account account = new Account();

            Coroutine fundAliceAccountCor = StartCoroutine(FaucetClient.Instance.FundAccount((success, fundResponseInfo) =>
            {
                responseInfo = fundResponseInfo;
            }, account.AccountAddress.ToString(), 100000000, "https://faucet.random.aptoslabs.com/"));
            yield return fundAliceAccountCor;
            Debug.Log(account.AccountAddress);
            // Create wait for Transaction Coroutine
            bool waitForTxnSuccess = false;
            // string transactionHash = "";
            AccountResourceCoin.Coin accountData = new AccountResourceCoin.Coin();
            Coroutine getAccountFunds = StartCoroutine(RestClient.Instance.GetAccountBalance((_account, _responseInfo) =>
            {
                accountData = _account;
                responseInfo = _responseInfo;
            }, account.AccountAddress));
            yield return getAccountFunds;
            Debug.Log(accountData.Value);
            
         

            Coroutine SubmitTransactionCor = StartCoroutine(RestClient.Instance.SubmitTransaction((transaction, submitResponseInfo) =>
            {
                transferTxn = transaction;
                responseInfo = submitResponseInfo;
            }, account, payload));
            yield return SubmitTransactionCor;

            if (responseInfo.status != ResponseInfo.Status.Success)
            {
                Debug.LogWarning("Transfer failed: " + responseInfo.message);
                yield break;
            }

            Debug.Log("Transfer Response: " + responseInfo.message);
            string transactionHash = transferTxn.Hash;
            Debug.Log("Transfer Response Hash: " + transferTxn.Hash);
            Coroutine waitForTransactionCor = StartCoroutine(
                RestClient.Instance.WaitForTransaction((_pending, _responseInfo) =>
                {
                    waitForTxnSuccess = _pending;
                    responseInfo = _responseInfo;
                }, transactionHash)
            );
            yield return waitForTransactionCor;

            if (!waitForTxnSuccess)
            {
                Debug.LogWarning("Transaction was not found. Breaking out of example: Error: " + responseInfo.message);
                yield break;
            }
        }
    }
}