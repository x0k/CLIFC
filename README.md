# CLIFC
Simple Command Line Interface for CLIPS

## Commands
 - `clifc` Run CLIPS in REPL
 - `clifc <file>` File to be processed
 - `clifc -f <file>[  ...[  <file>]]` Additional files to be processed.
 - `clifc -w` (Default: false) Watcher mode, app will restart after changing files.
 - `clifc --help` Display this help screen.
 - `clifc --version` Display version information.

## Additionally
To use non-constructor commands in loaded files, wrap them in curly braces.
```
(deffacts ...)
{(assert(fact))}
(defrule ...)
```