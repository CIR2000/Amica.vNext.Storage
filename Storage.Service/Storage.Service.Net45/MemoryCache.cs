using System;
using System.Collections.Generic;

namespace Amica.vNext.Storage
{
	// TODO currently not used. Remove from codebase?

    internal static class MemoryCache<T>
    {
        internal static readonly List<T> DataSource = new List<T>();
        // ReSharper disable once StaticMemberInGenericType
        internal static DateTime Updated { get; set; } = DateTime.MinValue;
    }
}