﻿using System;

namespace MsSql.Adapter.Utils
{
    public class DataReaderColumnDefinition
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public int Order { get; set; }

        public DataReaderColumnDefinition(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}