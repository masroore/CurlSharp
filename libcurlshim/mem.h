/**************************************************************************
 * The author of this software is David R. Hanson.
 *
 * Copyright (c) 1994,1995,1996,1997 by David R. Hanson. All Rights Reserved.
 *
 * Permission to use, copy, modify, and distribute this software for any
 * purpose, subject to the provisions described below, without fee is
 * hereby granted, provided that this entire notice is included in all
 * copies of any software that is or includes a copy or modification of
 * this software and in all copies of the supporting documentation for
 * such software.
 *
 * THIS SOFTWARE IS BEING PROVIDED "AS IS", WITHOUT ANY EXPRESS OR IMPLIED
 * WARRANTY. IN PARTICULAR, THE AUTHOR DOES MAKE ANY REPRESENTATION OR
 * WARRANTY OF ANY KIND CONCERNING THE MERCHANTABILITY OF THIS SOFTWARE OR
 * ITS FITNESS FOR ANY PARTICULAR PURPOSE.
 *
 * David Hanson / drh@microsoft.com /
 * http://www.research.microsoft.com/~drh/
 * $Id: mem.h,v 1.1 2005/02/17 22:47:24 jeffreyphillips Exp $
 **************************************************************************/

#ifndef MEM_INCLUDED
#define MEM_INCLUDED
#include "except.h"
extern const Except_T Mem_Failed;
extern void *Mem_alloc (long nbytes,
	const char *file, int line);
extern void *Mem_calloc(long count, long nbytes,
	const char *file, int line);
extern void Mem_free(void *ptr,
	const char *file, int line);
extern void *Mem_resize(void *ptr, long nbytes,
	const char *file, int line);
#define ALLOC(nbytes) \
	Mem_alloc((nbytes), __FILE__, __LINE__)
#define CALLOC(count, nbytes) \
	Mem_calloc((count), (nbytes), __FILE__, __LINE__)
#define  NEW(p) ((p) = ALLOC((long)sizeof *(p)))
#define NEW0(p) ((p) = CALLOC(1, (long)sizeof *(p)))
#define FREE(ptr) ((void)(Mem_free((ptr), \
	__FILE__, __LINE__), (ptr) = 0))
#define RESIZE(ptr, nbytes) 	((ptr) = Mem_resize((ptr), \
	(nbytes), __FILE__, __LINE__))
#endif
