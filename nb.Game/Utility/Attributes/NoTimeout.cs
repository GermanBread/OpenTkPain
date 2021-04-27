// System
using System;
using System.Diagnostics;

namespace nb.Game.Utility.Attributes
{
    /// <summary>
    /// Prevents this method from triggering the timeout message. Only applies for toplevel methods inside a class.
    /// </summary>
    public class NoTimeout : Attribute { }
}