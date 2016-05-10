using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

internal static class Test
{
    public static void Async(Func<Task> test)
    {
        test().GetAwaiter().GetResult();
    }
}