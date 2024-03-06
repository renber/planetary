#include <stdlib.h>
#include <planetary/utils.h>

// get a random number x with a <= x <= e
int irand( int a, int e)
{
	double r = e - a + 1;
	return a + (int)(r * rand()/(RAND_MAX+1.0));
}

// returns true in perc % of its calls
int percrand( int perc) {
	return perc >= irand(1, 100);
}

// returns true if the given array contains the element
bool a_contains(uint16 a[], uint8 alen, uint16 element)
{
  int i;
  for(i = 0; i < alen; i++)
   if (a[i] == element)
    return true;

  return false;
}

//compares if the float f1 is equal to f2 and returns 1 if true and 0 if false
bool compare_float(float f1, float f2)
{
    float precision = 0.00001;
    if (((f1 - precision) < f2) && ((f1 + precision) > f2))
    {
        return true;
    }
    else
    {
        return false;
    }
}
