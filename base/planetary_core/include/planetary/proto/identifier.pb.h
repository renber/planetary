/* Automatically generated nanopb header */
/* Generated by nanopb-0.3.9.9 */

#ifndef PB_IDENTIFIER_PB_H_INCLUDED
#define PB_IDENTIFIER_PB_H_INCLUDED
#include <nanopb/pb.h>
/* @@protoc_insertion_point(includes) */
#if PB_PROTO_HEADER_VERSION != 30
#error Regenerate this file with the current version of nanopb generator.
#endif

#ifdef __cplusplus
extern "C" {
#endif

/* Struct definitions */
typedef struct _Identifier {
    char name[9];
/* @@protoc_insertion_point(struct:Identifier) */
} Identifier;

typedef struct _NodeId {
    uint16_t shortId;
/* @@protoc_insertion_point(struct:NodeId) */
} NodeId;

typedef struct _QueryId {
    uint8_t shortId;
/* @@protoc_insertion_point(struct:QueryId) */
} QueryId;

/* Default values for struct fields */

/* Initializer values for message structs */
#define Identifier_init_default                  {""}
#define QueryId_init_default                     {0}
#define NodeId_init_default                      {0}
#define Identifier_init_zero                     {""}
#define QueryId_init_zero                        {0}
#define NodeId_init_zero                         {0}

/* Field tags (for use in manual encoding/decoding) */
#define Identifier_name_tag                      1
#define NodeId_shortId_tag                       1
#define QueryId_shortId_tag                      1

/* Struct field encoding specification for nanopb */
extern const pb_field_t Identifier_fields[2];
extern const pb_field_t QueryId_fields[2];
extern const pb_field_t NodeId_fields[2];

/* Maximum encoded size of messages (where known) */
#define Identifier_size                          11
#define QueryId_size                             6
#define NodeId_size                              6

/* Message IDs (where set with "msgid" option) */
#ifdef PB_MSGID

#define IDENTIFIER_MESSAGES \


#endif

#ifdef __cplusplus
} /* extern "C" */
#endif
/* @@protoc_insertion_point(eof) */

#endif
