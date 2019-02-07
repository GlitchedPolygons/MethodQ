# MethodQ
## The extremely simple and light solution to time management problems in generic C# projects (for when [Quartz.NET](https://github.com/quartznet/quartznet) is just overkill). 
You can queue any method call ([System.Action](https://docs.microsoft.com/en-us/dotnet/api/system.action?view=netframework-4.7.2)) for invocation in the future (repeating 
or non-repeating). 

* The only requirement is support for the [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)

---

Note that when scheduling method calls using the `IMethodQ.Schedule(Action, DateTime)` overload, the passed DateTime should be in UTC and **not exceed 24 days**, as that would throw an invalid timer argument exception (due to [System.Timers](https://docs.microsoft.com/en-us/dotnet/api/system.timers?view=netframework-4.7.2) using milliseconds (ms) as interval unit, [which are limited to Int32.MaxValue](https://stackoverflow.com/questions/1624789/maximum-timer-interval)).
