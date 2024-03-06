#ifndef PLANETARY_CONFIG_H_INCLUDED
#define PLANETARY_CONFIG_H_INCLUDED

/*
 This header filter contains all customizable parts of the PLANetary Query Framework
*/

// The maximum number of simultaneously running queries
#define MAX_RUNNING_QUERIES 5

// The maximum amount of parts in the query
#define MAX_QUERY_CONDITIONS 10
#define MAX_QUERY_SELECTS 10
#define MAX_QUERY_ACTUATORS 10

#define MAX_CONDITIONS_PER_GROUP 2
#define MAX_CHILDREN 10
#define MAX_CONDITION_GROUPS 2

// The maximum amount of result rows
// set to 1 if this is a leaf node
#define MAX_QUERY_RESULTS 10

// Indicates whether the result rows should be grouped by aggregate functions
// set to 0 if this is a leaf node to save some memory
#define ALLOW_GROUPING 1

// the maximum size of a single packet in bytes
#define MAX_PACKET_SIZE 96

// broadcast settings
#define PARENT_SELECTION_WAIT_TIME 1000
#define BROADCAST_WAIT_TIME (PARENT_SELECTION_WAIT_TIME + 1000)

// SENSOR TYPES
#define SENSOR_ID "id" // pseudo-sensor (e.g. xbee address)
#define SENSOR_TEMPERATURE "temp"
#define SENSOR_RANDNUMBER  "randno"

// ACTUATORS
#define ACTUATOR_BLINK 0
#define ACTUATOR_SOUND 1

// PACKET QUEUE
#define MAX_QUEUE_ELEMENTS 5
#define MAX_PACKET_RECEIVERS 2

// ROUTING
#define ROUTING_TABLE_SIZE 5

#endif // PLANETARY_CONFIG_H_INCLUDED
