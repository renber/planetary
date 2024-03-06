grammar PlanetaryQL;

/*
 * Parser Rules
 */

// IF RAISE or ACT are used without SENSE then GROUP BY cannot be used
query
	:   (act_stm (where_stm)? (at_stm)? (every_stm)?)
	  | (raise_stm (where_stm)? (at_stm)? (every_stm)?)
	  | (raise_stm (THEN act_stm)? (at_stm)? (where_stm)? (every_stm)?)
	  | (sense_stm (THEN raise_stm)? (THEN act_stm)? (at_stm)? (where_stm)? (groupby_stm)? (every_stm)?)
	  | (sense_stm into_store (at_stm)? (where_stm)?)
	  | (store_stm (at_stm)? (where_stm)?)
      EOF
    ;

into_store
	: INTO storename
	;

store_stm
	: create | drop
	;

create
	: CREATESTORE store (APPLY storefunc)
	;

drop
	: DROPSTORE store
	;

// the AT part of the SENSE statement is optional
sense_stm
    : SENSE sensorlist
	;

at_stm
	: AT table
	;

where_stm
	: WHERE conditionlist
	;

// the AT part of the ACT statement is optional
act_stm
	: ACT actorlist
	;

groupby_stm
	: GROUPBY sensor (SEPARATOR sensor)*
	;

// the AT part of the RAISE statement is optional
raise_stm
	: RAISE eventlist
	;

eventlist
	: eventdef (SEPARATOR eventdef)*
	;

eventdef
	: eventname (for_stm)?
	;

// statement to define that the query should be repeated in a given interval
every_stm
	: EVERY time (for_stm)?
	;

for_stm
	: FOR time
	;

sensorlist
  : aggregation (SEPARATOR aggregation)*
  ;

actorlist
  : actorfunc (SEPARATOR actorfunc)*
  ;

storelist
  : storename (SEPARATOR storename)*
  ;

aggregation
	: sensor | sensorfunc | storename '(' funcparam ')'
	;

conditionlist	
	: (conditiongroup)+ | ('(' conditiongroup+ ')' (condition_link conditiongroup)*)
	;

// a condition group consists of one or more condition
// linked by either AND or OR
conditiongroup
	: (condition (condition_link condition)*) | ( '(' conditiongroup (condition_link conditiongroup)* ')' )
	;

// conditions can refer to sensors or to events
condition
	: ((sensor OPERATOR NUMBER) | (NUMBER OPERATOR sensor) | (eventname) | (eventname OPERATOR NUMBER) | (NUMBER OPERATOR eventname))
	;

condition_link
	: AND | OR
	;

sensorfunc
	: func '(' sensor ')'
	;

storefunc
	: func '(' funcparam ')'
	;

// an actot function can have an arbitrary name and arbitary parameters
actorfunc
	: func '(' (funcparam (SEPARATOR funcparam)*)? ')'
	;

// an aggregation function (e.g MIN, MAX, AVG, SUM, COUNT or custom function)
func
	: IDENTIFIER
	;

funcparam
	: NUMBER
	;

sensor
	: IDENTIFIER
	;

store
	: IDENTIFIER
	;

table
	: IDENTIFIER
	;

// event names have to start with evt.
eventname
	: 'evt.' IDENTIFIER
	;

// store names have to start with store.
storename
	: 'store.' IDENTIFIER
	;

time
	: NUMBER timeunit
	;

timeunit
	: IDENTIFIER
	;

/*
 * Lexer Rules
 */


// case-insensitive definition of keywords
SENSE: S E N S E;
ACT: A C T;
AT: A T;
WHERE: W H E R E;
AND: A N D;
OR: O R;
GROUPBY: G R O U P ' ' B Y;
THEN: T H E N;
RAISE: R A I S E;
EVERY: E V E R Y;
FOR: F O R;
CREATESTORE: C R E A T E ' ' S T O R E;
DROPSTORE: D R O P ' ' S T O R E;
APPLY: A P P L Y;
INTO: I N T O;
FROM: F R O M;

SEPARATOR: ',';

// identifiers may not start with numbers
IDENTIFIER: [a-zA-Z][a-zA-Z0-9_]*;
NUMBER: [0-9]+('.'[0-9]*)?;
OPERATOR: '<' | '>' | '=' | '<=' | '>=' | '<>' | '!=';

// case-insensitive letters
fragment A : [aA];
fragment B : [bB];
fragment C : [cC];
fragment D : [dD];
fragment E : [eE];
fragment F : [fF];
fragment G : [gG];
fragment H : [hH];
fragment I : [iI];
fragment J : [jJ];
fragment K : [kK];
fragment L : [lL];
fragment M : [mM];
fragment N : [nN];
fragment O : [oO];
fragment P : [pP];
fragment Q : [qQ];
fragment R : [rR];
fragment S : [sS];
fragment T : [tT];
fragment U : [uU];
fragment V : [vV];
fragment W : [wW];
fragment X : [xX];
fragment Y : [yY];
fragment Z : [zZ];

// ignore whitespace
WS
	:	' ' -> skip
	;

// ignore newlines and tabs
NL
	: [\t\r\n] -> skip
	;