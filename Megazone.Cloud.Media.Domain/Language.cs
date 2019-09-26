using System;
using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    [Serializable]
    public class Language
    {
        public Language(string code, string name)
        {
            Code = code;
            Name = name;
        }


        public string Code { get; }
        public string Name { get; }
    }
}