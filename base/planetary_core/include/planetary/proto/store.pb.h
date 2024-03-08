/* Automatically generated nanopb header */
/* Generated by nanopb-0.3.9.9 */

#ifndef PB_STORE_PB_H_INCLUDED
#define PB_STORE_PB_H_INCLUDED
#include <nanopb/pb.h>
/* @@protoc_insertion_point(includes) */
#if PB_PROTO_HEADER_VERSION != 30
#error Regenerate this file with the current version of nanopb generator.
#endif

#ifdef __cplusplus
extern "C" {
#endif

/* Enum definitions */
typedef enum _StoreType {
    StoreType_DEFAULT = 0,
    StoreType_CREATE = 1,
    StoreType_DROP = 2
} StoreType;
#define _StoreType_MIN StoreType_DEFAULT
#define _StoreType_MAX StoreType_DROP
#define _StoreType_ARRAYSIZE ((StoreType)(StoreType_DROP+1))

/* Struct definitions */
typedef struct _DropStore {
    char dummy_field;
/* @@protoc_insertion_point(struct:DropStore) */
} DropStore;

typedef struct _CreateStore {
    char function[10];
    float param;
/* @@protoc_insertion_point(struct:CreateStore) */
} CreateStore;

typedef struct _Store {
    pb_size_t which_action;
    union {
        CreateStore create;
        DropStore drop;
    } action;
    char name[10];
/* @@protoc_insertion_point(struct:Store) */
} Store;

/* Default values for struct fields */

/* Initializer values for message structs */
#define Store_init_default                       {0, {CreateStore_init_default}, ""}
#define CreateStore_init_default                 {"", 0}
#define DropStore_init_default                   {0}
#define Store_init_zero                          {0, {CreateStore_init_zero}, ""}
#define CreateStore_init_zero                    {"", 0}
#define DropStore_init_zero                      {0}

/* Field tags (for use in manual encoding/decoding) */
#define CreateStore_function_tag                 1
#define CreateStore_param_tag                    2
#define Store_create_tag                         1
#define Store_drop_tag                           2
#define Store_name_tag                           3

/* Struct field encoding specification for nanopb */
extern const pb_field_t Store_fields[4];
extern const pb_field_t CreateStore_fields[3];
extern const pb_field_t DropStore_fields[1];

/* Maximum encoded size of messages (where known) */
#define Store_size                               31
#define CreateStore_size                         17
#define DropStore_size                           0

/* Message IDs (where set with "msgid" option) */
#ifdef PB_MSGID

#define STORE_MESSAGES \


#endif

#ifdef __cplusplus
} /* extern "C" */
#endif
/* @@protoc_insertion_point(eof) */

#endif