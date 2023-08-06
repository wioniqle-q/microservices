use aes::Aes128;
use aes::cipher::{BlockEncrypt, BlockDecrypt};
use aes::cipher::generic_array::GenericArray;
use std::str;
use libc::size_t;
use aes::cipher::KeyInit;

fn encrypt_string(input: &str, key: &[u8; 16]) -> Result<Vec<u8>, String> {
    let cipher = Aes128::new(GenericArray::from_slice(key));

    let input_bytes = input.as_bytes();
    let padding_length = 16 - (input_bytes.len() % 16);
    let mut padded_input = input_bytes.to_vec();
    for _ in 0..padding_length {
        padded_input.push(padding_length as u8);
    }

    let mut encrypted_data = Vec::with_capacity(padded_input.len());
    for chunk in padded_input.chunks_exact(16) {
        let mut block = GenericArray::clone_from_slice(chunk);
        cipher.encrypt_block(&mut block);
        encrypted_data.extend_from_slice(&block);
    }

    Ok(encrypted_data)
}

fn decrypt_string(encrypted_data: &[u8], key: &[u8; 16]) -> Result<String, String> {
    let cipher = Aes128::new(GenericArray::from_slice(key));

    let mut decrypted_data = Vec::with_capacity(encrypted_data.len());
    for chunk in encrypted_data.chunks_exact(16) {
        let mut block = GenericArray::clone_from_slice(chunk);
        cipher.decrypt_block(&mut block);
        decrypted_data.extend_from_slice(&block);
    }

    let padding_length = *decrypted_data.last().ok_or_else(|| "Invalid padding".to_string())?;
    if padding_length > 16 {
        return Err("Invalid padding".to_string());
    }

    decrypted_data.truncate(decrypted_data.len() - padding_length as usize);

    let decrypted_str = String::from_utf8(decrypted_data).map_err(|_| "Invalid UTF-8".to_string())?;

    Ok(decrypted_str)
}

#[no_mangle]
pub extern "C" fn free_buffer(ptr: *mut u8, size: size_t) {
    unsafe {
        if !ptr.is_null() {
            let layout = std::alloc::Layout::from_size_align_unchecked(size, 1);
            std::ptr::write_bytes(ptr, 0, layout.size());
            std::alloc::dealloc(ptr, layout);
        }
    }
}

fn allocate_buffer(size: usize) -> *mut u8 {
    let layout = std::alloc::Layout::from_size_align(size, 1).unwrap();
    let buffer_ptr = unsafe { std::alloc::alloc(layout) as *mut u8 };
    if !buffer_ptr.is_null() {
        unsafe {
            std::ptr::write_bytes(buffer_ptr, 0, size);
        }
    }
    
    buffer_ptr
}

#[no_mangle]
pub extern "C" fn encrypt(input: *const u8, input_len: size_t, key: *const u8, encrypted_data_len: *mut size_t) -> *mut u8 {
    let input_slice = unsafe { std::slice::from_raw_parts(input, input_len) };
    let key_slice = unsafe { std::slice::from_raw_parts(key, 16) };

    let mut new_key = [0u8; 16];
    new_key.copy_from_slice(key_slice);

    match encrypt_string(str::from_utf8(input_slice).unwrap_or(""), &new_key) {
        Ok(encrypted_data) => {
            let encrypted_data_len_value = encrypted_data.len();
            let encrypted_data_ptr = allocate_buffer(encrypted_data_len_value);
            if !encrypted_data_ptr.is_null() {
                unsafe {
                    std::ptr::copy_nonoverlapping(encrypted_data.as_ptr(), encrypted_data_ptr, encrypted_data_len_value);
                    *encrypted_data_len = encrypted_data_len_value as size_t;
                    std::ptr::write_bytes(input as *mut u8, 0, input_len);
                }
                encrypted_data_ptr
            } else {
                unsafe { *encrypted_data_len = 0 };
                std::ptr::null_mut()
            }
        }
        Err(_) => {
            unsafe { *encrypted_data_len = 0 };
            std::ptr::null_mut()
        }
    }
}

#[no_mangle]
pub extern "C" fn decrypt(encrypted_data: *const u8, encrypted_data_len: size_t, key: *const u8, decrypted_data_len: *mut size_t) -> *mut u8 {
    let encrypted_data_slice = unsafe { std::slice::from_raw_parts(encrypted_data, encrypted_data_len) };
    let key_slice = unsafe { std::slice::from_raw_parts(key, 16) };

    let mut new_key = [0u8; 16];
    new_key.copy_from_slice(key_slice);

    match decrypt_string(encrypted_data_slice, &new_key) {
        Ok(decrypted_data) => {
            let decrypted_data_len_value = decrypted_data.len();
            let decrypted_data_ptr = allocate_buffer(decrypted_data_len_value);
            if !decrypted_data_ptr.is_null() {
                unsafe {
                    std::ptr::copy_nonoverlapping(decrypted_data.as_ptr(), decrypted_data_ptr, decrypted_data_len_value);
                    *decrypted_data_len = decrypted_data_len_value as size_t;
                    std::ptr::write_bytes(encrypted_data as *mut u8, 0, encrypted_data_len);
                }
                decrypted_data_ptr
            } else {
                unsafe { *decrypted_data_len = 0 };
                std::ptr::null_mut()
            }
        }
        Err(_) => {
            unsafe { *decrypted_data_len = 0 };
            std::ptr::null_mut()
        }
    }
}

fn main() {
    let input = "Hello, Rust!";
    let key: [u8; 16] = [1; 16];

    let encrypted_data = encrypt_string(input, &key).unwrap();
    let decrypted_data = decrypt_string(&encrypted_data, &key).unwrap();

    println!("Input: {}", input);

    println!("Encrypted data: {:?}", encrypted_data);

    println!("Decrypted data: {}", decrypted_data);
}