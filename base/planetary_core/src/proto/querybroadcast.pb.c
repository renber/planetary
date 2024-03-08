/* Automatically generated nanopb constant definitions */
/* Generated by nanopb-0.3.9.9 */

#include <planetary/proto/querybroadcast.pb.h>
/* @@protoc_insertion_point(includes) */
#if PB_PROTO_HEADER_VERSION != 30
#error Regenerate this file with the current version of nanopb generator.
#endif



const pb_field_t QueryBroadcast_fields[5] = {
    PB_FIELD(  1, MESSAGE , SINGULAR, STATIC  , FIRST, QueryBroadcast, parentId, parentId, &NodeId_fields),
    PB_FIELD(  2, UINT32  , SINGULAR, STATIC  , OTHER, QueryBroadcast, distanceToSink, parentId, 0),
    PB_FIELD(  3, BOOL    , SINGULAR, STATIC  , OTHER, QueryBroadcast, parentIsPartOfQuery, distanceToSink, 0),
    PB_FIELD(  4, MESSAGE , SINGULAR, STATIC  , OTHER, QueryBroadcast, query, parentIsPartOfQuery, &Query_fields),
    PB_LAST_FIELD
};


/* Check that field information fits in pb_field_t */
#if !defined(PB_FIELD_32BIT)
/* If you get an error here, it means that you need to define PB_FIELD_32BIT
 * compile-time option. You can do that in pb.h or on compiler command line.
 *
 * The reason you need to do this is that some of your messages contain tag
 * numbers or field sizes that are larger than what can fit in 8 or 16 bit
 * field descriptors.
 */
PB_STATIC_ASSERT((pb_membersize(QueryBroadcast, parentId) < 65536 && pb_membersize(QueryBroadcast, query) < 65536), YOU_MUST_DEFINE_PB_FIELD_32BIT_FOR_MESSAGES_QueryBroadcast)
#endif

#if !defined(PB_FIELD_16BIT) && !defined(PB_FIELD_32BIT)
#error Field descriptor for QueryBroadcast.query is too large. Define PB_FIELD_16BIT to fix this.
#endif


/* @@protoc_insertion_point(eof) */
