using System;
using UnityEngine;

namespace Obvious.Soap.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class AutoTag : PropertyAttribute
    {
        public string Tag { get; }
        public int TagIndex { get; }

        public AutoTag(string tag)
        {
            Tag = tag;
        }
        public AutoTag(int tagIndex)
        {
            TagIndex = tagIndex;
        }
    }
}