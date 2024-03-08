#include <planetary/stores/spaceSaving.h>
#include <stdlib.h>


/*
*	@brief		Function to write to the spaceSaving Sketch, following the declaration specified in stores.h
*/
void writeToSpaceSaving(float value, char* structArray)
{
	if ((int)value == 0x7FFF) return;
	struct SpaceSavingCore * core = (struct SpaceSavingCore *)structArray;
	int newElement = (int)value;
	int index = findSlot(core->slots, newElement);
	overrideAtIndex(core->slots, newElement, index);
}


/*
*	@brief		Function to read from the spaceSaving Sketch, following the declaration specified in stores.h
*/
float readFromSpaceSaving(char* structArray, float value, int index, bool* out_isFinished)
{
	//If the value parameter is negative, an ordered request is processed. If positive, an unordered request
	bool ordered = value < 0 ? true : false;
	value = abs(value);
	struct SpaceSavingCore * core = (struct SpaceSavingCore *)structArray;
	qsort(core->slots, SLOTS, sizeof(struct SpaceSavingSlot), comparator);

	//k is the amount of elements wanted, max = all elements
	int k = (int)value > SLOTS ? SLOTS : (int)value;
	//If index >= k-1, the next bigger index will ask for a value outside of the result set. Therefore the query is finished after this iteration
	if (index >= k - 1)
	{
		*out_isFinished = true;
	}
	int i = index;
	bool eval = false;
	bool errorZero = core->slots[i].error == 0;
	int ith = core->slots[i].amount - core->slots[i].error;
	if (ordered)
	{
		eval = compareToIndex(ith, i + 1, core, errorZero);
	}
	else
	{
		eval = compareToIndex(ith, k, core, errorZero);
	}
	if (eval) return core->slots[i].element;
	*out_isFinished = true;
	return 0x7FFF;
}


/*
*	@brief		Function to check if the current slot is still in the result set
*	@ith		Index of the slot in question
*	@index		Index of the element the ith is compared to
*	@core		The SpaceSavingCore, needed for the comparison
*	@errorIsZero	Param that specifies, if the error of the ith slot is zero
*	@return		true, if the ith should be in the result set, false otehrweise
*/
bool compareToIndex(int ith, int index, struct SpaceSavingCore * core, bool errorIsZero)
{
	index = index >= SLOTS ? SLOTS - 1 : index;
	if(index == ith)
	{
		return true;
	}
	if (errorIsZero)
	{
		return ith >= core->slots[index].amount;
	}
	else
	{
		return ith > core->slots[index].amount;
	}
}


/*
*	@brief		Function to init the charArray for SpaceSaving provided by the store
*	@array		A char pointer to the begging of the char array used for the SpaceSaving sketch
*	@return		void
*/
void initSpaceSavingArray(char* array)
{
	struct SpaceSavingCore * core = (struct SpaceSavingCore *)array;
	for (int i = 0; i < SLOTS; i++)
	{
		struct SpaceSavingSlot * slot = &core->slots[i];
		slot->amount = 0;
		slot->error = 0;
		slot->element = 0x7FFF;	//max value for a 16 bit int
	}
}


/*
*	@brief		Function to find the best slot for a newElement of a dataStream
*	@array		The slot array from which a slot has to be selected
*	@newElement	The new element of the data stream
*	return		The index of the slot that has been selected, as int
*/
int findSlot(struct SpaceSavingSlot * array, int newElement)
{
	int bestIndex = 0;

	for (int i = 0; i < SLOTS; i++)
	{
		if (array[i].element == newElement)
		{
			return i;
		}

		if (array[bestIndex].amount < array[i].amount)
		{
			continue;
		}
		else if (array[bestIndex].amount > array[i].amount)
		{
			bestIndex = i;
			continue;
		}
		else
		{
			if (array[bestIndex].error > array[i].error)
			{
				continue;
			}
			else if (array[bestIndex].error < array[i].error)
			{
				bestIndex = i;
				continue;
			}
			else
			{
				if (array[bestIndex].element < array[i].element)
				{
					continue;
				}
				else if (array[bestIndex].element > array[i].element)
				{
					bestIndex = i;
					continue;
				}
			}
		}
	}
	return bestIndex;
}


/*
*	@brief		Function to write an element into a SpaceSavingSlot
*	@array		The array of SpaceSavingSlots
*	@newElement	The new element that has to be included in the slots
*	@index		The index of the array, that the new element is written into
*	@return		void
*/
void overrideAtIndex(struct SpaceSavingSlot * array, int newElement, int index)
{
	struct SpaceSavingSlot* slot = &array[index];

	//If the element is already in the slot, just increase the counter
	if (slot->element == newElement)
	{
		slot->amount += 1;
	}
	//If it is not in the slot, overwrite the slot
	else
	{
		slot->element = newElement;
		slot->error = slot->amount;
		slot->amount += 1;
	}
}


/*
*	@brief		Function to compare to spaceSavingSlots, used for the Quicksort algorithm. 
*/
int comparator(const void * v1, const void * v2)
{
	const struct SpaceSavingSlot * slot1 = (struct SpaceSavingSlot *)v1;
	const struct SpaceSavingSlot * slot2 = (struct SpaceSavingSlot *)v2;

	if (slot1->amount > slot2->amount)
	{
		return -1;
	}
	else if (slot2->amount > slot1->amount)
	{
		return 1;
	}
	else
	{
		if (slot1->error < slot2->error)
		{
			return -1;
		}
		else if(slot2->error < slot1->error)
		{
			return 1;
		}
		else
		{
			if (slot1->element > slot2->element)
			{
				return -1;
			}
			else return 1;
		}
	}
}