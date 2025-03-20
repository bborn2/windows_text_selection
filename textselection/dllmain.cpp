// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include <windows.h>
#include <string>
#include <iostream>

#include <oleacc.h>
#include <UIAutomation.h>

#include "selection.h"
using namespace selection;

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
         
        break;
    }
    return TRUE;
}

extern "C" __declspec(dllexport) void getSelectionText(void* buffer)
{
    //static std::string text = "Hello from DLL!";


    static std::string text = "";

    try {
        selection_impl::Selection result = selection_impl::GetSelection();

        if (result.process.has_value() && result.process->name.has_value()) {
            text = result.process->name.value() + " :";
        }
        else {
            text = "- : ";
        }

        text += result.text;
    }
    catch (const RuntimeException& e) {
        std::cerr << "Exception: " << e.what() << std::endl;
    }
    catch (const std::exception& e) {
        std::cerr << "Standard Exception: " << e.what() << std::endl;
    }
    catch (...) {
        std::cerr << "Unknown Exception occurred." << std::endl;
    }
    
    size_t maxSize = 1024 - 1; // 留一个字节给 '\0'
     
    errno_t err = strncpy_s(static_cast<char*>(buffer), maxSize, text.c_str(), maxSize - 1);
    static_cast<char*>(buffer)[text.length()] = '\0';

    // 检查 strncpy_s 是否成功
    if (err != 0)
    {
        std::cerr << "Error copying string to buffer!" << std::endl;
    }
    else
    {
        static_cast<char*>(buffer)[maxSize - 1] = '\0'; // 确保结尾是 '\0'
    }

    return;
}

// DLL 内存分配和数据操作
extern "C" __declspec(dllexport) void* allocate_buffer(size_t size)
{
    return malloc(size);  // 分配指定大小的内存
}



extern "C" __declspec(dllexport) void free_buffer(void* buffer)
{
    free(buffer);  // 释放内存
} 

