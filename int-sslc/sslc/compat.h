#ifndef _WIN32

#ifndef _COMPAT_H_DEFINED
#define _COMPAT_H_DEFINED

#include <string.h>
#include <errno.h>
#include <stdlib.h>
#include <unistd.h>
#include <stdio.h>
#include <stdarg.h>
#include <unistd.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <sys/types.h>
#include <dirent.h>
#include <fnmatch.h>
#include <sys/stat.h>
#include <string.h>
#include <stdlib.h>
#include <errno.h>
#include <stdio.h>
#include <stdint.h>
#include <limits.h>

#define _A_NORMAL 0x00
#define _A_SUBDIR 0x10

#define _MSC_VER


int sprintf_s(char *buffer, size_t sizeOfBuffer, const char *format, ...);
int rand_s(unsigned int *randomValue);

int strcpy_s(char* dest, size_t destsz, const char* src);
int strcat_s(char *dest, size_t destsz, const char *src);

#define _stricmp strcasecmp
#define GetCurrentProcessId() ((unsigned int)getpid())
#define _mkdir(path) mkdir((path), 0777)
#define _chdir       chdir
#define _getcwd      getcwd
#define _stat stat


#if !defined(__EMSCRIPTEN__)
typedef __time_t time_t;
#endif

typedef u_int32_t _fsize_t;


struct _finddata_t {
	unsigned attrib;
	time_t time_write;
	size_t size;
	char name[PATH_MAX];
};

typedef struct {
	DIR *dir;
	char pattern[PATH_MAX];
	char path[PATH_MAX];
} _find_handle_t;


intptr_t _findfirst(const char *pattern, struct _finddata_t *data);
int _findnext(intptr_t h, struct _finddata_t *data);
int _findclose(intptr_t h);



#endif
#endif