module Varsystem::RandomnessVarSystemTest {
    use std::vector;
    use std::signer;
    use aptos_framework::account;
    use aptos_framework::timestamp;
    use aptos_framework::coin::{Self, Coin};
    use aptos_framework::aptos_coin::AptosCoin;

    struct WalletList has key {
        wallets: vector<address>,
    }

    struct RewardState has key {
        reward_active: bool,
        reward_winners: vector<address>,
    }

    const SMART_CONTRACT_ADDRESS: address = @0xa82bcd7a9092cab8ff06caea73008aca9226e368ec5af1f14d99d36abb0a8f46; // Replace with the actual smart contract address
    const REWARD_AMOUNT: u64 = 1000000; // Reward amount in APT (e.g., 1 APT)
    const NUM_REWARD_WINNERS: u64 = 1; // Number of reward winners

    public entry fun init_wallet_list(account: &signer) {
        if (!exists<WalletList>(SMART_CONTRACT_ADDRESS)) {
            let wallet_list = WalletList {
                wallets: vector::empty(),
            };
            move_to(account, wallet_list);
        }
    }

public entry fun add_wallet(account: &signer) acquires WalletList {
    let wallet_address = signer::address_of(account);
    if (!exists<WalletList>(SMART_CONTRACT_ADDRESS)) {
        let wallet_list = WalletList {
            wallets: vector::empty(),
        };
        move_to(account, wallet_list);
    };
    let wallet_list = borrow_global_mut<WalletList>(SMART_CONTRACT_ADDRESS);
    if (!vector::contains(&wallet_list.wallets, &wallet_address)) {
        vector::push_back(&mut wallet_list.wallets, wallet_address);
    };
}

    public fun get_wallets(account: &signer): vector<address> acquires WalletList {
        let wallet_address = signer::address_of(account);
        assert!(exists<WalletList>(wallet_address), 1);
        let wallet_list = borrow_global<WalletList>(wallet_address);
        *&wallet_list.wallets
    }

    public entry fun start_reward(admin: &signer) acquires WalletList {
        assert!(signer::address_of(admin) == SMART_CONTRACT_ADDRESS, 1);
        assert!(!exists<RewardState>(SMART_CONTRACT_ADDRESS), 2);

        let wallet_list = borrow_global<WalletList>(SMART_CONTRACT_ADDRESS);
        assert!(vector::length(&wallet_list.wallets) > 0, 3);

        if (!exists<RewardState>(SMART_CONTRACT_ADDRESS)) {
            let reward_state = RewardState {
                reward_active: true,
                reward_winners: vector::empty(),
            };
            move_to(admin, reward_state);
        }
    }

    public entry fun end_reward(admin: &signer) acquires WalletList, RewardState {
        assert!(signer::address_of(admin) == SMART_CONTRACT_ADDRESS, 1);
        assert!(exists<RewardState>(SMART_CONTRACT_ADDRESS), 2);

        let reward_state = borrow_global_mut<RewardState>(SMART_CONTRACT_ADDRESS);
        assert!(reward_state.reward_active, 3);

        let wallet_list = borrow_global<WalletList>(SMART_CONTRACT_ADDRESS);
        let num_wallets = vector::length(&wallet_list.wallets);

        let i = 0;
        while (i < NUM_REWARD_WINNERS && i < num_wallets) {
            let winning_index = i % num_wallets;
            let winning_wallet = *vector::borrow(&wallet_list.wallets, winning_index);
            if (!vector::contains(&reward_state.reward_winners, &winning_wallet)) {
                vector::push_back(&mut reward_state.reward_winners, winning_wallet);
                let reward_coins = coin::withdraw<AptosCoin>(admin, REWARD_AMOUNT);
                coin::deposit(winning_wallet, reward_coins);
            };
            i = i + 1;
        };

        reward_state.reward_active = false;
    }

    public entry fun reset_reward(admin: &signer) acquires WalletList, RewardState {
        assert!(signer::address_of(admin) == SMART_CONTRACT_ADDRESS, 1);

        if (exists<WalletList>(SMART_CONTRACT_ADDRESS)) {
            let wallet_list = move_from<WalletList>(SMART_CONTRACT_ADDRESS);
            let WalletList { wallets: _ } = wallet_list;
        };

        if (exists<RewardState>(SMART_CONTRACT_ADDRESS)) {
            let reward_state = move_from<RewardState>(SMART_CONTRACT_ADDRESS);
            let RewardState { reward_active: _, reward_winners: _ } = reward_state;
        };
    }
}