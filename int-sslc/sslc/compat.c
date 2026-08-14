#include "compat.h"

#ifndef _WIN32


// Functions in this file are written by ChatGPT
// Need carefull review

int strcpy_s(char* dest, size_t destsz, const char* src) {
	if (!dest || !src || destsz == 0) return EINVAL;
	size_t len = strlen(src);
	if (len >= destsz) {
		dest[0] = '\0';
		return ERANGE;
	}
	strcpy(dest, src);
	return 0;
}

int strcat_s(char *dest, size_t destsz, const char *src) {
	if (!dest || !src || destsz == 0) return EINVAL;

	size_t dest_len = strlen(dest);
	size_t src_len = strlen(src);

	if (dest_len + src_len + 1 > destsz) {
		dest[0] = '\0';
		return ERANGE;
	}

	strcat(dest, src);
	return 0;
}

int rand_s(unsigned int *randomValue) {
	if (!randomValue) return EINVAL;
	*randomValue = (unsigned int)rand();
	return 0;
}

int sprintf_s(char *buffer, size_t sizeOfBuffer, const char *format, ...) {
	if (!buffer || sizeOfBuffer == 0 || !format) return EINVAL;

	va_list args;
	va_start(args, format);
	int written = vsnprintf(buffer, sizeOfBuffer, format, args);
	va_end(args);

	if (written < 0 || (size_t)written >= sizeOfBuffer) {
		buffer[0] = '\0'; // mimic Windows behavior
		return ERANGE;
	}

	return 0;
}


intptr_t _findfirst(const char *pattern, struct _finddata_t *data) {
	_find_handle_t *handle = calloc(1, sizeof(_find_handle_t));
	if (!handle) return -1;

	// Split pattern into directory path and wildcard
	const char *slash = strrchr(pattern, '/');
	if (slash) {
		size_t len = (size_t)(slash - pattern + 1);
		if (len >= sizeof(handle->path)) len = sizeof(handle->path) - 1;
		strncpy(handle->path, pattern, len);
		handle->path[len] = '\0';
		strncpy(handle->pattern, slash + 1, sizeof(handle->pattern) - 1);
	} else {
		strncpy(handle->path, "./", sizeof(handle->path) - 1);
		strncpy(handle->pattern, pattern, sizeof(handle->pattern) - 1);
	}

	handle->dir = opendir(handle->path);
	if (!handle->dir) {
		free(handle);
		return -1;
	}

	struct dirent *entry;
	while ((entry = readdir(handle->dir)) != NULL) {
		if (fnmatch(handle->pattern, entry->d_name, 0) == 0) {
			struct stat st;
			char fullpath[PATH_MAX];
			snprintf(fullpath, sizeof(fullpath), "%s%s", handle->path, entry->d_name);
			if (stat(fullpath, &st) == 0) {
				strncpy(data->name, entry->d_name, sizeof(data->name) - 1);
				data->name[sizeof(data->name) - 1] = '\0';

				data->time_write = st.st_mtime;
				data->size = st.st_size;
				data->attrib = S_ISDIR(st.st_mode) ? _A_SUBDIR : _A_NORMAL;
				return (intptr_t)handle;
			}
		}
	}

	closedir(handle->dir);
	free(handle);
	return -1;
}


int _findnext(intptr_t h, struct _finddata_t *data) {
	_find_handle_t *handle = (_find_handle_t *)h;
	struct dirent *entry;

	while ((entry = readdir(handle->dir)) != NULL) {
		if (fnmatch(handle->pattern, entry->d_name, 0) == 0) {
			struct stat st;
			char fullpath[PATH_MAX];
			snprintf(fullpath, sizeof(fullpath), "%s%s", handle->path, entry->d_name);
			if (stat(fullpath, &st) == 0) {
				strncpy(data->name, entry->d_name, sizeof(data->name) - 1);
				data->name[sizeof(data->name) - 1] = '\0';

				data->time_write = st.st_mtime;
				data->size = st.st_size;
				data->attrib = S_ISDIR(st.st_mode) ? _A_SUBDIR : _A_NORMAL;
				return 0;
			}
		}
	}

	return -1;
}

int _findclose(intptr_t h) {
	_find_handle_t *handle = (_find_handle_t *)h;
	if (handle) {
		if (handle->dir) closedir(handle->dir);
		free(handle);
		return 0;
	}
	return -1;
}

#endif