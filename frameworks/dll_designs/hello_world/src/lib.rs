use std::alloc::{dealloc, Layout};
use std::ffi::{c_char, CStr, CString};
use argon2::{self, Config, ThreadMode, Variant, Version};
use rand::RngCore;
use rand::rngs::OsRng;
use std::alloc::alloc;

#[no_mangle]
pub extern "C" fn create_hash(
    password: *const c_char,
    salt: *const c_char,
    iterations: u32,
    memory: u32,
    parallelism: u32,
    hash_length: u32,
) -> *mut c_char {
    if password.is_null() || salt.is_null() || iterations == 0 || memory == 0 || parallelism == 0 || hash_length == 0 {
        return std::ptr::null_mut();
    }

    let password = unsafe { CStr::from_ptr(password).to_bytes() };
    let salt = unsafe { CStr::from_ptr(salt).to_bytes() };

    let config = Config {
        variant: Variant::Argon2id,
        version: Version::default(),
        mem_cost: memory,
        time_cost: iterations,
        lanes: parallelism,
        thread_mode: ThreadMode::Parallel,
        secret: &[],
        ad: &[],
        hash_length,
    };

    let salt_vec = if salt.is_empty() {
        let mut salt = vec![0u8; 32];
        OsRng.fill_bytes(&mut salt);
        salt
    } else {
        salt.to_vec()
    };

    let password_str = std::str::from_utf8(password).unwrap_or_default();

    let hash = match argon2::hash_encoded(password_str.as_bytes(), &salt_vec, &config) {
        Ok(hash) => hash,
        Err(_) => return std::ptr::null_mut(),
    };

    let c_str_hash = match CString::new(hash) {
        Ok(c_str) => c_str,
        Err(_) => return std::ptr::null_mut(),
    };

    let c_string = c_str_hash.into_raw();

    let hash_len = hash_len(c_string);
    let layout = Layout::from_size_align(hash_len + 1, 1).unwrap();
    let hash_ptr = unsafe { alloc(layout) as *mut c_char };
    if hash_ptr.is_null() {
        return std::ptr::null_mut();
    }

    unsafe {
        std::ptr::copy_nonoverlapping(c_string, hash_ptr, hash_len);
        *hash_ptr.offset(hash_len as isize) = 0;
    }

    if hash_ptr.is_null() {
        return std::ptr::null_mut();
    }

    unsafe {
        dealloc(c_string as *mut u8, layout);
    }

    hash_ptr
}

#[no_mangle]
pub extern "C" fn verify_hash(
    hash: *const c_char,
    password: *const c_char,
) -> bool {
    if hash.is_null() || password.is_null() {
        return false;
    }

    let hash_str = match unsafe { CStr::from_ptr(hash).to_str() } {
        Ok(str) => str,
        Err(_) => return false,
    };

    let password_bytes = unsafe { CStr::from_ptr(password).to_bytes() };

    match argon2::verify_encoded(hash_str, password_bytes) {
        Ok(result) => result,
        Err(_) => false,
    }
}

#[no_mangle]
pub extern "C" fn free_hash(hash: *mut c_char) {
    if !hash.is_null() {
        let layout = Layout::from_size_align(hash_len(hash) + 1, 1).unwrap();
        unsafe {
            dealloc(hash as *mut u8, layout);
        }
    }
}

fn hash_len(hash: *const c_char) -> usize {
    let mut len = 0;
    while unsafe { *hash.offset(len) } != 0 {
        len += 1;
    }
    len.try_into().unwrap()
}

#[no_mangle]
pub extern "C" fn free_string(string: *mut c_char) {
    if !string.is_null() {
        let len = string_len(string);
        let layout = Layout::from_size_align(len + 1, 1).unwrap();
        unsafe {
            dealloc(string as *mut u8, layout);
        }
    }
}

fn string_len(string: *const c_char) -> usize {
    let mut len = 0;
    while unsafe { *string.offset(len) } != 0 {
        len += 1;
    }
    len.try_into().unwrap()
}