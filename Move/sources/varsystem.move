module Varsystem::RandomnessVarSystem {
    use std::vector;
    use std::signer;
    use aptos_framework::account;

    struct WalletList has key {
        wallets: vector<address>,
    }

    const SMART_CONTRACT_ADDRESS: address = @0xa82bcd7a9092cab8ff06caea73008aca9226e368ec5af1f14d99d36abb0a8f46; // Replace with the actual smart contract address

    public fun init_wallet_list(account: &signer) {
        let wallet_list = WalletList {
            wallets: vector::empty(),
        };
        move_to(account, wallet_list);
    }

    public fun add_wallet(account: &signer) acquires WalletList {
        let wallet_address = signer::address_of(account);
        if (!exists<WalletList>(wallet_address)) {
            init_wallet_list(account);
        };
        let wallet_list = borrow_global_mut<WalletList>(wallet_address);
        if (!vector::contains(&wallet_list.wallets, &SMART_CONTRACT_ADDRESS)) {
            vector::push_back(&mut wallet_list.wallets, SMART_CONTRACT_ADDRESS);
        };
    }

    public fun get_wallets(account: &signer): vector<address> acquires WalletList {
        let wallet_address = signer::address_of(account);
        assert!(exists<WalletList>(wallet_address), 1);
        let wallet_list = borrow_global<WalletList>(wallet_address);
        *&wallet_list.wallets
    }
}
