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
 * $Id: seq.h,v 1.1 2005/02/17 22:47:24 jeffreyphillips Exp $
 **************************************************************************/

#ifndef SEQ_INCLUDED
#define SEQ_INCLUDED
#define T Seq_T
typedef struct T *T;
extern T Seq_new(int hint);
extern T Seq_seq(void *x, ...);
extern void Seq_free(T *seq);
extern int Seq_length(T seq);
extern void *Seq_get(T seq, int i);
extern void *Seq_put(T seq, int i, void *x);
extern void *Seq_addlo(T seq, void *x);
extern void *Seq_addhi(T seq, void *x);
extern void *Seq_remlo(T seq);
extern void *Seq_remhi(T seq);
#undef T
#endif
