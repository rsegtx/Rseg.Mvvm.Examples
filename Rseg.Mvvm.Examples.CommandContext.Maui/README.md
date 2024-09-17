# Rseg.Mvvm.Examples.CommandContext

This example demonstrates a technique for defining a common context for executing command handlers. Most handlers in a MVVM app will have boiler plate code that is common, things like exception handling, setting IsBusy on and off, logging, analytics, and possibly other processing. This code can be tedious to implement and update without a way of sharing the code across command handlers. In this example, BaseViewModel provides two overloads of PerformHandler() that can be used to execute command handling logic within a context that provides common functionality.

To review this approach, search the code for “/// Waypoint” and navigate to each waypoint and review the comments and code.