# ![Planetary logo](https://www.tu-chemnitz.de/~berre/images/planetary_logo_small.png)  PLANetary core

The core files of the Planetary library in C for embedded, resource-limited sensor nodes.

PLANetary is a querying system for wireless sensor networks (WSN) which allows the user or services to pose queries in a SQL-like format to a WSN which are the answered by this network in an energy efficient way. This enables a high flexibility of the WSN towards changing requirements and use-cases as well as an energy-efficient oepration.

## Configuration

Configurable build options are in *planetary_config.h*. These should be adjusted according to the sensor node PLANetary should run on.

## Usage

Initialization:

```c
#include <stdio.h>
#include <stdlib.h>
#include "planetary/querytypes.h"
#include "planetary/queries.h"

void init()
{
    QueryCore pCore;
    initQueryCore(&pCore);
}
```
