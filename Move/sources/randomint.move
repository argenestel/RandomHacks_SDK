module getrandnumb::RandomnessInt3 {
    use aptos_framework::randomness;
    use std::signer;

    struct RandomNumber has key {
        value: u64,
        max_value: u64,
    }

    public entry fun generate_random_u64(account: &signer) acquires RandomNumber {
        let account_addr = signer::address_of(account);
        let max_value = 18446744073709551615; // 2^64 - 1

        if (!exists<RandomNumber>(account_addr)) {
            let random_number = randomness::u64_integer();
            move_to(account, RandomNumber { value: random_number, max_value });
        } else {
            let random_number = borrow_global_mut<RandomNumber>(account_addr);
            random_number.value = randomness::u64_integer();
            random_number.max_value = max_value;
        }
    }

    public fun get_random_number(addr: address): (u64, u64) acquires RandomNumber {
        let random_number = borrow_global<RandomNumber>(addr);
        (random_number.value, random_number.max_value)
    }
}
