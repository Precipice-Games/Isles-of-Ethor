using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Obvious.Soap
{
    //TODO: Rename to SoapTypeUtils when doing major Update
    public static class SoapUtils
    {
        private static readonly Dictionary<string, string> intrinsicToSystemTypeMap = new Dictionary<string, string>
        {
            { "byte", "System.Byte" },
            { "sbyte", "System.SByte" },
            { "char", "System.Char" },
            { "decimal", "System.Decimal" }, //not serializable by unity [DO NOT USE]. Use float or double instead.
            { "double", "System.Double" },
            { "uint", "System.UInt32" },
            { "nint", "System.IntPtr" },
            { "nuint", "System.UIntPtr" },
            { "long", "System.Int64" },
            { "ulong", "System.UInt64" },
            { "short", "System.Int16" },
            { "ushort", "System.UInt16" },
            { "int", "System.Int32" },
            { "float", "System.Single" },
            { "string", "System.String" },
            { "object", "System.Object" },
            { "bool", "System.Boolean" }
        };

        private static readonly HashSet<Type> unityTypes = new HashSet<Type>()
        {
            typeof(string), typeof(Vector4), typeof(Vector3), typeof(Vector2), typeof(Rect),
            typeof(Quaternion), typeof(Color), typeof(Color32), typeof(LayerMask), typeof(Bounds),
            typeof(Matrix4x4), typeof(AnimationCurve), typeof(Gradient), typeof(RectOffset),
            typeof(bool[]), typeof(byte[]), typeof(sbyte[]), typeof(char[]),
            typeof(double[]), typeof(float[]), typeof(int[]), typeof(uint[]), typeof(long[]),
            typeof(ulong[]), typeof(short[]), typeof(ushort[]), typeof(string[]),
            typeof(Vector4[]), typeof(Vector3[]), typeof(Vector2[]), typeof(Rect[]),
            typeof(Quaternion[]), typeof(Color[]), typeof(Color32[]), typeof(LayerMask[]), typeof(Bounds[]),
            typeof(Matrix4x4[]), typeof(AnimationCurve[]), typeof(Gradient[]), typeof(RectOffset[]),
            typeof(List<bool>), typeof(List<byte>), typeof(List<sbyte>), typeof(List<char>),
            typeof(List<double>), typeof(List<float>), typeof(List<int>), typeof(List<uint>), typeof(List<long>),
            typeof(List<ulong>), typeof(List<short>), typeof(List<ushort>), typeof(List<string>),
            typeof(List<Vector4>), typeof(List<Vector3>), typeof(List<Vector2>), typeof(List<Rect>),
            typeof(List<Quaternion>), typeof(List<Color>), typeof(List<Color32>), typeof(List<LayerMask>),
            typeof(List<Bounds>),
            typeof(List<Matrix4x4>), typeof(List<AnimationCurve>), typeof(List<Gradient>), typeof(List<RectOffset>),
            typeof(Vector3Int), typeof(Vector2Int), typeof(RectInt), typeof(BoundsInt),
            typeof(Vector3Int[]), typeof(Vector2Int[]), typeof(RectInt[]), typeof(BoundsInt[]),
            typeof(List<Vector3Int>), typeof(List<Vector2Int>), typeof(List<RectInt>), typeof(List<BoundsInt>),
        };

        private static readonly HashSet<string> unityTypesAsString = new HashSet<string>()
        {
            "string", "Vector4", "Vector3", "Vector2", "Rect",
            "Quaternion", "Color", "Color32", "LayerMask", "Bounds",
            "Matrix4x4", "AnimationCurve", "Gradient", "RectOffset",
            "GameObject", "Transform", "Material", "Texture", "Sprite",
            "AudioClip", "Mesh", "Shader", "Font", "TextAsset",
            "Rigidbody", "Rigidbody2D", "Camera", "Light", "AudioSource",
            "Vector2Int", "Vector3Int", "Collider", "Collider2D", "BoundsInt",
            "RectInt", "MeshRenderer", "SpriteRenderer", "BoxCollider",
            "BoxCollider2D", "CircleCollider2D", "PolygonCollider2D", "CapsuleCollider",
            "CapsuleCollider2D", "MeshCollider", "MeshCollider2D", "SphereCollider",
            "SphereCollider2D", "TerrainCollider", "WheelCollider", "WheelCollider2D",
            "CharacterController", "CharacterJoint", "ConfigurableJoint", "FixedJoint",
            "HingeJoint", "SpringJoint", "SliderJoint", "TargetJoint2D",
            "AnimationCurve", "AnimationClip", "Animator", "AnimatorController",
        };


        public static bool IsIntrinsicType(string typeName)
        {
            if (intrinsicToSystemTypeMap.TryGetValue(typeName.ToLower(), out var qualifiedName))
                typeName = qualifiedName;

            Type type = Type.GetType(typeName);
            if (type?.Namespace != null && type.Namespace.StartsWith("System"))
                return true;

            return false;
        }

        public static string GetIntrinsicType(string systemType)
        {
            return intrinsicToSystemTypeMap.FirstOrDefault(x =>
                x.Value == $"System.{systemType}").Key ?? systemType;
        }

        public static bool IsUnityType(Type type) => unityTypes.Contains(type);
        public static bool IsUnityType(string type) => unityTypesAsString.Contains(type);

        internal static bool CanBeCreated(string typeName)
        {
            if (IsIntrinsicType(typeName) || IsUnityType(typeName))
                return false;
            return true;
        }
        
        public static bool IsSerializable(Type type)
        {
            if (type == null)
                return false;
            
            if (typeof(UnityEngine.Object).IsAssignableFrom(type) || type.IsEnum)
                return true;
      
            if (type.IsArray)
            {
                //dont support multi-dimensional arrays
                if (type.GetArrayRank() != 1)
                    return false;
                return IsSerializable(type.GetElementType());
            }
            
            if (type.IsGenericType)
            {
                var genericDefinition = type.GetGenericTypeDefinition();
                if (genericDefinition == typeof(Nullable<>)
                    || genericDefinition == typeof(List<>))
                {
                    return IsSerializable(type.GetGenericArguments()[0]);
                }
                
                // Generic types are allowed on 2020.1 and later
#if !UNITY_2020_1_OR_NEWER
                return false;
#endif
            }

            return Attribute.IsDefined(type, typeof(SerializableAttribute), false);
        }

        // public static bool IsSerializableLazy(Type type)
        // {
        //     var isSerializable = false;
        //     isSerializable |= type.IsSerializable;
        //     isSerializable |= type.Namespace == "UnityEngine";
        //     isSerializable |= type.IsSubclassOf(typeof(MonoBehaviour));
        //     return isSerializable;
        // }
  
        internal static Type GetTypeByName(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (string.Equals(type.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        public static bool IsCollection(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsArray)
                return true;

            if (type.IsGenericType && type.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                return true;

            // Check if it's a non-generic collection
            if (typeof(IEnumerable).IsAssignableFrom(type))
                return true;

            return false;
        }
        
        public static bool InheritsFromOpenGeneric(Type type, Type openGenericBase)
        {
            if (type == null || openGenericBase == null) return false;

            // walk class hierarchy
            for (var t = type; t != null && t != typeof(object); t = t.BaseType)
            {
                var candidate = t.IsGenericType ? t.GetGenericTypeDefinition() : t;
                if (candidate == openGenericBase)
                    return true;
            }

            // optionally check interfaces as well (useful if your base is an interface)
            if (openGenericBase.IsInterface)
            {
                foreach (var itf in type.GetInterfaces())
                {
                    var candidate = itf.IsGenericType ? itf.GetGenericTypeDefinition() : itf;
                    if (candidate == openGenericBase)
                        return true;
                }
            }

            return false;
        }
    }
}