/* *********************************************************************
   Copyright 2009-2010 MySpace

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
********************************************************************* */


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Negroni.TemplateFramework;
using Negroni.TemplateFramework.Configuration;
using Negroni.TemplateFramework.Parsing;
using Negroni.TemplateFramework.Tooling;

namespace Negroni.TemplateFramework
{

	/// <summary>
	/// 
	/// Factory class for all Gadget Controls.  
	/// This loads all defined control definitions at runtime and is used to identify 
	/// gadget controls in markup and create instances of the proper control.
	/// <para>
	/// Additional controls may be loaded from external assemblies by calling
	/// <c>ControlFactory.LoadGadgetControls</c>
	/// </para>
	/// </summary>
	/// <remarks>
	/// This is a singleton class which may be used as a factory for arbitrary control structures.
	/// The default ControlFactory instance is the OSMLControlFactory.  Additional ControlFactory
	/// instances may be registered, loading controls from other Assemblies.
	/// </remarks>
	public class ControlFactory
	{

#if DEBUG
		public string myId = Guid.NewGuid().ToString();
#endif

		public const string CDATA_START_TAG = "<![CDATA[";
		public const string CDATA_END_TAG = "]]>";
		public const string COMMENT_START_TAG = "<!--";
		public const string COMMENT_END_TAG = "-->";


		const string GADGET_FACTORY_KEY = "gadgetControlFactory";

		public const string RESERVEDKEY_LITERAL = "Literal";
		public const string RESERVEDKEY_CUSTOM_TAG = "CustomTag";
		public const string RESERVEDKEY_GENERIC_CONTAINER = "GenericContainer";

		/// <summary>
		/// A time marker for when this ControlFactory instance was created.
		/// Used for certain performance counters.
		/// </summary>
		internal readonly long startTimeTicks = DateTime.Now.Ticks;


		#region private constructors for ControlFactory

		private ControlFactory() 
        {
            Catalog.Add(ParseContext.DefaultContext, new ControlCatalog(ParseContext.DefaultContext));
        }

		/// <summary>
		/// Initialize a new ControlFactory instance and register it under the factoryKey
		/// </summary>
		/// <param name="factoryKey"></param>
		/// <param name="controlAssembly"></param>
		public ControlFactory(string factoryKey, Assembly controlAssembly)
            :this()
        {
			FactoryKey = factoryKey;
			LoadGadgetControls(controlAssembly);

		}

		#endregion

		/// <summary>
		/// Key used to identify this factory instance
		/// </summary>
		public string FactoryKey { get; set; }



		#region ControlFactory singleton management

		static private Dictionary<string, ControlFactory> _factorySingletons = null;

		/// <summary>
		/// Dictionary of singleton ControlFactories registered with static ControlFactory
		/// </summary>
		static Dictionary<string, ControlFactory> FactorySingletons
		{
			get
			{
				if (_factorySingletons == null)
				{
					_factorySingletons = new Dictionary<string, ControlFactory>();
				}
				return _factorySingletons;
			}
		}


		static private ControlFactory _defaultInstance = null;

		///// <summary>
		///// Singleton accessor for the built-in Gadget ControlFactory instance.
		///// </summary>
		//[Obsolete("This must be retired because multiple control frameworks may be simultaneously active")]
		//static public ControlFactory Instance
		//{
		//    get
		//    {
		//        if (_defaultInstance == null)
		//        {
		//            InitializeGadgetControlFactoryInstance();
		//        }
		//        return _defaultInstance;
		//    }
		//}

		/// <summary>
		/// Tests to see if a factory is registered under the given key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		static public bool IsFactoryDefined(string key)
		{
			return (FactorySingletons.ContainsKey(key));
		}


		/// <summary>
		/// Initializes the Gadget ControlFactory and places it in the default 
		/// Instance singleton accessor.
		/// </summary>
		[Obsolete("Factory registration is config driven - retire this method")]
		static public void InitializeGadgetControlFactoryInstance()
		{
			_defaultInstance = new ControlFactory(GADGET_FACTORY_KEY, Assembly.GetExecutingAssembly());
			//add default key
			ParseContext defaultContextKey = new ParseContext(typeof(BaseTemplate));
			_defaultInstance.SetDefaultContextGroup(defaultContextKey);
			//force TagTemplate as a context key
			if (_defaultInstance.RegisteredControlTypes.ContainsKey(typeof(CustomTagTemplate)))
			{
				_defaultInstance.RegisteredControlTypes[typeof(CustomTagTemplate)].IsContextGroupContainer = true;
			}

			//_defaultInstance.InitializeGadgetControlCatalog();

		}

		/// <summary>
		/// Lock object for
		/// </summary>
		static object factorySingletonsLock = new object();

		/// <summary>
		/// Refreshes a ControlFactory instance from a config file
		/// </summary>
		/// <param name="key"></param>
		private static bool RefreshInstanceFromConfig(string key)
		{
			//Attempt to load from Config
			if (!NegroniFrameworkConfig.ControlFactories.ContainsKey(key)
				&& key != NegroniFrameworkConfig.CONFIGPARSER_CONTROLFACTORY)
			{
				return false;
			}

			lock (factorySingletonsLock)
			{
				if (!FactorySingletons.ContainsKey(key))
				{
					object ignoreLock = new object(); //already have a lock, so don't need another lock
					ControlFactory pitch = null;
					RefreshInstance(key, ref pitch, ignoreLock); 
				}
			}
			return true;
		}

		/// <summary>
		/// Reloads and initializes the instance ControlFactory based on key
		/// </summary>
		public static void RefreshInstance(string key, ref ControlFactory instance, object lockObject)
		{
			if (null == lockObject) lockObject = new object(); //make sure we don't fail if locking is ignored

			lock (lockObject)
			{
				//insure it is removed
				ControlFactory.RemoveControlFactory(key);

				List<string> assemblies = NegroniFrameworkConfig.ControlFactories[key];
				ControlFactory tmp = ControlFactory.CreateControlFactory(key);

				foreach (string item in assemblies)
				{
					try
					{
						AssemblyName name = new AssemblyName(item);
						Assembly asm = Assembly.Load(name);
						//Assembly asm = Assembly.LoadFrom(item);
						tmp.LoadGadgetControls(asm, NegroniFrameworkConfig.GetIncludeFilter(key, item),
							NegroniFrameworkConfig.GetExcludeFilter(key, item));
					}
					catch (Exception ex)
					{
						tmp.LoadErrors.Add(String.Format("Assembly load error: {0} \n{1}", item, ex.Message));
					}

				}
				instance = tmp;
			}
		}


		/// <summary>
		/// Creates and registers an empty ControlFactory instance under the given key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		static public ControlFactory CreateControlFactory(string key)
		{
			return CreateControlFactory(key, null);
		}

		/// <summary>
		/// Creates and registers a ControlFactory.  
		/// Loads any controls from controlAssembly.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="controlAssembly">Assembly containing BaseGadgetControl definitions</param>
		/// <returns></returns>
		static public ControlFactory CreateControlFactory(string key, Assembly controlAssembly)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("Must specify a key for ControlFactory");
			}

			if (FactorySingletons.ContainsKey(key))
			{
				return FactorySingletons[key];
			}

			ControlFactory factory = null;
			lock (factorySingletonsLock)
			{
				if (!FactorySingletons.ContainsKey(key))
				{
					factory = new ControlFactory(key, controlAssembly);
					factory.LoadGadgetControls(Assembly.GetExecutingAssembly());
					FactorySingletons.Add(key, factory);
				}
				else
				{
					//this happens if two threads hit at same time.
					//the second thread picks up result of the first
					factory = FactorySingletons[key];
				}
			}
			return factory;
		}

		/// <summary>
		/// Gets a reference to a ControlFactory singleton.  If the ControlFactory does not exist
		/// the framework will attempt to initialize it from the OpenSocialControlFramework.config
		/// definitions.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		static public ControlFactory GetControlFactory(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("Must specify a key for ControlFactory");
			}

			if (!FactorySingletons.ContainsKey(key))
			{
				if (!RefreshInstanceFromConfig(key))
				{
					throw new ControlFactoryNotDefinedException("Factory key: " + key + " not found in config definitions");
				}
			}
			return FactorySingletons[key];
		}

        /// <summary>
        /// Returns a list of all currently defined control factory keys
        /// </summary>
        /// <returns></returns>
        static public List<string> GetControlFactoryKeys()
        {
            List<string> factoryKeys = new List<string>();

            foreach (var key in FactorySingletons.Keys)
            {
                factoryKeys.Add(key);
            }
            return factoryKeys;
        }
        
        /// <summary>
		/// Removes a previously registered ControlFactory singleton
		/// </summary>
		/// <param name="key"></param>
		static public void RemoveControlFactory(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("Must specify a key for ControlFactory");
			}

			if (!FactorySingletons.ContainsKey(key))
			{
				return;
			}

			lock (factorySingletonsLock)
			{
				if (FactorySingletons.ContainsKey(key))
				{
					FactorySingletons.Remove(key);
				}
			}
		}

		/// <summary>
		/// Clears all registered ControlFactory singletons
		/// </summary>
		static public void ClearControlFactories()
		{
			lock (factorySingletonsLock)
			{
				ControlFactory configFactory = null;
				if (FactorySingletons.ContainsKey(NegroniFrameworkConfig.CONFIGPARSER_CONTROLFACTORY))
				{
					configFactory = FactorySingletons[NegroniFrameworkConfig.CONFIGPARSER_CONTROLFACTORY];
				}
				FactorySingletons.Clear();
				if (configFactory != null)
				{
					FactorySingletons.Add(NegroniFrameworkConfig.CONFIGPARSER_CONTROLFACTORY, configFactory);
				}
			}
		}

		#endregion


		#region ControlMap methods

		/// <summary>
		/// Object for locking the catalog during initialization
		/// </summary>
		private object catalogLock = new object();
		

		private Dictionary<ParseContext, ControlCatalog> _catalog = null;

		/// <summary>
		/// Master catalog of supported controls within their context
		/// </summary>
		Dictionary<ParseContext, ControlCatalog> Catalog
		{
			get
			{
				if (_catalog == null)
				{
					_catalog = new Dictionary<ParseContext, ControlCatalog>();
				}
				return _catalog;
			}
		}

		/// <summary>
		/// Hash of assemblies that have been loaded into this factory.
		/// This is used in conjunction with catalogLock to insure it doesn't 
		/// over-register on initialization
		/// </summary>
		Dictionary<Assembly, bool> _loadedAssemblies = new Dictionary<Assembly, bool>();

		/// <summary>
		/// Encapsulation of info stored in the AltAttributeMap
		/// </summary>
		class AltAttributeMapEntry
		{
			public AltAttributeMapEntry() { }

			public AltAttributeMapEntry(string attribute, string offsetKey)
			{
				Attribute = attribute;
				OffsetKey = offsetKey;
			}
			public AltAttributeMapEntry(string attribute, string offsetKey, int precedenceWeight)
				: this(attribute, offsetKey)
			{
				PrecedenceWeight = precedenceWeight;
			}

			public string Attribute = null;
			public string OffsetKey = null;
			public int PrecedenceWeight = 0;
		}

		private Dictionary<string, AltAttributeMapEntry> _altAttributeMap = null;

		/// <summary>
		/// Mapping of attribute alternatives to real tags.
		/// Key is on alternate attribute name, value is control OffsetKey
		/// </summary>
		Dictionary<string, AltAttributeMapEntry> AlternateAttributeMap
		{
			get
			{
				if (_altAttributeMap == null)
				{
					_altAttributeMap = new Dictionary<string, AltAttributeMapEntry>();
				}
				return _altAttributeMap;
			}
		}


		private Dictionary<string, ControlMap> _registeredControlOffsetKeys = null;

		/// <summary>
		/// Master mapping list of all registered Controls, keyed on their OffsetKey.
		/// This is a flat listing which ignores ContextGroup.  For contextual listings
		/// look into the <c>Catalog</c>
		/// </summary>
		Dictionary<string, ControlMap> RegisteredControlOffsetKeys
		{
			get
			{
				if (_registeredControlOffsetKeys == null)
				{
					_registeredControlOffsetKeys = new Dictionary<string, ControlMap>();
				}
				return _registeredControlOffsetKeys;
			}
		}


		private Dictionary<Type, ControlMap> _registeredControlTypes = null;

		/// <summary>
		/// Master mapping list of all registered Controls, keyed on their OffsetKey.
		/// This is a flat listing which ignores ContextGroup.  For contextual listings
		/// look into the <c>Catalog</c>
		/// </summary>
		Dictionary<Type, ControlMap> RegisteredControlTypes
		{
			get
			{
				if (_registeredControlTypes == null)
				{
					_registeredControlTypes = new Dictionary<Type, ControlMap>();
				}
				return _registeredControlTypes;
			}
		}



		private Dictionary<string, List<ControlMap>> _dependentAttributeMap = null;

		/// <summary>
		/// Mapping of tags which have dependent attributes to their full ControlMaps.
		/// Any tag may have 0, 1, or N dependent attributes.  
		/// This is defined in the control class with the AttributeTagDependent attribute
		/// </summary>
		public Dictionary<string, List<ControlMap>> DependentAttributeMap
		{
			get
			{
				if (_dependentAttributeMap == null)
				{
					_dependentAttributeMap = new Dictionary<string, List<ControlMap>>();
				}
				return _dependentAttributeMap;
			}
		}



		#region Methods supporting tests and interrogation of registered controls

		/// <summary>
		/// Returns number of Controls registered for a given ParseContext.
		/// Returns -1 if ParseContext doesn't exist.
		/// </summary>
		/// <param name="context"></param>
		/// <returns>Count of controls or -1 if context not registered</returns>
		public int GetControlCount(ParseContext context)
		{
			if (!Catalog.ContainsKey(context))
			{
				return -1;
			}
			else
			{
				return Catalog[context].Count;
			}
		}


		/// <summary>
		/// Builds and returns a list of all registered Context Groups.
		/// This is somewhat expensive, so cache the results if you plan on using it.
		/// </summary>
		/// <returns></returns>
		public List<ParseContext> GetAvailableContextGroups()
		{
			List<ParseContext> contexts = new List<ParseContext>();
			foreach (KeyValuePair<ParseContext, ControlCatalog> keyset in Catalog)
			{
				contexts.Add(keyset.Key);				
			}
			return contexts;
		}


		/// <summary>
		/// Discovers the first ContextGroup where the given type of control is registered.
        /// This is the context parsing group where this control tag will be found.
		/// Returns null if control type not registered.
		/// </summary>
		/// <param name="controlType"></param>
		/// <returns></returns>
		public ParseContext GetControlContextGroup(Type controlType)
		{
			foreach (KeyValuePair<ParseContext, ControlCatalog> keyset in Catalog)
			{
				if (keyset.Value.ControlMapControlType.ContainsKey(controlType))
				{
					return keyset.Key;
				}				
			}
			return null;
		}


		/// <summary>
		/// Discovers the first ContextGroup where the given type of control is registered.
		/// This is the context parsing group where this control tag will be found.
		/// Returns null if control type not registered.
		/// </summary>
		/// <param name="controlType"></param>
		/// <returns></returns>
		public List<ParseContext> GetControlContextGroups(Type controlType)
		{
			List<ParseContext> values = new List<ParseContext>();
			foreach (KeyValuePair<ParseContext, ControlCatalog> keyset in Catalog)
			{
				if (keyset.Value.ControlMapControlType.ContainsKey(controlType))
				{
					values.Add(keyset.Key);
				}
			}
			return values;
		}



		/// <summary>
		/// Returns the proper <c>ParseContext</c> for all child controls of the
		/// given controlType.  The controlType must be a type inheriting from
		/// BaseContainerControl.
		/// </summary>
		/// <remarks>
		/// If the controlType specified corresponds to a registered ContextGroup owner control,
		/// the ParseContext of the control its self is returned.  Otherwise, the controlType's 
		/// parent ContextGroup is returned.
		/// </remarks>
		/// <param name="controlType">The type of BaseContainerControl to find a child context for</param>
		/// <param name="controlParentContext">The ContextGroup specified for the controlType passed in.</param>
		/// <returns></returns>
		public ParseContext GetChildControlContextGroup(Type controlType, ParseContext controlParentContext)
		{
			ControlMap map = GetControlMap(controlType);
			if (map.IsContextGroupContainer)
			{
				//return this type
				return new ParseContext(controlType);
			}
			else
			{
				//return
				return controlParentContext;
			}
		}


		#endregion




		/// <summary>
		/// Static initialization of the ControlFactory internal plumbing
		/// </summary>
		static ControlFactory()
		{
			GenericRootElementMap = new ControlMap(String.Empty, ControlFactory.RESERVEDKEY_GENERIC_CONTAINER, typeof(BaseContainerControl));

			LiteralElementMap = new ControlMap(String.Empty, ControlFactory.RESERVEDKEY_LITERAL, typeof(GadgetLiteral));
		}


		public string GetDefaultContainerMarkupTag()
		{
			if (_catalog.ContainsKey(defaultContext))
			{
				return "script type='text/os-template' "; //_catalog[defaultContext].MyContext.;
			}
			return null;
		}

		private ParseContext defaultContext = ParseContext.DefaultContext;

		/// <summary>
		/// Specifies which ContextGroup will be used as the catch-all context
		/// when none is specified.  This must be invoked after initial ContextGroups 
		/// are registered or Assembly controls are loaded.
		/// </summary>
		/// <param name="contextName">An existing ContextGroup which will be identified as the Default</param>
		public void SetDefaultContextGroup(Type containerControlType)
		{
			SetDefaultContextGroup(new ParseContext(containerControlType));
		}

		/// <summary>
		/// Specifies which ContextGroup will be used as the catch-all context
		/// when none is specified.  This must be invoked after initial ContextGroups 
		/// are registered or Assembly controls are loaded.
		/// </summary>
		/// <param name="contextName">An existing ContextGroup which will be identified as the Default</param>
		public void SetDefaultContextGroup(ParseContext contextGroup)
		{
			defaultContext = contextGroup;

			if (!Catalog.ContainsKey(contextGroup))
			{
				Catalog.Add(contextGroup, null);
				if (this.RegisteredControlTypes.ContainsKey(contextGroup.ContainerControlType))
				{
					RegisteredControlTypes[contextGroup.ContainerControlType].IsContextGroupContainer = true;
				}

			}

			ControlCatalog genCatalog = null;
			ControlCatalog contextCatalog = null;
			ControlCatalog usedCatalog = null;

			if(Catalog.ContainsKey(contextGroup)){
				contextCatalog = Catalog[contextGroup];
			}

			if (Catalog.ContainsKey(ParseContext.DefaultContext))
			{
				genCatalog = Catalog[ParseContext.DefaultContext];
			}
			else
			{
				Catalog.Add(ParseContext.DefaultContext, null);
			}

			if (contextCatalog != null)
			{
				if (genCatalog == null)
				{
					usedCatalog = contextCatalog;
				}
				else
				{
					if (genCatalog.Count > 0)
					{
						usedCatalog = genCatalog;
						//add in any previously registered controls
						if (contextCatalog != null && contextCatalog.Count > 0)
						{
							foreach (KeyValuePair<string, ControlMap> keyset in contextCatalog.ControlMapOffsetKey)
							{
								if (!usedCatalog.ControlMapOffsetKey.ContainsKey(keyset.Value.OffsetKey))
								{
									usedCatalog.Add(keyset.Value);
								}
							}
						}
					}
					else
					{
						usedCatalog = contextCatalog;
					}
				}
				usedCatalog.ContextOffsetKey = contextCatalog.ContextOffsetKey;
			}
			else
			{
				if (genCatalog == null)
				{
					usedCatalog = new ControlCatalog(contextGroup);
				}
				else
				{
					usedCatalog = genCatalog;
				}
			}
			if (this.RegisteredControlTypes.ContainsKey(contextGroup.ContainerControlType))
			{
				if (!String.IsNullOrEmpty(RegisteredControlTypes[contextGroup.ContainerControlType].OffsetKey))
				{
					usedCatalog.ContextOffsetKey = RegisteredControlTypes[contextGroup.ContainerControlType].OffsetKey;
				}
			}

			Catalog[contextGroup] = usedCatalog;
			Catalog[ParseContext.DefaultContext] = usedCatalog;
		}

		/// <summary>
		/// Loads any Gadget control definitions from assembly into the ControlFactory
		/// </summary>
		/// <param name="assembly"></param>
		public void LoadGadgetControls(Assembly assembly)
		{
			LoadGadgetControls(assembly, null, null);
		}


		/// <summary>
		/// Loads any Gadget control definitions from assembly into the ControlFactory
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="includeFilter">Pattern match/names for control classes to include</param>
		/// <param name="excludeFilter">Pattern match/names for excluded control classes</param>
		public void LoadGadgetControls(Assembly assembly, string includeFilter, string excludeFilter){
			if (null == assembly)
			{
				return;
			}

			if (_loadedAssemblies.ContainsKey(assembly))
			{
				return;
			}

			lock (catalogLock)
			{
				//double check to fallout
				if (_loadedAssemblies.ContainsKey(assembly))
				{
					return;
				}

				Type[] types = assembly.GetTypes();

				Type baseControlType = typeof(BaseGadgetControl);
				Type containerControlType = typeof(BaseContainerControl);

				List<ParseContext> contextKeys = new List<ParseContext>();

				foreach (Type t in types)
				{
					Type baseType = t.BaseType;
					if (typeof(object) == baseType || t.IsAbstract)
					{
						continue;
					}
					if (InheritsFromType(t, baseControlType) && ClassPassesFilters(t.FullName, includeFilter, excludeFilter))
					{
						//will only look at current and direct parent class
						object[] attrs = t.GetCustomAttributes(false);
						bool foundVals = false;
						if (contextKeys.Count > 0) contextKeys.Clear();
						ControlMap ctlMap = new ControlMap();

						ctlMap.MarkupTag = null;
						string altAttrKey = null;
						//int altAttrPrecedenceWeight = 0;
						bool isRootElementDefinition = false;
						bool setDefaultCatalogContextGroupToCurrent = false;
						for (int i = 0; i < attrs.Length; i++)
						{
							if (attrs[i] is MarkupTagAttribute)
							{
								// this has the side effect of setting the offsetKey, if not already specified
								ctlMap.SetMarkupTag(((MarkupTagAttribute)attrs[i]).MarkupTag);
								foundVals = true;
							}
							else if (attrs[i] is OffsetKeyAttribute)
							{
								ctlMap.OffsetKey = ((OffsetKeyAttribute)attrs[i]).Key;
								foundVals = true;
							}
							else if (attrs[i] is ContextGroupAttribute)
							{
								ParseContext contextGroup = ((ContextGroupAttribute)attrs[i]).ContextGroup;
								contextKeys.Add(contextGroup);
							}
							else if (attrs[i] is ContextGroupContainerAttribute)
							{
								ctlMap.IsContextGroupContainer = true;
								ParseContext contextKey = new ParseContext(t);
								ContextGroupContainerAttribute cga = (ContextGroupContainerAttribute)attrs[i];
								if (cga.IsDefaultContext)
								{
									SetDefaultContextGroup(contextKey);
									setDefaultCatalogContextGroupToCurrent = true;
								}
								else if (!Catalog.ContainsKey(contextKey))
								{
									Catalog.Add(contextKey, new ControlCatalog(contextKey));
								}
							}
							else if (attrs[i] is AttributeTagAlternativeAttribute)
							{
								altAttrKey = ((AttributeTagAlternativeAttribute)attrs[i]).AttributeName;
								ctlMap.AttributeTagAlternative = altAttrKey;
								ctlMap.AttributeTagPrecedenceWeight = ((AttributeTagAlternativeAttribute)attrs[i]).PrecedenceWeight;
							}
							else if (attrs[i] is AttributeTagDependentAttribute)
							{
								AttributeTagDependentAttribute dependentAttr = (AttributeTagDependentAttribute)attrs[i];
								if (!string.IsNullOrEmpty(ctlMap.AttributeTagDependentKey))
								{
									ctlMap.AttributeTagDependentKey += "|" + dependentAttr.AttributeName;
									ctlMap.AttributeTagDependentValue += "|" + dependentAttr.AttributeValue;
								}
								else
								{
									ctlMap.AttributeTagDependentKey = dependentAttr.AttributeName;
									ctlMap.AttributeTagDependentValue = dependentAttr.AttributeValue;
								}
							}
							else if (attrs[i] is RootElementAttribute)
							{
								isRootElementDefinition = true;
								ctlMap.IsContextGroupContainer = true;
								RootElementAttribute rea = (RootElementAttribute)attrs[i];
								if (rea.IsDefaultParseContext)
								{
									ParseContext contextKey = new ParseContext(t);
									SetDefaultContextGroup(contextKey);
									setDefaultCatalogContextGroupToCurrent = true;
								}
							}

						}
						if (foundVals)
						{
							//if no keys add to Default ParseContext key
							if (contextKeys.Count == 0)
							{
								contextKeys.Add(ParseContext.DefaultContext);
							}

							ctlMap.ControlType = t;

							foreach (ParseContext context in contextKeys)
							{
								//add context on the fly, if not already defined
								if (!Catalog.ContainsKey(context))
								{
									Catalog.Add(context, new ControlCatalog(context));
								}
								if (!Catalog[context].ControlMapOffsetKey.ContainsKey(ctlMap.OffsetKey))
								{
									Catalog[context].Add(ctlMap);
								}
								else
								{
									this.LoadErrors.Add("Attempting to register duplicate control: " + ctlMap.OffsetKey + " in context: " + context.ContextName);
								}
							}
							//add to the master OffsetKey dictionary
							RegisteredControlOffsetKeys.Add(ctlMap.OffsetKey, ctlMap);
							//add to the master Type dictionary
							RegisteredControlTypes.Add(ctlMap.ControlType, ctlMap);
							//add to controlType counters
							Counters.AddCountedControlType(ctlMap.ControlType);

							//add alt attribute mapping, if found
							if (altAttrKey != null)
							{
								AlternateAttributeMap.Add(altAttrKey.ToLowerInvariant(), 
									new AltAttributeMapEntry(altAttrKey.ToLowerInvariant(), ctlMap.OffsetKey, ctlMap.AttributeTagPrecedenceWeight));
							}
							if (ctlMap.HasAttributeDependency())
							{
								if (!DependentAttributeMap.ContainsKey(ctlMap.MarkupTag.ToLowerInvariant()))
								{
									DependentAttributeMap.Add(ctlMap.MarkupTag.ToLowerInvariant(), new List<ControlMap>());
								}
								DependentAttributeMap[ctlMap.MarkupTag].Add(ctlMap);
							}
							//add the root
							if (isRootElementDefinition)
							{
								SetRootElement(ctlMap);
							}
							//check for setting default contextGroup
							if (setDefaultCatalogContextGroupToCurrent)
							{
								if (Catalog.ContainsKey(ParseContext.DefaultContext))
								{
									Catalog[ParseContext.DefaultContext].ContextOffsetKey = ctlMap.OffsetKey;
								}
							}
						}

					}
				}

				//verify and initialize the ContextOffsetKey values for each ControlCatalog
				foreach (KeyValuePair<ParseContext, ControlCatalog> keyset in Catalog)
				{
					ControlCatalog catalog = keyset.Value;
					if (String.IsNullOrEmpty(catalog.ContextOffsetKey))
					{
						if (RegisteredControlTypes.ContainsKey(keyset.Key.ContainerControlType))
						{
							ControlMap map = RegisteredControlTypes[keyset.Key.ContainerControlType];
							catalog.ContextOffsetKey = map.OffsetKey;
							map.IsContextGroupContainer = true;
						}
					}

				}

				_loadedAssemblies.Add(assembly, true);
			}

		}
		/// <summary>
		/// Checks the control classname against the filters
		/// </summary>
		/// <param name="controlClassFullname"></param>
		/// <param name="includeFilter"></param>
		/// <param name="excludeFilter"></param>
		/// <returns></returns>
		static internal bool ClassPassesFilters(string controlClassFullname, string includeFilter, string excludeFilter)
		{
			
			if (string.IsNullOrEmpty(includeFilter) && string.IsNullOrEmpty(excludeFilter))
			{
				return true;
			}
			string className, classNamespace;
			int dotPos = controlClassFullname.LastIndexOf(".");
			if (dotPos == -1)
			{
				className = controlClassFullname;
				classNamespace = null;
			}
			else
			{
				classNamespace = controlClassFullname.Substring(0, dotPos);
				className = controlClassFullname.Substring(dotPos + 1);
			}

			bool includeOk = ClassNameFilterPasses(className, classNamespace, includeFilter);
			if (!includeOk)
			{
				return false;
			}
			bool excludeOk = string.IsNullOrEmpty(excludeFilter) ||
				!ClassNameFilterPasses(className, classNamespace, excludeFilter);
			return includeOk && excludeOk;
		}

		static private bool ClassNameFilterPasses(string className, string classNamespace, string filter)
		{
			if (string.IsNullOrEmpty(filter) || filter == "*")
			{
				return true;
			}
			if (!filter.Contains("|"))
			{
				return ClassNamePassesFilterPart(className, classNamespace, filter);
			}
			else
			{
				bool passes = false;
				string[] fparts = filter.Split(new char[] { '|' });
				for (int i = 0; i < fparts.Length; i++)
				{
					if (ClassNamePassesFilterPart(className, classNamespace, fparts[i]))
					{
						passes = true;
						break;
					}
				}

				return passes;
			}
		}

		/// <summary>
		/// Checks on a part of a pipe delimited filter to see if it passes.
		/// </summary>
		/// <param name="className"></param>
		/// <param name="classNamespace"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		private static bool ClassNamePassesFilterPart(string className, string classNamespace, string filter)
		{
			if (string.IsNullOrEmpty(filter))
			{
				return false;
			}

			bool filterHasDot = filter.Contains(".");
			bool filterHasWildcard = filter.Contains("*");
			bool filterOk;
			//name only
			if (!filterHasDot)
			{
				if (filterHasWildcard)
				{
					filterOk = true;
				}
				else
				{
					filterOk = (className.Equals(filter));
				}
			}
			else
			{
				char[] dotChar = new char[] { '.' };
				string[] filterParts = filter.Split(dotChar);
				string[] nsParts = classNamespace.Split(dotChar);
				filterOk = true;
				for (int i = 0; i < filterParts.Length; i++)
				{
					if (filterParts[i] == "*")
					{
						continue;
					}
					else
					{
						if (i == nsParts.Length && filterParts[i] == className)
						{
							continue;
						}
						else if (i > nsParts.Length)
						{
							filterOk = false;
							break;
						}
						else if (filterParts[i] != nsParts[i])
						{
							filterOk = false;
							break;
						}
					}
				}
			}
			return filterOk;
		}

		/// <summary>
		/// Tests inheritence chain to see if type inherits from targetBaseType
		/// </summary>
		/// <param name="type"></param>
		/// <param name="targetBaseType"></param>
		/// <returns></returns>
		internal static bool InheritsFromType(Type type, Type targetBaseType)
		{
			//everything inherits from object
			if(targetBaseType == typeof(object)){
				return true;
			}
			Type chainType = type.BaseType;
			while(typeof(object) != chainType){
				if(chainType.Equals(targetBaseType)){
					return true;
				}
				chainType = chainType.BaseType;
			}
			return false;
		}


		/// <summary>
		/// Purge the Catalog before reloading it
		/// </summary>
		private void ClearControlCatalog()
		{
			if (_catalog != null)
			{
				Catalog.Clear();
			}


			if(_altAttributeMap != null)
			{
				AlternateAttributeMap.Clear();
			}
			if(_registeredControlOffsetKeys != null)
			{
				RegisteredControlOffsetKeys.Clear();
			}
			if(_registeredControlTypes != null)
			{
				RegisteredControlTypes.Clear();
			}

			
		}

		#endregion


		#region Static ControlFactory members


		/// <summary>
		/// Tests to see if the open/close brackets match up
		/// </summary>
		/// <param name="markup"></param>
		/// <returns></returns>
		public static bool IsTagBalancedElement(string markup)
		{
			if (string.IsNullOrEmpty(markup))
			{
				return true;
			}

			int openCnt = 0, closeCnt = 0;

			for (int i = 0; i < markup.Length; i++)
			{
				if (markup[i] == '<') openCnt++;
				if (markup[i] == '>') closeCnt++;
			}
			return (openCnt == closeCnt);
		}


		/// <summary>
		/// Tests markup to determine if it is a single element
		/// fully encapsulated by markup, or if there are multiple root-level
		/// markup elements.
		/// </summary>
		/// <param name="markup"></param>
		/// <returns>True if encapsulated or a literal text string, False if multiple root tags</returns>
		public static bool IsEncapsulatedElement(string markup)
		{
			if (string.IsNullOrEmpty(markup))
			{
				return true;
			}
			if (!markup.Contains("<") && !markup.Contains(">"))
			{
				return true;
			}

			//test for comments
			if (markup.StartsWith(COMMENT_START_TAG) && markup.EndsWith(COMMENT_END_TAG))
			{
				//TODO: count for embedded comment end tags
				return true;
			}


			string startTag = GetTagName(markup);

			int lastCloseTagPos = markup.LastIndexOf("</");
			int closeStartTagPos = markup.IndexOf("</" + startTag);
			int closeEndTagPos = -1;
			if (closeStartTagPos > -1)
			{
				closeEndTagPos = markup.IndexOf(">", closeStartTagPos);
			}

			int lastCloseTagEndPos = markup.LastIndexOf(">");
			int closeEmptyTagPos = markup.LastIndexOf("/>");

			if (lastCloseTagPos == closeStartTagPos 
				&& (closeEndTagPos == markup.Length - 1 ||
				(closeEmptyTagPos + 1 == markup.Length - 1 && closeEndTagPos == -1)))
			{
				return true;
			}
			return false;
		}
		

		/// <summary>
		/// Gets the tag element name from a markup tag.
		/// </summary>
		/// <param name="markup"></param>
		/// <returns>Value found for attribute or null</returns>
		public static string GetTagName(string markup)
		{
			if (string.IsNullOrEmpty(markup))
			{
				return null;
			}
			int startPos = markup.IndexOf("<");
			if (startPos == -1)
			{
				return null;
			}
			else
			{
				int commentStartPos = markup.IndexOf(COMMENT_START_TAG),
					cdataStartPos = markup.IndexOf(CDATA_START_TAG);

				if (startPos == commentStartPos || startPos == cdataStartPos)
				{
					return null;
				}

				if (markup[startPos + 1] == ' ')
				{
					markup = markup.Substring(startPos + 1).Trim();
					startPos = 0;
				}
				else
				{
					startPos++;
				}
				
			}

			int endPos = 0;
			int endPosSpace = markup.IndexOf(" ", startPos);
			int endPosNewline = markup.IndexOf("\n", startPos);
			int endPosTabChar = markup.IndexOf("\t", startPos);
			int endPosTag = markup.IndexOf(">", startPos);

			bool isSingleLineTagOnly = (endPosSpace == -1 && endPosNewline == -1 && endPosTabChar == -1);
			bool isEmptyTag = (endPosTag > -1 && (endPosTag < endPosSpace && endPosTag < endPosTabChar && endPosTag < endPosNewline));

			if (isSingleLineTagOnly || isEmptyTag)
			{
				endPos = markup.IndexOf("/>", startPos);
				if (endPos == -1)
				{
					endPos = markup.IndexOf(">", startPos);
					if (endPos == -1)
					{
						endPos = markup.Length;
					}
				}
				else
				{
					if (endPosTag < endPos)
					{
						endPos = endPosTag;
					}
				}
			}
			else{
				endPos = GetMinValidPosition(new int[]{endPosTag, endPosSpace, endPosNewline, endPosTabChar});
			}
			if (endPos > startPos)
			{
				return markup.Substring(startPos, endPos - startPos).Trim();
			}
			return null;
		}

		/// <summary>
		/// Gets the minimum position value that is greater than -1
		/// </summary>
		/// <param name="positionValues"></param>
		/// <returns></returns>
		private static int GetMinValidPosition(int[] positionValues)
		{
			if (positionValues.Length == 0) return -1;
			int minValue = Int32.MaxValue;
			for (int i = 0; i < positionValues.Length; i++)
			{
				if (positionValues[i] > -1)
				{
					minValue = Math.Min(minValue, positionValues[i]);
				}
			}
			return minValue;
		}



		/// <summary>
		/// Extracts the attribute value from a given tag string
		/// </summary>
		/// <param name="attrStr"></param>
		/// <returns>Value found for attribute or null</returns>
		public static string GetTagAttributeValue(string tag, string attribute)
		{
			if (string.IsNullOrEmpty(tag)) return null;
			int pos = tag.IndexOf(attribute + "=");
			if (pos == -1) return null;
			int pstart = pos + attribute.Length;

			string quoteChar = "\"";
			for (; pstart < tag.Length; pstart++)
			{
				string c = tag.Substring(pstart, 1);
				if (c == "\"")
				{
					break;
				}
				else if (c == "'")
				{
					quoteChar = "'";
					break;
				}
			}
			if (pstart == tag.Length - 1) return null;
            pstart++; //increment to actual string

			int pend = Math.Min(tag.Length, tag.IndexOf(quoteChar, pstart + 1));
			if (pend == -1) return null;
			return tag.Substring(pstart, pend - pstart);
		}

		#endregion


		#region Root element definition


		private Dictionary<string, ControlMap> _rootElements = null;

		/// <summary>
		/// Dictionary keyed to markupTag of all registered Root Elements
		/// </summary>
		public Dictionary<string, ControlMap> RootElements
		{
			get
			{
				if (_rootElements == null)
				{
					_rootElements = new Dictionary<string, ControlMap>();
				}
				return _rootElements;
			}
		}



		private ControlMap _rootElement = null;

		/// <summary>
		/// ControlMap of the current root-level Control.
		/// If this is not explicitly set this will evaluate to a <c>BaseContainerControl</c>.
		/// </summary>
		public ControlMap RootElement
		{
			get
			{
				if (_rootElement != null)
				{
					return _rootElement;
				}
				else
				{
					return ControlFactory.GenericRootElementMap;
				}
			}
		}
		/// <summary>
		/// Sets the current RootElement control map.
		/// </summary>
		/// <param name="rootMap"></param>
		internal void SetRootElement(ControlMap rootMap)
		{
			_rootElement = rootMap;
			//confirm the RootContext and add this control map
			if (!Catalog.ContainsKey(ParseContext.RootContext))
			{
				Catalog.Add(ParseContext.RootContext, new ControlCatalog(ParseContext.RootContext));
			}
			Catalog[ParseContext.RootContext].Add(rootMap);
		}

		#endregion

		/// <summary>
		/// Builds a fully parsed control tree from the passed markup.
		/// Use this method as a general Factory entry point to build from
		/// an unrecognized piece of markup.
		/// </summary>
		/// <remarks>
		/// This can only parse from the root element or from elements
		/// in the default context.
		/// </remarks>
		/// <param name="markup"></param>
		/// <returns></returns>
		public BaseGadgetControl BuildControlTree(string markup)
		{
			if (string.IsNullOrEmpty(markup))
			{
				return new RootElementMaster();
			}

			string tag = ControlFactory.GetTagName(markup);

			ControlMap map = GetControlMap(tag, ParseContext.RootContext);
			if (map == null || map.ControlType == typeof(GadgetLiteral))
			{
				map = GetControlMap(tag, ParseContext.DefaultContext);
			}
			if (map == null || map.ControlType == typeof(GadgetLiteral))
			{
				return new GadgetLiteral(markup);
			}
			BaseGadgetControl control = Activator.CreateInstance(map.ControlType) as BaseGadgetControl;
			if (!(control is BaseContainerControl))
			{
				string outer = GetDefaultContainerMarkupTag();
				if (!string.IsNullOrEmpty(outer))
				{
					control = BuildControlTree(string.Format(
						"<{0}>{1}</0>", outer, markup));
					if (control is BaseContainerControl)
					{
						((BaseContainerControl)control).MyControlFactory = this;
					}
				}
			}
			else
			{
				((BaseContainerControl)control).MyControlFactory = this;
			}
			if (control is RootElementMaster)
			{
				control.MyRootMaster = (RootElementMaster)control;
				//control.MyRootMaster.ma
			}
			control.LoadTag(markup);
			
			return control;
		}



		#region Reserved ControlMaps

		/// <summary>
		/// Generic RootElement corresponding to a BaseContainerControl.
		/// This is used as the Root if the root has not been set in a ControlFactory instance.
		/// </summary>
		static ControlMap GenericRootElementMap;

		/// <summary>
		/// Reserved element ControlMap corresponding to a Literal control
		/// </summary>
		static ControlMap LiteralElementMap;

		#endregion


		private ControlFactoryCounters _counters = new ControlFactoryCounters();

		/// <summary>
		/// Hook to enable/disable and invoke control counters.
		/// Per-type control counting is enabled with this object,
		/// but actual counts are stored in the ControlCatalog.
		/// </summary>
		public ControlFactoryCounters Counters
		{
			get
			{
				return _counters;
			}
		}


		private List<string> _loadErrors = null;

		/// <summary>
		/// Accessor for loadErrors.
		/// Performs lazy load upon first request
		/// </summary>
		public List<string> LoadErrors
		{
			get
			{
				if (_loadErrors == null)
				{
					_loadErrors = new List<string>();
				}
				return _loadErrors;
			}
		}



		/// <summary>
		/// Core factory method for creating a control instance
		/// Create an instance based on the ControlMap.
		/// TODO: Make this instance?  Maybe not required
		/// </summary>
		/// <remarks>
		/// This is the core overload for creating a control.
		/// Currently uses reflection and Activator.CreateInstance, but could
		/// optimize to use a switch statement (requires more maintenance).
		/// </remarks>
		/// <param name="map"></param>
		/// <param name="markup"></param>
		/// <param name="offset">OffsetItem containing all the pre-processed offsets for this control</param>
		/// <param name="master">GadgetMaster object representing the root element.  If this is null each control will auto-create their own master</param>
		/// <returns></returns>
		private BaseGadgetControl CreateControlInstance(ControlMap map, string markup, OffsetItem offset, RootElementMaster master)
		{
			if (null == master)
			{
				throw new ArgumentNullException("RootMaster must be supplied to create a control");
			}
			BaseGadgetControl control = Activator.CreateInstance(map.ControlType) as BaseGadgetControl;
			if (null != control)
			{
				if (null != master)
				{
					control.MyRootMaster = master;
					master.ReconfirmControlFactorySet(this);
				}

				if (null != offset)
				{
					control.MyOffset = offset;
				}

				//Update counters
				if (this.Counters.IsGlobalCounterEnabled)
				{
					Counters.IncrementGlobalCount(); //global control count
				}
				if (this.Counters.IsControlUseCountingEnabled)
				{
					Counters.IncrementControlUsageCount(map.ControlType);
				}

				//set the Factory pointer
				if (control is BaseContainerControl)
				{
					((BaseContainerControl)control).MyControlFactory = this;
				}

				control.LoadTag(markup);
				if (control is BaseContainerControl && !control.IsParsed)
				{
					if (!control.IsParsed)
					{
						((BaseContainerControl)control).Parse();
					}
				}
			}
			return control;
		}


		/// <summary>
		/// Create a control instance in the Template context
		/// </summary>
		/// <param name="offsetKey"></param>
		/// <param name="markup"></param>
		/// <returns></returns>
		public BaseGadgetControl CreateControlFromKeyString(string offsetKey, string markup, RootElementMaster master)
		{
			return CreateControlFromKeyString(offsetKey, markup, ParseContext.DefaultContext, master);
		}


		/// <summary>
		/// Create a skeletal control for an unrecognized markup sequence
		/// </summary>
		/// <param name="markup"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		private BaseGadgetControl CreateUnrecognizedControl(string markup, ParseContext context)
		{
			if (ParseContext.DefaultContext == context)
			{
				return new GadgetLiteral(markup);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Create an instance
		/// </summary>
		/// <param name="offsetKey"></param>
		/// <param name="markup"></param>
		/// <returns></returns>
		public BaseGadgetControl CreateControlFromKeyString(string offsetKey, string markup, ParseContext context, RootElementMaster master)
		{
			if (!Catalog[context].ControlMapOffsetKey.ContainsKey(offsetKey))
			{
				CreateUnrecognizedControl(markup, context);
			}

			return CreateControlInstance(Catalog[context].ControlMapOffsetKey[offsetKey], markup, null, master);
		}

		/// <summary>
		/// Creates an instance of the proper control based on tag
		/// </summary>
		/// <param name="markupTag"></param>
		/// <param name="markup">Raw markup to load into the control</param>
		/// <returns></returns>
		[Obsolete("We already need to parse full tag to handle dependent attributes")]
		public BaseGadgetControl CreateControl(string markupTag, string markup, ParseContext context, RootElementMaster master)
		{
			ControlMap map;
			markupTag = markupTag.ToLowerInvariant();
            
			if (Catalog.ContainsKey(context) && Catalog[context].ControlMapMarkup.ContainsKey(markupTag))
			{
				map = Catalog[context].ControlMapMarkup[markupTag];
			}
			else
			{
				map = ControlFactory.LiteralElementMap;
			}

			return CreateControlInstance(map, markup, null, master);
		}

		/// <summary>
		/// Creates an instance of the proper control based on tag
		/// </summary>
		/// <param name="markup">Raw markup to load into the control</param>
		/// <returns></returns>
		public BaseGadgetControl CreateControl(string markup, ParseContext context, RootElementMaster master)
		{
			string offsetKey = GetOffsetKey(markup, context);

			ControlMap map;
            if (Catalog.ContainsKey(context) && Catalog[context].ControlMapOffsetKey.ContainsKey(offsetKey))
			{
				map = Catalog[context].ControlMapOffsetKey[offsetKey];
			}
			else
			{
				map = ControlFactory.LiteralElementMap;
			}
			return CreateControlInstance(map, markup, null, master);
		}


		/// <summary>
		/// Creates an instance of the proper control based on OffsetItem
		/// </summary>
		/// <param name="offset">OffsetItem for this control.  This may include fully parsed child offsets</param>
		/// <param name="markup">Raw markup to load into the control</param>
		/// <returns></returns>
		public BaseGadgetControl CreateControl(OffsetItem offset, string markup, ParseContext context, RootElementMaster master)
		{
			ControlMap map;
			if (Catalog.ContainsKey(context) &&
				Catalog[context].ControlMapOffsetKey.ContainsKey(offset.OffsetKey))
			{
				map = Catalog[context].ControlMapOffsetKey[offset.OffsetKey];
			}
			else{
				map = ControlFactory.LiteralElementMap;
			}

			return CreateControlInstance(map, markup, offset, master);
		}

		/// <summary>
		/// Return the type of control to render.
		/// This overload always assumes the context is a template script.
		/// <c>ParseContext.InTemplate</c>
		/// </summary>
		/// <param name="markupTag"></param>
		/// <returns></returns>
		public Type GetControlType(string markupTag)
		{
			return GetControlType(markupTag, ParseContext.DefaultContext);
		}

		/// <summary>
		/// Return the type of control to render
		/// </summary>
		/// <param name="markupTag">Raw tag to look for</param>
		/// <param name="context">Parsing context for valid controls</param>
		/// <returns></returns>
		public Type GetControlType(string markupTag, ParseContext context)
		{
			if (String.IsNullOrEmpty(markupTag))
			{
				return typeof(GadgetLiteral);
			}

			string fullTag;
			if (markupTag.IndexOf("<") == -1)
			{
				fullTag = null;
			}
			else
			{
				fullTag = markupTag;
				markupTag = ControlFactory.GetTagName(markupTag);
			}

			markupTag = markupTag.ToLowerInvariant();

            if (Catalog.ContainsKey(context))
            {
                if (!Catalog[context].ControlMapMarkup.ContainsKey(markupTag))
                {
                    if (fullTag != null)
                    {
                        string offsetKey = GetOffsetKey(fullTag, context);
                        if (Catalog[context].ControlMapOffsetKey.ContainsKey(offsetKey))
                        {
                            return Catalog[context].ControlMapOffsetKey[offsetKey].ControlType;
                        }
                    }
                }
                else
                {
                    return Catalog[context].ControlMapMarkup[markupTag].ControlType;
                }
            }
			return typeof(GadgetLiteral);
		}


		/// <summary>
		/// Gets the OffsetKey value for a control based on Attribute alternate defintion
		/// for the control.  Return the Literal reserved OffsetKey if there is not
		/// an alternate attribute definition found.
		/// This is only a valid call when parsing within default context (Template)
		/// </summary>
		/// <param name="markup"></param>
		/// <returns></returns>
		public string GetAlternateAttributeOffsetKey(string markup)
		{
			if (string.IsNullOrEmpty(markup))
			{
				return ControlFactory.RESERVEDKEY_LITERAL;
			}
			//create an instance of a control.  Controls internally parse all attributes
			BaseGadgetControl control = new GadgetLiteral(markup);
			List<AltAttributeMapEntry> candidates = new List<AltAttributeMapEntry>();
			foreach (KeyValuePair<string, AltAttributeMapEntry> keyset in AlternateAttributeMap)
			{
				if (!String.IsNullOrEmpty(control.GetAttribute(keyset.Key)))
				{
					candidates.Add(keyset.Value);
				}
			}
			if (candidates.Count == 0)
			{
				return ControlFactory.RESERVEDKEY_LITERAL;
			}
			else if (candidates.Count == 1)
			{
				return candidates[0].OffsetKey;
			}
			else
			{
				int maxWeight = Int32.MinValue;
				int pos = -1;
				for (int i = 0; i < candidates.Count; i++)
				{
					if (candidates[i].PrecedenceWeight > maxWeight)
					{
						maxWeight = candidates[i].PrecedenceWeight;
						pos = i;
					}
				}
				if (pos > -1)
				{
					return candidates[pos].OffsetKey;
				}
				else
				{
					return ControlFactory.RESERVEDKEY_LITERAL;
				}
			}
		}


		/// <summary>
		/// Returns the string OffsetKey associated with the given control type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public string GetOffsetKey(Type type, ParseContext context)
		{
			if (Catalog[context].ControlMapControlType.ContainsKey(type))
			{
				return Catalog[context].ControlMapControlType[type].OffsetKey;
			}
			return ControlFactory.RESERVEDKEY_LITERAL;
		}

		/// <summary>
		/// Return the string OffsetKey associated with the given markupTag.
		/// The markupTag value may be either just the tag name or the entire tag.
		/// In the case of controls which have dependent attribute definitions 
		/// (defined with <c>AttributeTagDependentAttribute</c>) it must be
		/// the complete start element tag, with angle brackets and attributes.
		/// </summary>
		/// <param name="markupTag"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public string GetOffsetKey(string markupTag, ParseContext context)
		{
			bool isFullTag = (markupTag.IndexOf("<") > -1);
			if(!isFullTag){
				markupTag = markupTag.ToLowerInvariant();
				if(Catalog[context].ControlMapMarkup.ContainsKey(markupTag)){
					return Catalog[context].ControlMapMarkup[markupTag].OffsetKey;
				}
				else{
					if (markupTag == RootElement.MarkupTag.ToLowerInvariant())
					{
						return RootElement.OffsetKey;
					}
					else
					{
						return ControlFactory.RESERVEDKEY_LITERAL;
					}
				}
			}

			string tag = ControlFactory.GetTagName(markupTag).ToLowerInvariant();
			if (DependentAttributeMap.ContainsKey(tag))
			{
				GadgetLiteral testControl = new GadgetLiteral(markupTag);
				List<ControlMap> dependentMaps = DependentAttributeMap[tag];
				string bestMatch = ControlFactory.RESERVEDKEY_LITERAL;
				int bestMatchCount = 0; //degree of specificity in matching
				foreach (ControlMap item in dependentMaps)
				{
					if (item.AttributeTagDependentKey.IndexOf("|") == -1)
					{
						if (item.AttributeTagDependentValue == testControl.GetAttribute(item.AttributeTagDependentKey))
						{
							if (0 == bestMatchCount && ControlFactory.RESERVEDKEY_LITERAL == bestMatch)
							{
								bestMatch = item.OffsetKey;
								continue;
							}
						}
					}
					else
					{
						//Handle case where control has multiple dependent attributes
						bool foundItem = true;
						string[] keys = item.AttributeTagDependentKey.Split('|');
						string[] vals = item.AttributeTagDependentValue.Split('|');
						for (int i = 0; i < keys.Length; i++)
						{
							string thisVal = testControl.GetAttribute(keys[i]);
							if(string.IsNullOrEmpty(thisVal) ||
								(vals[i].Length > 0 && !thisVal.Equals(vals[i], StringComparison.InvariantCultureIgnoreCase)))
							{
								//not this item - drop out to outer loop
								foundItem = false;
								break;
							}
						}
						if(foundItem && bestMatchCount < keys.Length){
							bestMatchCount = keys.Length;
							bestMatch = item.OffsetKey;
							continue;
						}

					}
				}
				return bestMatch;
			}
			else
			{
				if (Catalog[context].ControlMapMarkup.ContainsKey(tag))
				{
					return Catalog[context].ControlMapMarkup[tag].OffsetKey;
				}
				else
				{
					string altOffset = GetAlternateAttributeOffsetKey(markupTag);
					if (altOffset != ControlFactory.RESERVEDKEY_LITERAL)
					{
						return altOffset;
					}
				}
			}
			if (tag == RootElement.MarkupTag)
			{
				return RootElement.OffsetKey;
			}
			else
			{
				return ControlFactory.RESERVEDKEY_LITERAL;
			}
		}


		/// <summary>
		/// Internal convenience method for getting the ControlMap associated with
		/// the markupTag.
		/// </summary>
		/// <param name="markupTag"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		internal ControlMap GetControlMap(string markupTag, ParseContext context)
		{
			string offsetKey = GetOffsetKey(markupTag, context);
			if (Catalog[context].ControlMapOffsetKey.ContainsKey(offsetKey))
			{
				return Catalog[context].ControlMapOffsetKey[offsetKey];
			}
			else if (offsetKey == RootElement.OffsetKey && RootElement.MarkupTag == GetTagName(markupTag))
			{
				//todo - fix this to recognize multiple roots
				return RootElement;
			}
			else
			{
				return LiteralElementMap;
			}
		}

		/// <summary>
		/// Gets a ControlMap from the master list of RegisteredControlOffsetKeys.
		/// Returns associated ControlMap, or LiteralElementMap, if not found.
		/// </summary>
		/// <param name="offsetKey"></param>
		/// <returns></returns>
		internal ControlMap GetControlMap(string offsetKey)
		{
			if (RegisteredControlOffsetKeys.ContainsKey(offsetKey))
			{
				return RegisteredControlOffsetKeys[offsetKey];
			}
			else
			{
				return LiteralElementMap;
			}
		}

		/// <summary>
		/// Gets a ControlMap from the master list of RegisteredControlOffsetKeys.
		/// Returns associated ControlMap, or LiteralElementMap, if not found.
		/// </summary>
		/// <param name="offsetKey"></param>
		/// <returns></returns>
		internal ControlMap GetControlMap(Type controlType)
		{
			if (RegisteredControlTypes.ContainsKey(controlType))
			{
				return RegisteredControlTypes[controlType];
			}
			else
			{
				return LiteralElementMap;
			}
		}


		/// <summary>
		/// Returns the string OffsetKey of the control type defined as the context root
		/// for the given context value.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public string GetContextGroupOffsetKey(ParseContext context)
		{

			if (Catalog.ContainsKey(context))
			{
				return Catalog[context].ContextOffsetKey;
			}
			return ControlFactory.RESERVEDKEY_GENERIC_CONTAINER;
		}

		/// <summary>
		/// Determines if the given type is a ContextGroup container.
		/// Returns true if it is, otherwise false.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool IsContextGroupContainerControl(Type type)
		{
			foreach (KeyValuePair<ParseContext, ControlCatalog> keyset in Catalog)
			{
				if (type.Equals(keyset.Key.ContainerControlType))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Determines if the given type is a ContextGroup container.
		/// Returns true if it is, otherwise false.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool IsContextGroupContainerControl(string offsetKey)
		{
			if (RegisteredControlOffsetKeys.ContainsKey(offsetKey))
			{
				return RegisteredControlOffsetKeys[offsetKey].IsContextGroupContainer;
			}
			return false;
		}


		/// <summary>
		/// Returns true if the given <c>ParseContext</c> is allowed to have Literal controls.
		/// Returns false if context only allows defined tag.
		/// TODO: Make this attribute/manifest driven.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public bool AllowLiteralTags(ParseContext context)
		{
			//if (context.Equals(ParseContext.DefaultContext)
			//    || context.Equals(new ParseContext(typeof(BaseContainerControl)))
			//    || context.Equals(new ParseContext(typeof(BaseTemplate))))
			//{
			//    return true;
			//}
			//return false;
			return true;
		}


		/// <summary>
		/// Returns true if the given <c>ParseContext</c> is allowed to use custom tag definitions
		/// Returns false if context only allows defined controls.
		/// TODO: Make this attribute/manifest driven.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public bool AllowCustomTags(ParseContext context)
		{
			return AllowLiteralTags(context);
			//if (context.Equals(ParseContext.DefaultContext)
			//    || context.Equals(new ParseContext(typeof(BaseContainerControl)))
			//    || context.Equals(new ParseContext(typeof(OSML.Controls.OsTemplate))))
			//{
			//    return true;
			//}
			//return false;
		}


		/// <summary>
		/// Create an appropriate root offset for the given tag and context.
		/// The OffsetItem returned will correspond to one of the defined
		/// ContextGroup controls.
		/// </summary>
		/// <param name="markupTag"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public OffsetItem CreateRootOffset(string markupTag, ParseContext context)
		{
			ControlMap map = GetControlMap(markupTag, context);

			ParseContext currentContext = new ParseContext(map.ControlType);
			return CreateRootOffset(currentContext);
		}


		public OffsetItem CreateRootOffset(ParseContext context)
		{
			OffsetItem root = new OffsetItem();
			root.Position = 0;
			root.ParentOffset = root; //cyclic on top
			root.OffsetKey = GetContextGroupOffsetKey(context);			

			return root;
		}



		private CustomTagFactory _globalCustomTagFactory = null;

		/// <summary>
		/// Global CustomTagFactory for any globally registered custom tags.
		/// </summary>
		public CustomTagFactory GlobalCustomTagFactory
		{
			get
			{
				if (_globalCustomTagFactory == null)
				{
					_globalCustomTagFactory = new CustomTagFactory();
					_globalCustomTagFactory.MyControlFactory = this;
				}
				return _globalCustomTagFactory;
			}
		}

		




	}
}
