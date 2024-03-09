module getrandnumb::RandomnessExample22 {
    use aptos_framework::randomness;
    use std::signer;

    struct RandomNumber has key {
        value: u64,
    }

    public entry fun generate_random_u64(account: &signer) acquires RandomNumber {
        let random_number = randomness::u64_integer();
        let account_addr = signer::address_of(account);

        if (exists<RandomNumber>(account_addr)) {
            let random_number_resource = borrow_global_mut<RandomNumber>(account_addr);
            random_number_resource.value = random_number;
        } else {
            move_to(account, RandomNumber { value: random_number });
        }
    }

    public fun get_random_number(addr: address): u64 acquires RandomNumber {
        borrow_global<RandomNumber>(addr).value
    }
}
