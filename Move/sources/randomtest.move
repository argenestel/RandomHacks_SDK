module RandomVarSystem::RandomVarRewardSystem {
    use std::vector;
    use std::signer;
    use aptos_framework::account;
    use aptos_framework::timestamp;
    use aptos_framework::coin::{Self, Coin};
    use aptos_framework::aptos_coin::AptosCoin;
    use aptos_framework::randomness;
    use aptos_std::smart_vector;
    use aptos_std::smart_vector::SmartVector;


    struct WalletList has key {
        wallets: vector<address>,
    }

    struct RewardState has key {
        reward_active: bool,
        reward_winners: vector<address>,
    }

    const SMART_CONTRACT_ADDRESS: address = @0x83af638081fc385750856d3db2bf47034243091869e6bce1ced66644e0ac97f4; // Replace with the actual smart contract address
    const REWARD_AMOUNT: u64 = 1000000; // Reward amount in APT (e.g., 1 APT)
    const NUM_REWARD_WINNERS: u64 = 1; // Number of reward winners

    public entry fun init_wallet_list(admin: &signer) {
        assert!(signer::address_of(admin) == SMART_CONTRACT_ADDRESS, 1);
        if (!exists<WalletList>(SMART_CONTRACT_ADDRESS)) {
            let wallet_list = WalletList {
                wallets: vector::empty(),
            };
            move_to(admin, wallet_list);
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

    public entry fun start_reward_group(admin: &signer, owner_addresses: vector<address>) acquires WalletList {
        assert!(signer::address_of(admin) == SMART_CONTRACT_ADDRESS, 1);
        assert!(!exists<RewardState>(SMART_CONTRACT_ADDRESS), 2);

        if (!exists<WalletList>(SMART_CONTRACT_ADDRESS)) {
            let wallet_list = WalletList {
                wallets: vector::empty(),
            };
            move_to(admin, wallet_list);
        };

        let wallet_list = borrow_global_mut<WalletList>(SMART_CONTRACT_ADDRESS);
        vector::append(&mut wallet_list.wallets, owner_addresses);

        if (!exists<RewardState>(SMART_CONTRACT_ADDRESS)) {
            let reward_state = RewardState {
                reward_active: true,
                reward_winners: vector::empty(),
            };
            move_to(admin, reward_state);
        }
    }

    public entry fun end_reward_groupA(admin: &signer, reward_amount: u64) acquires WalletList, RewardState {
        assert!(signer::address_of(admin) == SMART_CONTRACT_ADDRESS, 1);
        assert!(exists<RewardState>(SMART_CONTRACT_ADDRESS), 2);

        let reward_state = borrow_global_mut<RewardState>(SMART_CONTRACT_ADDRESS);
        assert!(reward_state.reward_active, 3);

        let wallet_list = borrow_global<WalletList>(SMART_CONTRACT_ADDRESS);
        let num_wallets = vector::length(&wallet_list.wallets);
        assert!(num_wallets > 0, 4);

        let winning_index = randomness::u64_range(0, num_wallets);
        let winning_wallet = *vector::borrow(&wallet_list.wallets, winning_index);
        if (!vector::contains(&reward_state.reward_winners, &winning_wallet)) {
            vector::push_back(&mut reward_state.reward_winners, winning_wallet);
            let reward_coins = coin::withdraw<AptosCoin>(admin, reward_amount);
            coin::deposit(winning_wallet, reward_coins);
        };

        reward_state.reward_active = false;
    }

public entry fun end_reward_group(admin: &signer, owner_addresses: vector<address>, reward_amount: u64) acquires WalletList, RewardState {
    assert!(signer::address_of(admin) == SMART_CONTRACT_ADDRESS, 1);
    assert!(exists<RewardState>(SMART_CONTRACT_ADDRESS), 2);
    let reward_state = borrow_global_mut<RewardState>(SMART_CONTRACT_ADDRESS);
    assert!(reward_state.reward_active, 3);

    let wallet_list = borrow_global_mut<WalletList>(SMART_CONTRACT_ADDRESS);
    vector::append(&mut wallet_list.wallets, owner_addresses);

    let num_wallets = vector::length(&wallet_list.wallets);
    let winning_index = randomness::u64_range(0, num_wallets);
    let winning_wallet = *vector::borrow(&wallet_list.wallets, winning_index);
            if (!vector::contains(&reward_state.reward_winners, &winning_wallet)) {
                vector::push_back(&mut reward_state.reward_winners, winning_wallet);
                let reward_coins = coin::withdraw<AptosCoin>(admin, reward_amount);
                coin::deposit(winning_wallet, reward_coins);
            };

    reward_state.reward_active = false;
}


    public entry fun end_reward(admin: &signer, num_rewards : u64, reward_amount: u64) acquires WalletList, RewardState {
        assert!(signer::address_of(admin) == SMART_CONTRACT_ADDRESS, 1);
        assert!(exists<RewardState>(SMART_CONTRACT_ADDRESS), 2);

        let reward_state = borrow_global_mut<RewardState>(SMART_CONTRACT_ADDRESS);
        assert!(reward_state.reward_active, 3);

        let wallet_list = borrow_global<WalletList>(SMART_CONTRACT_ADDRESS);
        let num_wallets = vector::length(&wallet_list.wallets);


            let winning_index = randomness::u64_range(0, num_wallets);
            let winning_wallet = *vector::borrow(&wallet_list.wallets, winning_index);
            if (!vector::contains(&reward_state.reward_winners, &winning_wallet)) {
                vector::push_back(&mut reward_state.reward_winners, winning_wallet);
                let reward_coins = coin::withdraw<AptosCoin>(admin, reward_amount);
                coin::deposit(winning_wallet, reward_coins);
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