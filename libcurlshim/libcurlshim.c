/***************************************************************************
 *
 * Project: libcurl.NET
 *
 * Copyright (c) 2004, 2005 Jeff Phillips (jeff@jeffp.net)
 *
 * This software is licensed as described in the file COPYING, which you
 * should have received as part of this distribution.
 *
 * You may opt to use, copy, modify, merge, publish, distribute and/or sell
 * copies of this Software, and permit persons to whom the Software is
 * furnished to do so, under the terms of the COPYING file.
 *
 * This software is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
 * ANY KIND, either express or implied.
 *
 * $Id: LibCurlShim.c,v 1.1 2005/02/17 22:47:24 jeffreyphillips Exp $
 **************************************************************************/

#if _WIN32 || _WIN64
	#if _WIN64
		#define _ENV_64
	#else
		#define _ENV_32
	#endif
#elif __GNUC__
	#if __x86_64__ || __ppc64__
		#define _ENV_64
	#else
		#define _ENV_32
	#endif
#endif

//#define WIN32_LEAN_AND_MEAN

#define CURLSHIM_API __declspec(dllexport) 

#include <windows.h>
#include <time.h>
#include "seq.h"
#include "list.h"
#include "table.h"
#include "mem.h"

#define CURLOPT_WRITEFUNCTION       20011
#define CURLOPT_WRITEDATA           10001
#define CURLOPT_READFUNCTION        20012
#define CURLOPT_READDATA            10009
#define CURLOPT_PROGRESSFUNCTION    20056
#define CURLOPT_PROGRESSDATA        10057
#define CURLOPT_DEBUGFUNCTION       20094
#define CURLOPT_DEBUGDATA           10095
#define CURLOPT_HEADERFUNCTION      20079
#define CURLOPT_HEADERDATA          10029
#define CURLOPT_SSL_CTX_FUNCTION    20108
#define CURLOPT_SSL_CTX_DATA        10109
#define CURLOPT_IOCTLFUNCTION       20130
#define CURLOPT_IOCTLDATA           10131

#define CURLFORM_END                17

#define CURLSHOPT_LOCKFUNC          3
#define CURLSHOPT_UNLOCKFUNC        4
#define CURLSHOPT_USERDATA          5

#pragma warning(disable : 4100 4311 4312)

static HMODULE          g_hModCurl;
static HMODULE          g_hModSock;
static Table_T          g_delegateTable;
static CRITICAL_SECTION g_csDelegateTable;
static Table_T          g_shareDelegateTable;
static CRITICAL_SECTION g_csShareDelegateTable;

typedef int   (__cdecl *CPROC)();
typedef void* (__cdecl *CPVPROC)();
typedef int   (__cdecl *OPTPROC)(void*, int, __int64);

static CPROC pfn_curl_easy_setopt;
static CPROC pfn_curl_share_setopt;
static CPROC pfn_curl_multi_fdset;
static CPVPROC pfn_curl_multi_info_read;
static FARPROC pfn_curl_formadd;
static FARPROC pfn_select;

static void import_functions()
{
	if (!pfn_curl_easy_setopt)
	{
		pfn_curl_easy_setopt = (CPROC)GetProcAddress(g_hModCurl, "curl_easy_setopt");
	}

	if (!pfn_curl_share_setopt)
	{
		pfn_curl_share_setopt = (CPROC)GetProcAddress(g_hModCurl, "curl_share_setopt");
	}

	if (!pfn_curl_formadd)
	{
		pfn_curl_formadd = (FARPROC)GetProcAddress(g_hModCurl, "curl_formadd");
	}

	if (!pfn_select)
	{
		pfn_select = (FARPROC)GetProcAddress(g_hModSock, "select");
	}

	if (!pfn_curl_multi_fdset)
	{
		pfn_curl_multi_fdset = (CPROC)GetProcAddress(g_hModCurl, "curl_multi_fdset");
	}

	if (!pfn_curl_multi_info_read)
	{
		pfn_curl_multi_info_read = (CPVPROC)GetProcAddress(g_hModCurl, "curl_multi_info_read");
	}
}

CURLSHIM_API void curl_shim_initialize()
{
#ifdef _ENV_32
	g_hModCurl = GetModuleHandle("libcurl.dll");
#else
	g_hModCurl = GetModuleHandle("libcurl64.dll");
#endif
	g_hModSock = GetModuleHandle("ws2_32.dll");
	g_delegateTable = Table_new(16, NULL, NULL);
	InitializeCriticalSection(&g_csDelegateTable);
	g_shareDelegateTable = Table_new(16, NULL, NULL);
	InitializeCriticalSection(&g_csShareDelegateTable);
	import_functions();
}

static void vfree(const void* key, void** value, void* cl)
{
	FREE(*value);
}

CURLSHIM_API void curl_shim_cleanup()
{
	Table_map(g_delegateTable, vfree, NULL);
	Table_free(&g_delegateTable);
	DeleteCriticalSection(&g_csDelegateTable);
	Table_map(g_shareDelegateTable, vfree, NULL);
	Table_free(&g_shareDelegateTable);
	DeleteCriticalSection(&g_csShareDelegateTable);
}

CURLSHIM_API char* curl_shim_get_version_char_ptr(
	void* p, int offset)
{
	char* q = &((char*)p)[offset];
	char** qq = (char**)q;
	return *qq;
}

#pragma message("This will break in 64-bits, unless cURL is rebuilt")
CURLSHIM_API int curl_shim_get_version_int_value(
	void* p, int offset)
{
	int* q = (int*)p;
	q += offset / sizeof(int);
	return *q;
}

CURLSHIM_API int curl_shim_get_number_of_protocols(
	void* p, int protOffset)
{
	int nProtocols = 0;
	char* q = &((char*)p)[protOffset];
	char*** qq = (char***)q;
	char** rr = *qq;
	while (*rr++)
		nProtocols++;
	return nProtocols;
}

CURLSHIM_API char* curl_shim_get_protocol_string(
	void* p, int protOffset, int nProt)
{
	char* q = &((char*)p)[protOffset];
	char*** qq = (char***)q;
	char** rr = *qq;
	return rr[nProt];
}

CURLSHIM_API void* curl_shim_alloc_strings()
{
	Seq_T seq = Seq_new(0);
	return (void*)seq;
}

CURLSHIM_API char* curl_shim_add_string(void* p, char* pInStr)
{
	char* pOutStr;
	Seq_T seq = (Seq_T)p;

	pOutStr = (char*)malloc(strlen(pInStr) + 1);
	strcpy(pOutStr, pInStr);
	Seq_addhi(seq, pOutStr);
	return pOutStr;
}

CURLSHIM_API void curl_shim_free_strings(void* p)
{
	int i, count;
	Seq_T seq = (Seq_T)p;

	count = Seq_length(seq);
	for (i = 0; i < count; i++)
		free(Seq_get(seq, i));
	Seq_free(&seq);
}

CURLSHIM_API void* curl_shim_add_string_to_slist(
	void* lst, char* pInStr)
{
	char* pOutStr;
	List_T list = (List_T)lst;

	pOutStr = (char*)malloc(strlen(pInStr) + 1);
	strcpy(pOutStr, pInStr);
	return List_push(list, (void*)pOutStr);
}

CURLSHIM_API void* curl_shim_get_string_from_slist(
	void* lst, char** ppString)
{
	List_T list = (List_T)lst;
	*ppString = (char*)list->first;
	return (void*)list->rest;
}

CURLSHIM_API void curl_shim_free_slist(void* lst)
{
	void* pvStr = NULL;
	List_T list = (List_T)lst;
	while ((list = List_pop(list, &pvStr)) != NULL)
		free(pvStr);
}

static size_t write_callback_impl(char* szptr, size_t sz,
	size_t nmemb, void* pvThis)
{
	// locate the delegates
	FARPROC fpWriteDel;
	unsigned int* pnDelegates =
		(unsigned int*)Table_get(g_delegateTable, pvThis);

	if (!pnDelegates)
		return 0;
	fpWriteDel = (FARPROC)pnDelegates[0];
	return fpWriteDel(szptr, sz, nmemb, pvThis);
}

static size_t read_callback_impl(void* szptr, size_t sz,
	size_t nmemb, void* pvThis)
{
	// locate the delegates
	FARPROC fpReadDel;
	unsigned int* pnDelegates =
		(unsigned int*)Table_get(g_delegateTable, pvThis);

	if (!pnDelegates)
		return 0;
	fpReadDel = (FARPROC)pnDelegates[1];
	return fpReadDel(szptr, sz, nmemb, pvThis);
}

static int progress_callback_impl(void* pvThis, double dlTotal,
	double dlNow, double ulTotal, double ulNow)
{
	// locate the delegates
	FARPROC fpProgDel;
	unsigned int* pnDelegates =
		(unsigned int*)Table_get(g_delegateTable, pvThis);

	if (!pnDelegates)
		return 0;
	fpProgDel = (FARPROC)pnDelegates[2];
	return fpProgDel(pvThis, dlTotal, dlNow, ulTotal, ulNow);
}

static int debug_callback_impl(void* pvCurl, int infoType,
	char* szMsg, size_t msgSize, void* pvThis)
{
	// locate the delegates
	FARPROC fpDebugDel;
	unsigned int* pnDelegates =
		(unsigned int*)Table_get(g_delegateTable, pvThis);

	if (!pnDelegates)
		return 0;
	fpDebugDel = (FARPROC)pnDelegates[3];
	return fpDebugDel(infoType, szMsg, msgSize, pvThis);
}

static size_t header_callback_impl(char* szptr, size_t sz,
	size_t nmemb, void* pvThis)
{
	// locate the delegates
	FARPROC fpHeaderDel;
	unsigned int* pnDelegates =
		(unsigned int*)Table_get(g_delegateTable, pvThis);

	if (!pnDelegates)
		return 0;
	fpHeaderDel = (FARPROC)pnDelegates[4];
	return fpHeaderDel(szptr, sz, nmemb, pvThis);
}

static int ssl_ctx_callback_impl(void* pvCurl, void* ctx, void* pvThis)
{
	// locate the delegates
	FARPROC fpSslCtxDel;
	unsigned int* pnDelegates =
		(unsigned int*)Table_get(g_delegateTable, pvThis);

	if (!pnDelegates)
		return 0;
	fpSslCtxDel = (FARPROC)pnDelegates[5];
	return fpSslCtxDel(ctx, pvThis);
}

static int ioctl_callback_impl(void* pvCurl, int cmd, void* pvThis)
{
	// locate the delegates
	FARPROC fpIoctlDel;
	unsigned int* pnDelegates =
		(unsigned int*)Table_get(g_delegateTable, pvThis);

	if (!pnDelegates)
		return 0;
	fpIoctlDel = (FARPROC)pnDelegates[6];
	return fpIoctlDel(cmd, pvThis);
}

CURLSHIM_API int curl_shim_install_delegates(void* handle,
	void* pvThis, void* pvWriteDel, void* pvReadDel, void* pvProgDel,
	void* pvDebugDel, void* pvHeaderDel, void* pvSSLContextDel,
	void* pvIoctlDel)
{
	// cast return from GetProcAddress as a CPROC
	//CPROC pcp = pfn_curl_easy_setopt; // (CPROC)GetProcAddress(g_hModCurl, "curl_easy_setopt");

	// install all delegates through here when this works
	unsigned int* pnDelegates = malloc(7 * sizeof(unsigned int));
	pnDelegates[0] = (unsigned int)pvWriteDel;
	pnDelegates[1] = (unsigned int)pvReadDel;
	pnDelegates[2] = (unsigned int)pvProgDel;
	pnDelegates[3] = (unsigned int)pvDebugDel;
	pnDelegates[4] = (unsigned int)pvHeaderDel;
	pnDelegates[5] = (unsigned int)pvSSLContextDel;
	pnDelegates[6] = (unsigned int)pvIoctlDel;

	// add to the table (need to serialize access)
	EnterCriticalSection(&g_csDelegateTable);
	Table_put(g_delegateTable, pvThis, pnDelegates);
	LeaveCriticalSection(&g_csDelegateTable);

	// setup the callbacks from libcurl
	pfn_curl_easy_setopt(handle, CURLOPT_WRITEFUNCTION, write_callback_impl);
	pfn_curl_easy_setopt(handle, CURLOPT_WRITEDATA, pvThis);
	pfn_curl_easy_setopt(handle, CURLOPT_READFUNCTION, read_callback_impl);
	pfn_curl_easy_setopt(handle, CURLOPT_READDATA, pvThis);
	pfn_curl_easy_setopt(handle, CURLOPT_PROGRESSFUNCTION, progress_callback_impl);
	pfn_curl_easy_setopt(handle, CURLOPT_PROGRESSDATA, pvThis);
	pfn_curl_easy_setopt(handle, CURLOPT_DEBUGFUNCTION, debug_callback_impl);
	pfn_curl_easy_setopt(handle, CURLOPT_DEBUGDATA, pvThis);
	pfn_curl_easy_setopt(handle, CURLOPT_HEADERFUNCTION, header_callback_impl);
	pfn_curl_easy_setopt(handle, CURLOPT_HEADERDATA, pvThis);
	pfn_curl_easy_setopt(handle, CURLOPT_SSL_CTX_FUNCTION, ssl_ctx_callback_impl);
	pfn_curl_easy_setopt(handle, CURLOPT_SSL_CTX_DATA, pvThis);
	pfn_curl_easy_setopt(handle, CURLOPT_IOCTLFUNCTION, ioctl_callback_impl);
	pfn_curl_easy_setopt(handle, CURLOPT_IOCTLDATA, pvThis);

	return 0;
}

CURLSHIM_API void curl_shim_cleanup_delegates(void* pvThis)
{
	void* pvDelegates;
	EnterCriticalSection(&g_csDelegateTable);
	pvDelegates = Table_remove(g_delegateTable, pvThis);
	LeaveCriticalSection(&g_csDelegateTable);
	if (pvDelegates)
		free(pvDelegates);
}

static void lock_callback_impl(void* pvHandle, int data,
	int access, void* pvThis)
{
	// locate the delegates
	FARPROC fpLockDel;
	unsigned int* pnDelegates =
		(unsigned int*)Table_get(g_shareDelegateTable, pvThis);

	if (pnDelegates)
	{
		fpLockDel = (FARPROC)pnDelegates[0];
		fpLockDel(data, access, pvThis);
	}
}

static void unlock_callback_impl(void* pvHandle, int data,
	void* pvThis)
{
	// locate the delegates
	FARPROC fpUnlockDel;
	unsigned int* pnDelegates =
		(unsigned int*)Table_get(g_shareDelegateTable, pvThis);

	if (pnDelegates)
	{
		fpUnlockDel = (FARPROC)pnDelegates[1];
		fpUnlockDel(data, pvThis);
	}
}

CURLSHIM_API int curl_shim_install_share_delegates(
	void* handle, void* pvThis, void* pvLockDel, void* pvUnlockDel)
{
	// cast return from GetProcAddress as a CPROC
	//CPROC pcp = pfn_curl_share_setopt; // (CPROC)GetProcAddress(g_hModCurl, "curl_share_setopt");

	// install delegates
	unsigned int* pnDelegates = malloc(2 * sizeof(unsigned int));
	pnDelegates[0] = (unsigned int)pvLockDel;
	pnDelegates[1] = (unsigned int)pvUnlockDel;

	// add to the table, with serialized access
	EnterCriticalSection(&g_csShareDelegateTable);
	Table_put(g_shareDelegateTable, pvThis, pnDelegates);
	LeaveCriticalSection(&g_csShareDelegateTable);

	// set up the callbacks from libcurl
	pfn_curl_share_setopt(handle, CURLSHOPT_LOCKFUNC, lock_callback_impl);
	pfn_curl_share_setopt(handle, CURLSHOPT_UNLOCKFUNC, unlock_callback_impl);
	pfn_curl_share_setopt(handle, CURLSHOPT_USERDATA, pvThis);

	return 0;
}

CURLSHIM_API void curl_shim_cleanup_share_delegates(void* pvThis)
{
	void* pvDelegates;
	EnterCriticalSection(&g_csShareDelegateTable);
	pvDelegates = Table_remove(g_shareDelegateTable, pvThis);
	LeaveCriticalSection(&g_csDelegateTable);
	if (pvDelegates)
		free(pvDelegates);
}

CURLSHIM_API void curl_shim_get_file_time(
	time_t t, int* yy, int* mm, int* dd, int* hh,
	int* mn, int* ss)
{
	struct tm* ptm = localtime(&t);
	*yy = ptm->tm_year + 1900;
	*mm = ptm->tm_mon + 1;
	*dd = ptm->tm_mday;
	*hh = ptm->tm_hour;
	*mn = ptm->tm_min;
	*ss = ptm->tm_sec;
}


CURLSHIM_API int curl_shim_formadd(int* pvPosts,
	void* pvItems, int nCount)
{
#ifdef _ENV_32
	int argPairs = (nCount - 1) / 2 - 1;
	int stackFix = sizeof(int)* (nCount + 2);
	int* ppLast = &pvPosts[1];
	int* ppFirst = &pvPosts[0];
	int retVal = 0;

	// here, wer're calling a vararg function
	__asm
	{
		push CURLFORM_END; we know to be last value
			mov  ecx, argPairs; number of arg pairs in ecx
			mov  ebx, pvItems; start of args
		Args : mov  eax, [ebx + 8 * ecx + 4]; argpair->value
			   push eax; get it onto stack
			   mov  eax, [ebx + 8 * ecx]; argpair->code
			   push eax; put it on the stack
			   dec  ecx; decrement argpair counter
			   jns  Args; jump if not negative

			   push ppLast; push the last item
			   push ppFirst; and the first item

			   call pfn_curl_formadd; call curl_formadd
			   mov  retVal, eax; store the return value
			   add  esp, stackFix; fix the stack
	}
	return retVal;
#else
	return 43; //CURLE_BAD_FUNCTION_ARGUMENT
#endif
}


CURLSHIM_API void* curl_shim_alloc_fd_sets()
{
	// three contigous fd_sets: one for read, one for write,
	// and one for error
	void *pvfdSets;
	int nSize = 3 * sizeof(fd_set);
	pvfdSets = malloc(nSize);
	memset(pvfdSets, 0, nSize);
	return pvfdSets;
}

CURLSHIM_API void curl_shim_free_fd_sets(void* pvfdSets)
{
	free(pvfdSets);
}

CURLSHIM_API int curl_shim_multi_fdset(void* pvMulti,
	void* pvfdSets, int* maxFD)
{
	// cast return from GetProcAddress as a CPROC
	//CPROC pcp = pfn_curl_multi_fdset; // (CPROC)GetProcAddress(g_hModCurl, "curl_multi_fdset");
	fd_set* pfdSets = (fd_set*)pvfdSets;
	int retVal;

	FD_ZERO(&pfdSets[0]);
	FD_ZERO(&pfdSets[1]);
	FD_ZERO(&pfdSets[2]);
	retVal = pfn_curl_multi_fdset(pvMulti, &pfdSets[0], &pfdSets[1], &pfdSets[2], maxFD);
	return retVal;
}

CURLSHIM_API int curl_shim_select(int maxFD, void* pvfdSets,
	int timeoutMillis)
{
	int retVal;
	struct timeval timeout;
	//FARPROC fpSelect = pfn_select;// (FARPROC)GetProcAddress(g_hModSock, "select");
	fd_set* pfdSets = (fd_set*)pvfdSets;

	timeout.tv_sec = timeoutMillis / 1000;
	timeout.tv_usec = (timeoutMillis % 1000) * 1000;
	retVal = pfn_select(maxFD, &pfdSets[0], &pfdSets[1], &pfdSets[2], &timeout);
	return retVal;
}

CURLSHIM_API void* curl_shim_multi_info_read(void* pvHandle,
	int* nMsgs)
{
	// cast return from GetProcAddress as a CPROC
	List_T lst = NULL;
	//CPVPROC pcp = pfn_curl_multi_info_read;// (CPVPROC)GetProcAddress(g_hModCurl, "curl_multi_info_read");
	void* pvItem;
	int i, nLocalMsgs, j = 0;
	unsigned int *pnReturn = NULL;
	unsigned int *pnItem;

	*nMsgs = 0;
	while ((pvItem = pfn_curl_multi_info_read(pvHandle, &nLocalMsgs)) != NULL)
		lst = List_push(lst, pvItem);

	*nMsgs = List_length(lst);
	if (*nMsgs == 0)
		return NULL;
	pnReturn = (unsigned int*)malloc(3 * (*nMsgs) * sizeof(unsigned int));
	for (i = 0; i < (*nMsgs); i++)
	{
		lst = List_pop(lst, (void**)&pnItem);
		pnReturn[j++] = pnItem[0];
		pnReturn[j++] = pnItem[1];
		pnReturn[j++] = pnItem[2];
	}
	List_free(&lst);
	return pnReturn;
}

CURLSHIM_API void curl_shim_multi_info_free(void* pvMultiInfo)
{
	if (pvMultiInfo)
		free(pvMultiInfo);
}
