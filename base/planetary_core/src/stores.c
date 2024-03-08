#include <string.h>
#include <stdlib.h>

/*StoreCore functions*/
#include <planetary/stores/frugal.h>
#include <planetary/stores/memory.h>
#include <planetary/stores/spaceSaving.h>
#include <planetary/stores/linearCounting.h>

#include <planetary/stores.h>


/*!
	@brief				Function to handle a store message
	@param collection	The collection of stores on the arduino
	@param store		The Store Message received
	@return				Void
*/
void handleStoreMessage(struct StoreCollection* collection, Store* store) 
{
	struct StoreCore storeCore;
	switch (store->which_action) {
		case StoreType_DEFAULT:
			break;
		case StoreType_CREATE:
			storeCore = createStore(store->action.create, store->name, store->action.create.function);
			//If the store type is not known, dont create a store
			if (storeCore.writeToStore == NULL || storeCore.readFromStore == NULL)
			{
				return;
			}
			addStore(collection, storeCore);
			break;
		case StoreType_DROP:
			dropStore(collection, store->name);
		default:
			return;
	}
}

/*!
	@brief				Function to handle an incoming Selector Query with a Store as the TargetType
	@param collection	The StoreCollection collection on the node
	@param param		Yet to be defined, but needed
	@return				Yet to be determined
*/
void writeIntoStore(struct StoreCollection* collection, Identifier id, float param)
{
	int index = findStoreCoreInCollection(collection, id.name);
	if (index == -1) return;
	struct StoreCore* storeCore = &collection->collection[index];
	storeCore->writeToStore(param, storeCore->structArray);
}


/*!
*/
float readFromStore(struct StoreCollection* collection, Identifier id, float param, int index, bool* out_isFinished)
{
	int storeIndex = findStoreCoreInCollection(collection, id.name);
	if (storeIndex == -1)
	{
		*out_isFinished = true;
		return;
	}
	struct StoreCore* storeCore = &collection->collection[storeIndex];
	float result = storeCore->readFromStore(storeCore->structArray, param, index, out_isFinished);
	return result;
}


/*!
	@brief				Function to create a new StoreCore Object regarding the specified function
	@param store		The createStore Message holding important informations
	@param name			The name of the stores functionality
	@return				A new StoreCore Object for the specified function
*/
struct StoreCore createStore(CreateStore store, char* name, char* function) 
{
	struct StoreCore storeCore = { name, NULL, NULL };
	strcpy(storeCore.name, name);
	if (!strcmp(function, "frugal")) 
	{
		storeCore.writeToStore = writeToFrugal;
		storeCore.readFromStore = readFromFrugal;
		initFrugalArray(storeCore.structArray, store.param);
		return storeCore;
	}
	if (!strcmp(function, "memory")) 
	{
		storeCore.writeToStore = writeToMemory;
		storeCore.readFromStore = readFromMemory;
		initMemoryArray(storeCore.structArray);
		return storeCore;
	}
	if (!strcmp(function, "spaceSave"))
	{
		storeCore.writeToStore = writeToSpaceSaving;
		storeCore.readFromStore = readFromSpaceSaving;
		initSpaceSavingArray(storeCore.structArray);
		return storeCore;
	}
	if (!strcmp(function, "linCount"))
	{
		storeCore.writeToStore = writeToLinearCounting;
		storeCore.readFromStore = readFromLinearCounting;
		initLinearCountingArray(storeCore.structArray);
		return storeCore;

	}
	return storeCore;
}


/*!
	@brief				Function to delete a store, found by the name
	@param collection	The collection of stores on the arduino
	@param name			The name of the store that is to be dropped
	@return				Void
*/
void dropStore(struct StoreCollection* collection, char* name) 
{
	int position = findStoreCoreInCollection(collection, name);
	if (position < 0) return;
	collection->collection[position].name[0] = '\0';
	collection->inStore--;
}


/*!
	@brief				Function to find a store in a StoreCollection by name
	@param collection	The collection of stores on the arduino
	@param name			The name of the store that is to be found
	@return				The index of the found store, -1 if no store was found
*/
int findStoreCoreInCollection(struct StoreCollection* collection, char* name)
{
	for (int i = 0; i < StoreCollectionSize; i++) {
		if (!strcmp(collection->collection[i].name, name)) {
			return i;
		}
	}
	return -1;
}


/*!
	@brief				Function to add a store to a collection
	@param collection	The collection of stores on the arduino
	@param core			The new StoreCore that needs to be added
	@return				Void
*/
void addStore(struct StoreCollection* collection, struct StoreCore core) 
{
	if (collection->inStore >= StoreCollectionSize) return;
	for (int i = 0; i < StoreCollectionSize; i++) {
		//if the slot is still available, the name will be NULL
		if (collection->collection[i].name[0] == '\0') {
			collection->collection[i] = core;
			collection->inStore++;
			break;
		}
	}
}

/*!
	@brief				Function to create a new StoreCoreCollection
	@param collection	The collection of stores on the arduino
	@return				Void
*/
void initDefaultStoreCoreCollection(struct StoreCollection * storeCollection)
{
	struct StoreCore core = { '\0', NULL, NULL };
	for (int i = 0; i < StoreCollectionSize; i++) {
		storeCollection->collection[i] = core;
	}
	storeCollection->inStore = 0;
}

