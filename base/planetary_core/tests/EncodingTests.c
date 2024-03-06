#include <stddef.h>
#include <stdbool.h>

#include <nanopb/pb.h>
#include <nanopb/pb_encode.h>
#include <nanopb/pb_decode.h>

#include <planetary/proto/query.pb.h>
#include <planetary/proto/resultset.pb.h>

#include "CuTest.h"

#define TEST_BUF_SIZE 1024

void TestEmptyQueryEncDec(CuTest *tc)
{
	// create an empty query, encode and decode it
	Query query = Query_init_default;
	query.queryId.shortId = 42;

	unsigned char buf[TEST_BUF_SIZE];
	pb_ostream_t ostream = pb_ostream_from_buffer(buf, TEST_BUF_SIZE);
	if (!pb_encode(&ostream, Query_fields, &query))
		CuFail(tc, "Encoding failed");
	
	int len = ostream.bytes_written;
	Query rQuery;
	pb_istream_t istream = pb_istream_from_buffer(buf, len);
	if (!pb_decode(&istream, Query_fields, &rQuery))
		CuFail(tc, "Decoding failed");

	CuAssertIntEquals(tc, 42, rQuery.queryId.shortId);
}

void TestEmptyResultsetEncDec(CuTest *tc)
{
	Resultset set = Resultset_init_default;
	set.queryId.shortId = 42;
	set.rows_count = 0;

	unsigned char buf[TEST_BUF_SIZE];
	pb_ostream_t ostream = pb_ostream_from_buffer(buf, TEST_BUF_SIZE);
	if (!pb_encode(&ostream, Resultset_fields, &set))
		CuFail(tc, "Encoding failed");

	int len = ostream.bytes_written;
	Resultset rSet;
	pb_istream_t istream = pb_istream_from_buffer(buf, len);
	if (!pb_decode(&istream, Resultset_fields, &rSet))
		CuFail(tc, "Decoding failed");

	CuAssertIntEquals(tc, 42, rSet.queryId.shortId);
}

CuSuite* EncodingGetSuite()
{
    CuSuite* suite = CuSuiteNew();

    SUITE_ADD_TEST(suite, TestEmptyQueryEncDec);
	SUITE_ADD_TEST(suite, TestEmptyResultsetEncDec);

    return suite;
}
