using System;

namespace Zybach.API.Logging;

[AttributeUsage(AttributeTargets.Method)]
public class LogIgnoreAttribute : Attribute
{
    public LogIgnoreAttribute() { }
}