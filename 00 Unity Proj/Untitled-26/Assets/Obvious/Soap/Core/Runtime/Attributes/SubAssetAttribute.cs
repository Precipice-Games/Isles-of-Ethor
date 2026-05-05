using System;
using UnityEngine;

namespace Obvious.Soap.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SubAssetAttribute : PropertyAttribute
    {
        public SubAssetAttribute()
        {
        }
    }
}