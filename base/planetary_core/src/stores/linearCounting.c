#include <planetary/stores/linearCounting.h>
#include <math.h>


/*
*	@brief		Function to write to the linarCounting Sketch, following the declaration specified in stores.h
*/
void writeToLinearCounting(float value, char* structArray)
{
	struct LinearCountingCore * core = (struct LinearCountingCore *)structArray;

	/*Check if the bitmap is full*/
	if (core->numberOfElements >= SIZE * 8)
	{
		core->count = evaluate(core);
		core->numberOfElements = 0;
		initBitMap(core->bitMap);
	}

	/*Hash the value and find the spot in the bitmap*/
	int hash = hashing(value);
	hash = hash % (SIZE * 8);
	int byteIndex = (int)(hash / 8);
	int bitIndex = hash % 8;
	int addressedBit = (core->bitMap[byteIndex] & (1 << bitIndex)) >> bitIndex;

	/*If the bit is not set yet, do so*/
	if (addressedBit == 0)
	{
		core->bitMap[byteIndex] |= 1 << bitIndex;
		core->numberOfElements ++;
	}
}


/*
*	@brief		Function to read from the linarCounting Sketch, following the declaration specified in stores.h
*/
float readFromLinearCounting(char* structArray, float value, int index, bool* out_isFinished)
{
	struct LinearCountingCore * core = (struct LinearCountingCore *)structArray;
	//LinearCounting only has one possible answer, so the request is finished after one result
	*out_isFinished = true;
	return evaluate(core);
}


/*
*	@brief		Function to init the charArray for LinearCounting provided by the store
*	@array		A char pointer to the begging of the char array used for the linearCounting sketch
*	@return		void
*/
void initLinearCountingArray(char* array)
{
	struct LinearCountingCore * core = (struct LinearCountingCore *)array;
	initBitMap(core->bitMap);
	core->count = 0;
	core->numberOfElements = 0;
}


/*
*	@brief		Function to evaluate the current estimation
*	@core		The LinearCountingCore which holds all the relevant information for the estimation
*	@return		The current estimation as an Integer
*/
int evaluate(struct LinearCountingCore* core)
{
	int u = calculateZeros(core);
	float v = (float)(u) / (float)(SIZE * 8);
	if (v <= 0.0f)
	{
		v = 1.0f / (float)(SIZE * 8);
	}
	int result = (int)(-(float)(SIZE * 8) * log(v) + (float)core->count);
	return result;
}


/*
*	@brief		Function to calculate how many zeros are in the bitMap
*	@core		Function core which holds the bitMap
*	@return		Amount of zeros in the bitMap
*/
int calculateZeros(struct LinearCountingCore* core)
{
	//Since numberOfElements gives us the amount of '1's in the bitMap, we can easily figure out the amount of '0's
	int zeros = SIZE * 8 - core->numberOfElements;
	return zeros;
}


/*
*	@brief		Function to set every bit in the BitMap to '0'
*	@bitMap		The map that is going to be set to '0'
*	@return		void
*/
void initBitMap(char* bitMap)
{
	for (int i = 0; i < SIZE; i++)
	{
		bitMap[i] = 0;
	}
}


/*
*	@brief		Function to hash a given float value to an int. The function simply takes to modulo to evenly distribute the input
*	@value		The value that is to be hashed
*	@return		The hash as int
*/
int hashing(float value)
{
	return (int)value % (SIZE * 8);
}