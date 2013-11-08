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
 * $Id: table.h,v 1.1 2005/02/17 22:47:24 jeffreyphillips Exp $
 **************************************************************************/

#ifndef TABLE_INCLUDED
#define TABLE_INCLUDED
#define T Table_T
typedef struct T *T;
extern T    Table_new (int hint,
	int cmp(const void *x, const void *y),
	unsigned hash(const void *key));
extern void Table_free(T *table);
extern int   Table_length(T table);
extern void *Table_put   (T table, const void *key,
	void *value);
extern void *Table_get   (T table, const void *key);
extern void *Table_remove(T table, const void *key);
extern void   Table_map    (T table,
	void apply(const void *key, void **value, void *cl),
	void *cl);
extern void **Table_toArray(T table, void *end);
#undef T
#endif
