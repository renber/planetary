/* Automatically generated nanopb header */
/* Generated by nanopb-0.3.9.9 */

#ifndef PB_RESULTSET_PB_H_INCLUDED
#define PB_RESULTSET_PB_H_INCLUDED
#include <nanopb/pb.h>
#include <planetary/proto/identifier.pb.h>
/* @@protoc_insertion_point(includes) */
#if PB_PROTO_HEADER_VERSION != 30
#error Regenerate this file with the current version of nanopb generator.
#endif

#ifdef __cplusplus
extern "C" {
#endif

/* Struct definitions */
typedef struct _ResultRow {
    uint32_t numberOfNodes;
    pb_size_t values_count;
    float values[10];
/* @@protoc_insertion_point(struct:ResultRow) */
} ResultRow;

typedef struct _Resultset {
    QueryId queryId;
    pb_size_t rows_count;
    ResultRow rows[20];
/* @@protoc_insertion_point(struct:Resultset) */
} Resultset;

/* Default values for struct fields */

/* Initializer values for message structs */
#define Resultset_init_default                   {QueryId_init_default, 0, {ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default, ResultRow_init_default}}
#define ResultRow_init_default                   {0, 0, {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}}
#define Resultset_init_zero                      {QueryId_init_zero, 0, {ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero, ResultRow_init_zero}}
#define ResultRow_init_zero                      {0, 0, {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}}

/* Field tags (for use in manual encoding/decoding) */
#define ResultRow_numberOfNodes_tag              1
#define ResultRow_values_tag                     2
#define Resultset_queryId_tag                    1
#define Resultset_rows_tag                       2

/* Struct field encoding specification for nanopb */
extern const pb_field_t Resultset_fields[3];
extern const pb_field_t ResultRow_fields[3];

/* Maximum encoded size of messages (where known) */
#define Resultset_size                           (1166 + QueryId_size)
#define ResultRow_size                           56

/* Message IDs (where set with "msgid" option) */
#ifdef PB_MSGID

#define RESULTSET_MESSAGES \


#endif

#ifdef __cplusplus
} /* extern "C" */
#endif
/* @@protoc_insertion_point(eof) */

#endif
