#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <intrin.h>

namespace
{
constexpr unsigned int kMaxSteamApiOrdinal = 4096;
constexpr wchar_t kManagedDllName[] = L"steam_api64_managed.dll";

HMODULE g_managedModule;
FARPROC g_exports[kMaxSteamApiOrdinal];
SRWLOCK g_lock = SRWLOCK_INIT;

struct UnicodeStringCompat
{
    USHORT Length;
    USHORT MaximumLength;
    PWSTR Buffer;
};

struct PebLdrDataCompat
{
    ULONG Length;
    BOOLEAN Initialized;
    PVOID SsHandle;
    LIST_ENTRY InLoadOrderModuleList;
    LIST_ENTRY InMemoryOrderModuleList;
    LIST_ENTRY InInitializationOrderModuleList;
};

struct PebCompat
{
    BYTE Reserved1[0x18];
    PebLdrDataCompat* Ldr;
};

struct LdrDataTableEntryCompat
{
    LIST_ENTRY InLoadOrderLinks;
    LIST_ENTRY InMemoryOrderLinks;
    LIST_ENTRY InInitializationOrderLinks;
    PVOID DllBase;
    PVOID EntryPoint;
    ULONG SizeOfImage;
    UnicodeStringCompat FullDllName;
    UnicodeStringCompat BaseDllName;
};

void Fatal(const wchar_t* message, DWORD error)
{
    UNREFERENCED_PARAMETER(error);
    OutputDebugStringW(L"SKYNET native steam_api proxy fatal: ");
    OutputDebugStringW(message);
    OutputDebugStringW(L"\n");
    TerminateProcess(GetCurrentProcess(), 0x53504E58);
}

void UnlinkListEntry(LIST_ENTRY* entry)
{
    if (entry == nullptr || entry->Flink == nullptr || entry->Blink == nullptr)
    {
        return;
    }

    entry->Blink->Flink = entry->Flink;
    entry->Flink->Blink = entry->Blink;
    entry->Flink = entry;
    entry->Blink = entry;
}

void HideManagedModuleFromPeb(HMODULE module)
{
    if (module == nullptr)
    {
        return;
    }

#if defined(_M_X64)
    auto* peb = reinterpret_cast<PebCompat*>(__readgsqword(0x60));
#else
    auto* peb = reinterpret_cast<PebCompat*>(__readfsdword(0x30));
#endif
    if (peb == nullptr || peb->Ldr == nullptr)
    {
        return;
    }

    LIST_ENTRY* head = &peb->Ldr->InLoadOrderModuleList;
    for (LIST_ENTRY* cursor = head->Flink; cursor != head && cursor != nullptr; cursor = cursor->Flink)
    {
        auto* entry = CONTAINING_RECORD(cursor, LdrDataTableEntryCompat, InLoadOrderLinks);
        if (entry->DllBase == module)
        {
            UnlinkListEntry(&entry->InLoadOrderLinks);
            UnlinkListEntry(&entry->InMemoryOrderLinks);
            UnlinkListEntry(&entry->InInitializationOrderLinks);
            return;
        }
    }
}

void BuildManagedPath(wchar_t* path, DWORD pathChars)
{
    HMODULE self = nullptr;
    if (!GetModuleHandleExW(
            GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
            reinterpret_cast<LPCWSTR>(&BuildManagedPath),
            &self))
    {
        Fatal(L"GetModuleHandleExW failed", GetLastError());
    }

    DWORD length = GetModuleFileNameW(self, path, pathChars);
    if (length == 0 || length >= pathChars)
    {
        Fatal(L"GetModuleFileNameW failed", GetLastError());
    }

    wchar_t* slash = wcsrchr(path, L'\\');
    if (slash == nullptr)
    {
        Fatal(L"Could not locate proxy directory", ERROR_BAD_PATHNAME);
    }

    slash[1] = L'\0';
    if (lstrlenW(path) + lstrlenW(kManagedDllName) + 1 >= static_cast<int>(pathChars))
    {
        Fatal(L"Managed DLL path is too long", ERROR_FILENAME_EXCED_RANGE);
    }

    lstrcatW(path, kManagedDllName);
}

HMODULE EnsureManagedModuleLoaded()
{
    if (g_managedModule != nullptr)
    {
        return g_managedModule;
    }

    wchar_t managedPath[MAX_PATH] = {};
    BuildManagedPath(managedPath, MAX_PATH);

    HMODULE module = LoadLibraryW(managedPath);
    if (module == nullptr)
    {
        Fatal(L"LoadLibraryW steam_api64_managed.dll failed", GetLastError());
    }

    HideManagedModuleFromPeb(module);
    g_managedModule = module;
    return module;
}
}

extern "C" FARPROC ResolveSteamApiExport(unsigned int ordinal)
{
    if (ordinal == 0 || ordinal >= kMaxSteamApiOrdinal)
    {
        Fatal(L"Invalid export ordinal", ordinal);
    }

    FARPROC cached = g_exports[ordinal];
    if (cached != nullptr)
    {
        return cached;
    }

    AcquireSRWLockExclusive(&g_lock);
    cached = g_exports[ordinal];
    if (cached == nullptr)
    {
        HMODULE module = EnsureManagedModuleLoaded();
        cached = GetProcAddress(module, reinterpret_cast<LPCSTR>(static_cast<ULONG_PTR>(ordinal)));
        if (cached == nullptr)
        {
            Fatal(L"GetProcAddress by ordinal failed", GetLastError());
        }

        g_exports[ordinal] = cached;
    }
    ReleaseSRWLockExclusive(&g_lock);
    return cached;
}

BOOL WINAPI DllMain(HINSTANCE instance, DWORD reason, LPVOID)
{
    if (reason == DLL_PROCESS_ATTACH)
    {
        DisableThreadLibraryCalls(instance);
    }

    return TRUE;
}
