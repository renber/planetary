#include <stdlib.h>
#include "frugalHelper.h"
#include <planetary/stores/frugal.h>

void initRandomDoubleArray(double* doubleArray, int length)
{
	double current = 20.0f;
	for (int i = 0; i < length; i++)
	{
		double rand = randomNumber();
		switch (classify(rand))
		{
		case INCREASE:
			current += 0.1;
			break;
		case DECREASE:
			current -= 0.1;
			break;
		case STAY:
		default:
			break;
		}
		doubleArray[i] = current;
	}
}


double calcMedian(double* doubleArray, int length, double quantil)
{
	qsort(doubleArray, length, sizeof(double), compareDoubles);
	double median;
	if (length % 2 == 0)
		median = (doubleArray[length / 2] + doubleArray[length / 2 + 1]) / 2.0;
	else
		median = doubleArray[length / 2 + 1];

	return median;
}


int classify(double rand)
{
	if (rand > 0.95) return INCREASE;
	if (rand < 0.05) return DECREASE;
	return STAY;
}

int compareDoubles(const void* p1, const void* p2)
{
	if (*(double*)p1 < *(double*)p2) return -1;
	if (*(double*)p1 > *(double*)p2) return 1;
	return 0;
}
