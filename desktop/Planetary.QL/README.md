# PLANetary Query Language

This repository contains the formal definition of the PLANetary Query Language (PLANetaryQL) in ANTLR4 format
and the generated ANTLR4 parser and lexer classes in C#.

A query in PLANetaryQL has the following basic structure

```
[SENSE sensors, aggregation functions]
[THEN]
[RAISE evt'event names]
[THEN]
[ACT actor functions (actor parameters)]
[AT sensor table name]
[WHERE condition AND/OR condition ...]
[GROUP BY sensors]
[EVERY timespan]
```