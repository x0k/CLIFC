# CLIFC
Simple Command Line Interface for CLIPS

## Commands
 - `clifc` Run CLIPS in REPL
 - `clifc <file>` File to be processed
 - `clifc -f <file>[  ...[  <file>]]` Additional files to be processed.
 - `clifc <file> [-f] -w` Watcher mode, app will restart after changing files.
 - `clifc <file> [-f] -r` Reset and run after loading files.
 - `clifc --help` Display this help screen.
 - `clifc --version` Display version information.

## Additionally
To use non-constructor commands in loaded files, wrap them in curly braces.
```
(deffacts ...)
{assert(fact)}
(defrule ...)
```
To switch in REPL mode in the middle of a file load, use the '>' operator.  

```
(deffacts ...)
> 
(defrule ...)
```
To exit REPL mode, enter an empty string or end.  
