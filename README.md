# puttybeam

## configure
the default session settings need to be created manually

- open putty
- select `Default Settings`
- click `Save` Button

## build
open a Developer Command Prompt

```
csc puttybeamcli.cs
```

## use

__!!! clean will delete all stored putty sessions !!!__

```
puttybeamcli.exe {store <subnet> <start> <end> <username> | clean}
```
