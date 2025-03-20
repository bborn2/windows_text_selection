// Project: selection
// File: lib.rs
// Created Date: 2023-06-04
// Author: Pylogmon <pylogmon@outlook.com>

mod windows;

use std::ffi::CString;
use std::os::raw::c_char;
use crate::windows::get_text as _get_text;

/// Get the text selected by the cursor
///
/// Return empty string if no text is selected or error occurred
/// # Example
///
/// ```
/// use selection::get_text;
///
/// let text = get_text();
/// println!("{}", text);
/// ```
/// 
/// 
#[unsafe(no_mangle)] // 确保函数名称不被 Rust 修改
pub extern "C" fn get_text() -> *mut c_char {
    // let text = "Hello from Rust DLL!".to_string();

    let text = _get_text().trim().to_owned();
    let c_string = CString::new(text).unwrap();
    c_string.into_raw() // 将 Rust 字符串转换为 C 兼容指针
}

// pub extern "C" fn get_text() -> String {
//     _get_text().trim().to_owned()
// }


#[cfg(test)]
mod tests {
    use crate::get_text;
    #[test]
    fn it_works() {
        println!("{}", get_text());
    }
}