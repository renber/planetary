#include <planetary/stores/frugal.h>
#include <stdlib.h>

/*!
	@brief				Function to evaluate a new input with the frugal method
	@param value		The new value received
	@param structArray	The struct array with all the important frugal values
	@return				Void
*/
void writeToFrugal(float value, char* structArray) 
{		
	if ((int)value == 0x7FFF) return;
	struct FrugalCore* frugalCore = (struct FrugalCore*) structArray;
	if (frugalCore->firstNumber)
	{
		frugalCore->firstNumber = false;
		frugalCore->m = value;
		frugalCore->n++;
		return frugalCore->m;
	}
	frugalCore->n += 1;
	if (frugalCore->wasGreater)
	{
		if (value < frugalCore->m)
		{
			frugalCore->c = 0;
			frugalCore->wasGreater = false;
		}
		else
		{
			if (value > frugalCore->m)
			{
				frugalCore->c = 0;
				frugalCore->wasGreater = true;
			}
		}
	}
	frugalCore->c += 1;
	frugalCore->step = capStep(frugalCore->step);
	float rand = randomNumber();
	if (value > frugalCore->m && rand > (1.0f - frugalCore->quantil))
	{
		frugalCore->step += frugalCore->sign > 0 ? f(frugalCore->step) : -f(frugalCore->step);
		frugalCore->m += frugalCore->step > 0 ? capGrowth(frugalCore) : 1;
		if (frugalCore->m > value)
		{
			frugalCore->step += value - frugalCore->m;
		}
		if (frugalCore->sign < 0 && frugalCore->step > 1.0f)
		{
			frugalCore->step = 1.0f;
		}
		frugalCore->sign = 1;
	}
	else if (value < frugalCore->m && rand > frugalCore->quantil)
	{
		frugalCore->step += frugalCore->sign < 0 ? f(frugalCore->step) : -f(frugalCore->step);
		frugalCore->m -= frugalCore->step > 0 ? capGrowth(frugalCore) : 1;
		if (frugalCore->m < value)
		{
			frugalCore->step += frugalCore->m - value;
		}
		if (frugalCore->sign > 0 && frugalCore->step > 1.0f)
		{
			frugalCore->step = 1.0f;
		}
		frugalCore->sign = -1;
	}
}


/*!
	@brief					Function to return the current median value
	@param structArray		The struct array with all the important frugal values
	@param value			Unused
	@param index			Unused
	@param out_isFinished	Will always be set to true, since there is only one median value
	@return					The current median as a float value
*/
float readFromFrugal(char* structArray, float value, int index, bool* out_isFinished)
{
	struct FrugalCore* frugalCore = (struct FrugalCore*) structArray;
	*out_isFinished = true;
	return frugalCore->m;
}

/*!
	@brief				Function to keep the step between -0.5f < step < 0.5f
	@param step			The current step that needs to be checked
	@return				A float value which will be the new step
*/
float capStep(float step)
{
	return step > 0.5f || step < -0.5f ? 0.5f : step;
}

/*!
	@brief				Function to limit the amount the median can change, given the numbers of already analyzed values
	@param frugalCore	A pointer to a FrugalCore struct, from which all needed values will be extracted
	@return				A float value representing the change of the median
*/
float capGrowth(struct FrugalCore * frugalCore)
{
	return frugalCore->n > (frugalCore->c * 3) ? frugalCore->n / (frugalCore->n - (frugalCore->c * 3)) : frugalCore->step;
}

/*!
	@brief				Function to change the step base each value
	@param step			The current step, which is not used, but can be used
	@return				Will always return 1.0f, this can be changed though
*/
float f(float step)
{
	return 1.0f;
}

/*!
	@brief				Function to init the provided char array with the frugal struct values
	@param array		The char array which needs to be inited
	@param quantil		The quantil that the frugal algorithm is supposed to use
	@return				Void
*/
void initFrugalArray(char* array, float quantil) 
{
	struct FrugalCore* frugalCore = (struct FrugalCore*) array;
	frugalCore->m = 0;
	frugalCore->quantil = quantil;
	frugalCore->sign = 1;
	frugalCore->step = 1;
	frugalCore->c = 0;
	frugalCore->n = 0;
	frugalCore->wasGreater = true;
	frugalCore->firstNumber = true;
}

float randomNumber()
{
	return (float)rand() / (float)RAND_MAX;
}