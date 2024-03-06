#ifndef _UTILS_H_
#define _UTILS_H_

#include <planetary/typedefs.h>

#ifndef NULL
#define NULL 0
#endif // NULL

// random numbers
int irand( int a, int e);
int percrand( int perc);

bool a_contains(uint16 a[], uint8 alen, uint16 element);
bool compare_float(float f1, float f2);

#endif




