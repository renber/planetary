#ifndef STORES_H_INCLUDED
#define STORES_H_INCLUDED

#include <planetary/typedefs.h>
#include <planetary/planetary_config.h>

#include <planetary/proto/identifier.pb.h>
#include <planetary/proto/store.pb.h>

#define StoreCollectionSize 10
#define StructArraySize 40

typedef void(*writeFunction)(float, char*);
typedef float(*readFunction)(char*, float value, int index, bool* out_isFinished);

struct StoreCore{
	char name[10];
	writeFunction writeToStore;
	readFunction readFromStore;
	char structArray[StructArraySize];
};

struct StoreCollection{
	int inStore;
	struct StoreCore collection[StoreCollectionSize];
};


void handleStoreMessage(struct StoreCollection* collection, Store* store);

void writeIntoStore(struct StoreCollection* collection, Identifier id, float param);

float readFromStore(struct StoreCollection* collection, Identifier id, float param, int index, bool* out_isFinished);

struct StoreCore createStore(CreateStore store, char* name, char* function);

void dropStore(struct StoreCollection* collection, char* name);

void addStore(struct StoreCollection* collection, struct StoreCore core);

int findStoreCoreInCollection(struct StoreCollection* collection, char* name);

void initDefaultStoreCoreCollection(struct StoreCollection * storeCollection);


#endif // STORES_H_INCLUDED
