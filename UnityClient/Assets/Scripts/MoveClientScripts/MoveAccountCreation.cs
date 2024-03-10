using Aptos.HdWallet;
using NBitcoin;
using UnityEngine;


namespace Playsphere.AccountCreator{

[ExecuteInEditMode]
public class MoveAccountCreation : MonoBehaviour
{
    public void WalletCreator(){
        Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
        Wallet wallet = new Wallet(mnemo);
        PlayerPrefs.SetString("Mnemonic", mnemo.ToString());
        PlayerPrefs.SetString("Address", wallet.GetAccount(0).AccountAddress.ToString());
    }    
}
}