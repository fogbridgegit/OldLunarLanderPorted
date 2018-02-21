﻿//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
#if UNITY_WINRT && !UNITY_EDITOR
#define USE_WINRT
#endif

using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;
using HoloToolkit.Unity;
using WWIToolkit.Unity.Utility;

//using HUX.Utility;

// You can create a custom attribute type easily by doing this:
//
//	public class TestAttribute : CustomAttributeBase
//	{
//		public TestAttribute() { }
//		public new static void RegisterTypeWithManager()
//		{
//			AttributeCacheManager.Instance.RegisterCustomAttributeType<TestAttribute>();
//		}
//	}
//
// You can provide additional constructors, and add additional members as needed.
// Here's how to query custom attributes:
//
//  List<TestAttribute> atts = AttributeCacheManager.Instance.GetAttributes<TestAttribute>();
//

namespace WWIToolkit.Unity.Debug
{
    public class AttributeCacheManager : Singleton<AttributeCacheManager>
    {
        // List of loaded assemblies so we don't process them more than once
        static List<Assembly> m_loadedAssemblies = new List<Assembly>();

        // Map of custom attribute types to their attribute cache
        Dictionary<Type, AttributeCacheBase> m_attributeCaches = new Dictionary<Type, AttributeCacheBase>();

        // List of all declaring types.  Currently unused
        public List<Type> m_allDeclaringTypes = new List<Type>();

        // Map of declaring types to their instances
        public Dictionary<Type, UnityEngine.Object[]> m_instances = new Dictionary<Type, UnityEngine.Object[]>();

        // Pretty much override use of Singleton<>, provide a custom setting which automatically creates instance if not found
        private static AttributeCacheManager _instance;
        public new static AttributeCacheManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AttributeCacheManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AttributeCacheManager");
                        _instance = go.AddComponent<AttributeCacheManager>();
                    }
                }
                if (_instance == null)
                {
                    UnityEngine.Debug.LogWarning("AttributeCacheManager failed to find or create instance!");
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        // We have to be careful that we process all assemblies and have all the custom attribute types known.
        // We can process assemblies when we learn about a new custom attribute, or vice versa.  Or we can try
        // to guarantee all all of one is ready and then process.  We'll see.
        protected override void Awake()
        {
            base.Awake();

            // Gather custom attribute types first (hopefully they're all loaded)
            FindCustomAttributeTypes();

            // Process loaded assemblies (hopefully they're all loaded)
            LoadAssemblyAttributes();

            // Build unique list of declaring types
            CacheDeclaringTypes();
        }

        void Start()
        {
            StartCoroutine(CoCacheInstancesLoop());
        }

        // Returns the list of cached custom attributes of a given type
        public List<TAttribute> GetAttributes<TAttribute>() where TAttribute : CustomAttributeBase
        {
            if (m_attributeCaches.ContainsKey(typeof(TAttribute)))
            {
                AttributeCache<TAttribute> cache = m_attributeCaches[typeof(TAttribute)] as AttributeCache<TAttribute>;
                if (cache != null)
                {
                    return cache.GetTAttributes();
                }
            }
            return null;
        }

        // Load custom attribute types (from the assemblies we have)
        public void FindCustomAttributeTypes()
        {
            Assembly[] assemblies = Instance.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
#if USE_WINRT
			foreach (TypeInfo typeInfo in assembly.DefinedTypes)
			{
				if (typeInfo.IsSubclassOf(typeof(CustomAttributeBase)) && !typeInfo.Equals(typeof(CustomAttributeBase)))
				{
					MethodInfo mi = typeInfo.AsType().GetMethod("RegisterTypeWithManager");
					if (mi != null)
					{
						mi.Invoke(null, null);
					}
				}
			}
#else
                Type[] types = assembly.GetExportedTypes();
                foreach (Type type in types)
                {
                    if (type.IsSubclassOf(typeof(CustomAttributeBase)) && type != typeof(CustomAttributeBase))
                    {
                        MethodInfo mi = type.GetMethod("RegisterTypeWithManager");
                        if (mi != null)
                        {
                            mi.Invoke(null, null);
                        }
                    }
                }
#endif
            }
        }

        // Invoked by CustomAttributeBase.RegisterTypeWithManager
        public void RegisterCustomAttributeType<TAttribute>() where TAttribute : CustomAttributeBase
        {
            if (!m_attributeCaches.ContainsKey(typeof(TAttribute)))
            {
                m_attributeCaches[typeof(TAttribute)] = new AttributeCache<TAttribute>();
            }
        }

        // Get known assemblies
        Assembly[] GetAssemblies()
        {
            Assembly[] assemblies = null;
#if USE_WINRT
		// Load only the known assemblies...
		assemblies = new Assembly[1];
		assemblies[0] = typeof(AttributeCacheManager).GetTypeInfo().Assembly;
#else
            // This logic seems to be safe as all assemblies generated by Unity use this prefix
            // If Unity changes this logic the additional check should help warn about that.
            List<Assembly> asmList = new List<Assembly>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("Assembly-"))
                {
                    asmList.Add(assembly);
                }
            }

            assemblies = asmList.ToArray();
#endif
            if (assemblies.Length == 0)
            {
                UnityEngine.Debug.LogError("No valid assemblies were found when looking for custom attributes.  Did the Unity naming convention change?");
            }
            return assemblies;
        }

        // Looks for Unity specific assemblies to load attributes from.
        public static void LoadAssemblyAttributes()
        {
            Assembly[] assemblies = Instance.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Instance.CacheAttributesOnce(assembly);
            }
        }

        // Caches tagged methods and fields only if the assembly hasn't been processed yet
        public bool CacheAttributesOnce(Assembly assembly)
        {
            if (assembly == null)
            {
                return false;
            }

            if (m_loadedAssemblies.Contains(assembly))
            {
                return true;
            }

            m_loadedAssemblies.Add(assembly);
            //Debug.Log("Loading Console methods from assembly: " + assembly.FullName);

            CacheAttributes(assembly);

            return true;
        }

        // Finds all methods and fields and lets AttributeCaches process them
        void CacheAttributes(Assembly assembly)
        {
#if USE_WINRT
		foreach (TypeInfo typeInfo in assembly.DefinedTypes)
		{
			foreach (MethodInfo methodInfo in typeInfo.DeclaredMethods)
			{
				ProcessMemberInfo(methodInfo, null);		
			}

			foreach (FieldInfo fieldInfo in typeInfo.DeclaredFields)
			{
				ProcessMemberInfo(null, fieldInfo);
			}
		}		
#else
            Type[] types = assembly.GetExportedTypes();
            foreach (Type type in types)
            {
                MethodInfo[] typeMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (MethodInfo methodInfo in typeMethods)
                {
                    ProcessMemberInfo(methodInfo, null);
                }

                FieldInfo[] typeFields = type.GetFields();
                foreach (FieldInfo fieldInfo in typeFields)
                {
                    ProcessMemberInfo(null, fieldInfo);
                }
            }
#endif
        }

        public void ProcessMemberInfo(MethodInfo methodInfo, FieldInfo fieldInfo)
        {
            foreach (KeyValuePair<Type, AttributeCacheBase> pair in m_attributeCaches)
            {
                pair.Value.ProcessMemberInfo(methodInfo, fieldInfo);
            }
        }

        // Stores all declaring types for all found custom attributes
        // Sets up the m_instances map for attribute types which request instances
        void CacheDeclaringTypes()
        {
            m_allDeclaringTypes = new List<Type>();
            m_instances = new Dictionary<Type, UnityEngine.Object[]>();
            foreach (KeyValuePair<Type, AttributeCacheBase> attPair in m_attributeCaches)
            {
                List<KeyValuePair<Type, bool>> attDeclaringTypes = attPair.Value.GetDeclaringTypes();

                foreach (KeyValuePair<Type, bool> typePair in attDeclaringTypes)
                {
                    // Store all declaring types uniquely
                    if (!m_allDeclaringTypes.Contains(typePair.Key))
                    {
                        m_allDeclaringTypes.Add(typePair.Key);
                    }

                    // The custom attribute requests instances of its declaring type to be cached
                    if (typePair.Value)
                    {
                        if (!m_instances.ContainsKey(typePair.Key))
                        {
                            m_instances[typePair.Key] = new UnityEngine.Object[0];
                        }
                    }
                }
            }

            UnityEngine.Debug.Log("all decl types: " + m_allDeclaringTypes.Count);
            UnityEngine.Debug.Log("all instance types: " + m_instances.Count);
        }

        // Cache instances for the attributes which requested it
        IEnumerator CoCacheInstancesLoop()
        {
            while (true)
            {
                // Get a list of the types
                List<Type> types = new List<Type>(m_instances.Keys);
                List<Type>.Enumerator iter = types.GetEnumerator();

                // Use an amortized task since this could be somewhat expensive
                //Debug.Log("About to process...");
                YieldInstruction result = Amortizer.Process(gameObject, "CoCacheInstances", 2f, () =>
                {
                    if (iter.MoveNext())
                    {
                        m_instances[iter.Current] = GameObject.FindObjectsOfType(iter.Current);
                    //Debug.Log("Found " + m_instances[iter.Current].Length + " instances for " + iter.Current);

                    // This may kind of estimate the amount of actual work being done. Maybe.
                    return m_allDeclaringTypes.Count + m_instances[iter.Current].Length;
                    }

                //Debug.Log("Iter finished");
                return -1;

                });

                yield return result;

                // Apply cached instances to the attributes
                foreach (KeyValuePair<Type, AttributeCacheBase> pair in m_attributeCaches)
                {
                    pair.Value.UpdateInstances();
                }

                //Debug.Log("Waiting for 1 seconds");
                yield return new WaitForSeconds(1f);
            }
        }

        /*public UnityEngine.Object[] GetInstances(Type type)
        {
            if (m_instances.ContainsKey(type))
            {
                return m_instances[type];
            }
            return new UnityEngine.Object[0];
        }*/

#if UNITY_EDITOR
        static bool _processMemberInfoEditor<T>(ref List<T> atts, MethodInfo methodInfo, FieldInfo fieldInfo) where T : CustomAttributeBase
        {
            bool added = false;
            bool isMethod = methodInfo != null;
            MemberInfo memberInfo = isMethod ? methodInfo : fieldInfo as MemberInfo;

            // See if the custom attribute exists on the method or field
            T attribute = null;
            object[] customAttributes = memberInfo.GetCustomAttributes(typeof(T), true);
            if (customAttributes.Length > 0)
            {
                attribute = (T)customAttributes[0];
            }

            // Try to add the attribute
            if (attribute != null)
            {
                bool isStatic = isMethod ? methodInfo.IsStatic : fieldInfo.IsStatic;

                // Only if not already added
                if (atts.Find(t => t.memberInfo == memberInfo) == null)
                {
                    if (isStatic || typeof(UnityEngine.Object).IsAssignableFrom(memberInfo.DeclaringType))
                    {
                        attribute.SetInfo(memberInfo, methodInfo, fieldInfo);
                        atts.Add(attribute);
                        added = true;
                    }
                }
            }
            return added;
        }

        static Assembly[] _getAttsEditorAssemblies = null;
        static int _getAttsEditorCurrentAssembly;

        // Editor helpers
        public static bool GetAttributesEditor<T>(bool init, ref List<T> result) where T : CustomAttributeBase
        {
            bool added = false;
            if (init)
            {
                _getAttsEditorAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                _getAttsEditorCurrentAssembly = 0;
            }
            else
            {
                if (_getAttsEditorAssemblies != null)
                {
                    while (_getAttsEditorCurrentAssembly < _getAttsEditorAssemblies.Length)
                    {
                        Assembly A = _getAttsEditorAssemblies[_getAttsEditorCurrentAssembly];

                        // Skip assemblies that don't start with 'Assembly-', these are the user scripts assemblies.  Major speedup
                        if (!A.FullName.StartsWith("Assembly-"))
                        {
                            ++_getAttsEditorCurrentAssembly;
                            continue;
                        }

                        Type[] types = A.GetExportedTypes();
                        foreach (Type type in types)
                        {
                            MethodInfo[] typeMethods = type.GetMethods();
                            foreach (MethodInfo methodInfo in typeMethods)
                            {
                                added |= _processMemberInfoEditor<T>(ref result, methodInfo, null);
                            }

                            FieldInfo[] typeFields = type.GetFields();
                            foreach (FieldInfo fieldInfo in typeFields)
                            {
                                added |= _processMemberInfoEditor<T>(ref result, null, fieldInfo);
                            }
                        }
                        ++_getAttsEditorCurrentAssembly;
                        break;
                    }
                }
            }
            UnityEngine.Debug.Log("Caching: " + (_getAttsEditorCurrentAssembly) + " of: " + _getAttsEditorAssemblies.Length + ". Added: " + added);
            if (added)
            {
                UnityEngine.Debug.Log("  " + _getAttsEditorAssemblies[_getAttsEditorCurrentAssembly - 1].GetName());
            }
            return _getAttsEditorCurrentAssembly < _getAttsEditorAssemblies.Length - 1;
        }
#endif
    }


    // Base class for custom attributes!
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class CustomAttributeBase : Attribute
    {
        public MemberInfo memberInfo;
        public MethodInfo methodInfo;
        public FieldInfo fieldInfo;

        public bool bCacheInstances;
        public event Action<CustomAttributeBase, UnityEngine.Object> OnInstanceCreated;
        public event Action<CustomAttributeBase> OnInstancesMaybeRemoved;

        // If enabled by bCacheInstances, contains all instances of the owning type
        public UnityEngine.Object[] instances = new UnityEngine.Object[0];

        public CustomAttributeBase()
        {
            LoadAssembly();
        }

        public virtual void SetInfo(MemberInfo _memberInfo, MethodInfo _methodInfo, FieldInfo _fieldInfo)
        {
            memberInfo = _memberInfo;
            methodInfo = _methodInfo;
            fieldInfo = _fieldInfo;

            LoadAssembly();
        }

        public string GetMemberName()
        {
            return memberInfo.Name;
        }

        public string GetMemberNameFull()
        {
            return GetDeclaringType().ToString() + "." + GetMemberName();
        }

        public Type GetDeclaringType()
        {
            if (memberInfo != null)
            {
                return memberInfo.DeclaringType;
            }
            return null;
        }

        public bool IsField()
        {
            return fieldInfo != null;
        }

        public bool IsMethod()
        {
            return methodInfo != null;
        }

        public bool IsStatic()
        {
            if (fieldInfo != null)
            {
                return fieldInfo.IsStatic;
            }
            else if (methodInfo != null)
            {
                return methodInfo.IsStatic;
            }
            return false;
        }

        public void UpdateInstances()
        {
            if (bCacheInstances)
            {
                //Debug.Log("Updating instances!  Current: " + instances.Length);
                UnityEngine.Object[] newInstances = AttributeCacheManager.Instance.m_instances[memberInfo.DeclaringType];
                //Debug.Log("  New: " + newInstances.Length);

                // If the instance callbacks have been subscribed to, we need to compare the lists and execute the events accordingly
                if (OnInstanceCreated != null)
                {
                    // Look for new objects
                    foreach (UnityEngine.Object obj in newInstances)
                    {
                        bool bFound = false;
                        for (int i = 0; i < instances.Length; ++i)
                        {
                            if (obj == instances[i])
                            {
                                bFound = true;
                                break;
                            }
                        }

                        if (!bFound)
                        {
                            OnInstanceCreated(this, obj);
                        }
                    }
                }

                // Check if instances have been removed, if there's a subscriber to the callback
                bool itemsRemoved = false;
                if (OnInstancesMaybeRemoved != null)
                {
                    // First, the easy check
                    if (newInstances.Length < instances.Length)
                    {
                        itemsRemoved = true;
                    }
                    else
                    {
                        // Compare elements.  More new instances than old ones.
                        // It's not for sure if anything has been removed, but it's a good bet
                        for (int i = 0; i < instances.Length; ++i)
                        {
                            if (instances[i] != newInstances[i])
                            {
                                itemsRemoved = true;
                                break;
                            }
                        }
                    }
                }

                instances = newInstances;

                // Now that instances may have been removed, fire the event
                if (OnInstancesMaybeRemoved != null && itemsRemoved)
                {
                    OnInstancesMaybeRemoved(this);
                }
            }
        }

        public static bool IsEnum(Type type)
        {
#if UNITY_EDITOR || !UNITY_METRO
            return type.IsEnum;
#else
		// This might be for device. Conditionally compile whichever works
		return type.GetTypeInfo().IsEnum;
#endif
        }

        public static void RegisterTypeWithManager()
        {
            // This must be overridden
            // Looks like AttributeCacheManager.Instance.RegisterCustomAttributeType<your type>()
            UnityEngine.Debug.LogError("Must override RegisterTypeWithManager()");
        }

        void LoadAssembly()
        {
#if !USE_WINRT
            AttributeCacheManager.Instance.CacheAttributesOnce(Assembly.GetExecutingAssembly());
#endif
        }
    }


    // Base class for attribute cache
    public abstract class AttributeCacheBase
    {
        public abstract void ProcessMemberInfo(MethodInfo methodInfo, FieldInfo fieldInfo);
        public abstract List<KeyValuePair<Type, bool>> GetDeclaringTypes();
        public abstract void UpdateInstances();
    }

    // Attribute cache works with one type of custom attribute
    public class AttributeCache<TAttribute> : AttributeCacheBase where TAttribute : CustomAttributeBase
    {
        public List<TAttribute> cachedTAttributes = new List<TAttribute>();

        public List<TAttribute> GetTAttributes()
        {
            return cachedTAttributes;
        }

        public override void UpdateInstances()
        {
            foreach (TAttribute att in cachedTAttributes)
            {
                att.UpdateInstances();
            }
        }

        // Get a unique list of all declaring types, paired with whether they want instances cached
        public override List<KeyValuePair<Type, bool>> GetDeclaringTypes()
        {
            List<KeyValuePair<Type, bool>> types = new List<KeyValuePair<Type, bool>>();
            foreach (TAttribute att in cachedTAttributes)
            {
                Type t = att.GetDeclaringType();
                if (!types.Exists(a => { return a.Key == t; }))
                {
                    types.Add(new KeyValuePair<Type, bool>(t, att.bCacheInstances));
                }
            }
            return types;
        }

        // Checks for the custom attribute on the method or field, and if found, creates record 
        //  for the member info and adds it to the cache
        public override void ProcessMemberInfo(MethodInfo methodInfo, FieldInfo fieldInfo)
        {
            bool isMethod = methodInfo != null;
            MemberInfo memberInfo = isMethod ? methodInfo : fieldInfo as MemberInfo;

            // See if the custom attribute exists on the method or field
            TAttribute attribute = null;
#if USE_WINRT
		attribute = memberInfo.GetCustomAttribute<TAttribute>(true);
#else
            object[] customAttributes = memberInfo.GetCustomAttributes(typeof(TAttribute), true);
            if (customAttributes.Length > 0)
            {
                attribute = (TAttribute)customAttributes[0];
            }
#endif

            // Try to add the attribute
            if (attribute != null)
            {
                bool isStatic = isMethod ? methodInfo.IsStatic : fieldInfo.IsStatic;

                // Only if not already added
                if (cachedTAttributes.Find(t => t.memberInfo == memberInfo) == null)
                {
#if USE_WINRT
				if (isStatic || memberInfo.DeclaringType.GetTypeInfo().IsSubclassOf(typeof(UnityEngine.Object)))
#else
                    if (isStatic || typeof(UnityEngine.Object).IsAssignableFrom(memberInfo.DeclaringType))
#endif
                    {
                        // Create the record for the member info
                        attribute.SetInfo(memberInfo, methodInfo, fieldInfo);
                        cachedTAttributes.Add(attribute);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("Attempting to register a field of a non-Object derived class.  This is unsupported!");
                    }
                }
            }
        }
    }

}
