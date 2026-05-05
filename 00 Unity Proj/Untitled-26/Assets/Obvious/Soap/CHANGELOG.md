## [3.8.0] - 2026-02-22
- Fixed auto path refresh when using single column layout project window
- Modified Event Listeners to have virtual protected methods
- Modified Scriptable Variables and Collections reset to be on scene unload

## [3.7.1] - 2026-01-19
- Fixed occasional null ref compatibility with Odin and ReadOnly Assets
- Fixed ScriptableEvents not displaying all listeners in the inspector
- Modified MenuItem name from 'Obvious Game' to 'Obvious' for consistency

## [3.7.0] - 2026-01-13
- Fixed Soap Asset Creator for Unity 6 compatibility
- Modified Reset On mode naming for better clarity
- Added Read-Only support for Scriptable Variables, Scriptable Lists, and Scriptable Dictionaries
- Added a menu item on all MonoBehaviours and ScriptableObjects to create all referenced Soap assets and sub-assets in one click
- Added a Settings button to the Soap Wizard
- Removed the Reset Value button from the Scriptable Variable drawer to simplify the UI

## [3.6.1] - 2025-11-05
- Fixed ignoring anything that is no an .asset when GUID are assigned
- Fixed Assembly name for GC2 compatibility
- Fixed Singleton message warning on dev builds
- Added Getter for GC2 Scripts
- Added virtual FilePath property to ScriptableSave<T>
- Added Static methods to create RuntimeEvents

## [3.6.0] - 2025-10-05
- Improved Soap Asset Creator UX
- Improved Scriptable Save API
- Added Scriptable Singleton
- Added Clear MenuItem for Scriptable Collections
- Added SetValueWithoutNotify method for Scriptable Variables
- Optimized Scriptable Variable to prevent boxing for value types reducing GC allocations

## [3.5.3] - 2025-07-29
- Fixed ScriptableVariable using improper equality checks
- Fixed DeleteSubAsset calling the wrong method

## [3.5.2] - 2025-07-27
- Added Find References in Scene, Project, and Both
- Added Link URL support for documentation
- Added compatibility with the UEE Package
- OnValueChanged is now also triggered when the Local Value of a Variable Reference changes in the Inspector
- Added DeleteSubAsset method for SubAssets
- Made SoapUtils methods public
- Replaced Odin [ShowIf] attribute with [HideIf] on bindings
- Fixed incorrect value usage in the Ratio property of Int and Float Variables
- Fixed ScriptableCollection resetting even when "None" reset mode was selected
- Fixed ScriptableList displaying an incorrect count
- Fixed serialization error icon appearing in ScriptableCollections property drawers 
- Removed deprecated scripts

## [3.5.1] - 2025-03-11
- Added a SelectParent method for SubAssets
- Added keeping the Namespace in the SoapTypeCreator persistent
- Added compatibly with nullable types Scriptable Variables
- SubAssets backing fields names are now properly displayed
- Fixed templates scripts now using Rider format
- Fixed allowing to inherit from ScriptableCollection
- Fixed Odin [HideLabel] bug with SubAssets icon
- Fixed ScriptableCollection resetting when "None" reset mode was selected

## [3.5.0] - 2025-02-09
- Added Scriptable Dictionary type
- Added Scriptable Dictionary Creator Window
- Added new Example Scene for Scriptable Dictionaries
- Added Set Icon menu item to assign icons to existing Soap CS files
- Added auto-assignment of icons when creating new Soap types with the Soap Asset Creator
- Added Soap Asset Creator
- Added Soap Asset Creator menu item at the top of the menu items list
- Added optional Prefixes in Soap Settings
- Added new category in Soap Wizard (Collections) that groups lists and dictionaries
- Changed ScriptableBase now automatically inherits from SerializedScriptableObject if Odin is present
- Fixed changing reset mode for collections when not reloading domain
- Fixed EditorPrefs used for all projects (now per project)
- Fixed ScriptableList now properly allows duplicates

## [3.4.1] - 2025-01-17
- Added safety assigning for tags if not found in settings.
- Fixed MinReference property returning wrong value for Int and Float Variables

## [3.4.0] - 2025-01-12
- Added [AutoTag] attribute for auto tagging SOAP SO on creation
- Added a way to add new tag from the tag dropdown
- Added support to set the tag of multiple SOAP SO at once
- Added Bindings icons
- Changed Min and MaxReference in Int and Float variables to a public property (Get/Set)
- Fixed Delay in EventListeners not initializing with local value
- Fixed error when drawing a list of ScriptableVariables
- Fixed [ShowIf] attribute not working with custom inspectors
- Fixed ScriptableBaseDrawer not compatible with [ReadOnly] attribute
- Added Runtime Injectors and [RuntimeInjectable] attribute

## [3.3.1] - 2024-11-11
- Added override Get Value in Float and Int Variable
- Added [HelpUrl] attribute to classes
- Fixed infinite loop caused by circular references in FindReferences
- Fixed direct text assignment in BindTextMeshPro to use SetText()
- Fixed enum not usable without needing [Serializable] attribute

## [3.3.0] - 2024-10-11
- Added Support for Game Creator 2
- Added Variable Reference usage for min and max Value of Int and Float Variables
- Added option to use MaxValue from Int and Float Variables in Bindings
- Added option to enable raising Scriptable Events from the inspector
- Added shortcuts support to Soap Wizard 
- Added Soap Settings in Preferences Window
- Added FloatReference for Delay field in EventListeners
- Fixed References with Playmaker Actions
- Fixed OnValueChanged Object display with Playmaker Actions
- Fixed Runtime value being shown in the inspector
- Fixed Bindings not always refreshing value when modified from Inspector

## [3.2.2] - 2024-09-22
- Added more accessibility for Soap Type Creator
- Removed Documentation PDF's
- Fixed Minimal Drawing mode
- Fixed Odin not showing Runtime Value

## [3.2.1] - 2024-09-17
- Added BindSlider has a MaxValue property
- Fixed UIToolkit Wrapper that prevent Soap property drawer
- Fixed rare null ref in Wizard
- Fixed GUID error when including other Sub Assets types with Variables

## [3.2.0] - 2024-09-14
- Added Scriptable Save
- Added ResettableScriptableObject
- Added Component Type in core Soap types
- Changed Soap Wizard's layout (cleaner and showing more data)
- Changed a lot of methods and fields to protected and or virtual
- Fixed Custom Editor in UIToolkit Drawers
- Fixed Runtime Variables being reset when the scene loads (default is not)
- Fixed Variable reference UseLocal not propagating OnValueChanged
- Fixed Variables always being set as dirty even if not changed
- Fixed ability to create instances of Generic Soap SO by pressing the create shortcut button

## [3.1.0] - 2024-07-14
- Added Runtime Variables (example scene and documentation)
- Added drawing other parameters even if type is not serializable
- Changed Serialization error to warning
- Fixed various errors due to sub assets checking
- Fixed using Soap SO in Odin serialized dictionary
- Fixed using Soap with Odin. (Now drawing with Odin if it is there)

## [3.0.0] - 2024-06-10
- Added Scriptable Enums
- Added Scriptable SubAssets
- Added typed EventListeners for Playmaker Actions
- Added Quaternion as included type
- Added Soap Type Creator Window
- Changed OnValueChanged from private to protected
- Fixed occasional null ref when FindingReference
- Fixed missing update in serialized Object inspector
- Improved Soap Wizard UX

## [2.7.2] - 2024-04-15
- Fixed playmaker actions related to ScriptableLists
- Moved root folder out of Plugins
- Removed Tests from public package

## [2.7.1] - 2024-04-02
- Added gameObject reference to Debug.Log for Scriptable Variables
- Added custom path setting for "Create" shortcut in inspector
- Added Null Check for asset reference
- Renamed ResetToInitialValue() to ResetValue()
- Fixed Odin compatibility for Variable Ref and Scriptable Events
- Fixed drawing properties on ScriptabLists for monobehavior Type
- Removed Reset public, made it internal

## [2.7.0] - 2024-03-02
- Added Description for all Soap scriptable objects 
- Added custom inspector for runtime scene objects variables
- Added missing documentation (PlayerVariable, PreviousValue and description)
- Fixed error with SoapSettings sometimes null
- Fixed Namespace wrapping for generated classes

## [2.6.1] - 2024-02-14
- Added Vector2Int as part of core
- Removed unnecessary [Serializable] attributes on abstract classes
- Fixed [SerializeField] for List<T>

## [2.6.0] - 2024-02-12
- Added TryAdd methods for List
- Added ScriptableList<T> inherits from IList<T>
- Added Option to override destination path when creating new type
- Added Optional namespace when creating new type
- Added nameof(T) for generated type to support refactoring
- Added Find references of Soap SO (in scenes and assets)
- Fixed Dynamic space for Vector2 and 3 in variable property drawer
- Changed Moved Soap into Plugins folder
- Changed Menu name to Obvious Game
- Changed ScriptableList Add() does not check for already added items anymore 

## [2.5.0] - 2024-01-12
- Added Scriptable list use a hashset for gain in performance 
- Added FAQ to Soap Window
- Added Option to override Save ID (GUID) of a scriptable variable 
- Added Delay for EventListeners
- Added Categories
- Added Integration with Playmaker
- Added ForEach support for scriptable Lists
- Changed Soap Wizard's layout (using more icons for better UX)

## [2.4.0] - 2023-12-20
- Added Unit Test for Type Creation
- Added Scene Documentation Button
- Added manual naming on creation
- Added settings in soap Window
- Added option to generate class from Create New Wizard
- Added Unit Tests for Type creation
- Moved Scene documentations in Documentation folder
- Changed min value for clamp from minValue to 0 (for floats and ints)

## [2.3.1] - 2023-11-09
- Fix Nested Folder for Soap Wizard
- Fix Pascal Case name of the variable in the name but not type
- Added Unit Test for Type Creation

## [2.3.0] - 2023-11-09
- Updated package to latest LTS version of Unity
- Updated Soap Icon
- Added documentation in code and tooltips
- Added Ratio property for IntVariable and FloatVariable
- Added Min and Max property for IntVariable and FloatVariable

## [2.2.1] - 2023-09-07
- Revert SerializeField for variable and lists
- Fixed compatibility with Fast Script Reload
- Fixed error when adding component
- Added compatibility with Odin Inspector

## [2.2.0] - 2023-08-07
- Fixed use SerializeReference instead of SerializeField for variable and lists
- Fixed not use Linq for IsEmpty property of lists

## [2.1.0] - 2023-06-18
- Added BindToInputField Component
- Fixed error when displaying non serializable classes
- Fixed overriding all custom property drawers
- Removed PlayModeResetter 
- Removed FastPlayMode Scene

## [2.0.0] - 2023-05-23
- Added ScriptableVariable Default value field
- Added Icons for Windows
- Added ScriptableVariables now reset when exiting PlayMode
- Fixed SoapSettings created multiple times
- Fixed cached editor
- Fixed null refs due to repaint logic
- Fixed various colors
- Fixed naming standards of EventListeners
- Removed PlayModeResetter logic
- Removed ScriptableVariable InitialValue field

## [1.5.3] - 2023-05-10
- Fixed Custom editor performances issues
- Fixed expanding ScriptableEvents from EventListeners
- Added Custom property drawer for each type
- Added Raise button for ScriptableEvent property drawer
- Added Count label for ScriptableList property drawer

## [1.5.2] - 2023-05-02
- Fixed Editor namespace compile error 
- Fixed Editor code preventing builds

## [1.5.1] - 2023-04-29
- Fixed create SO at selected folder instead of predefined path
- Fixed create new Types at selected folder instead of predefined path
- Fixed settings can only be modified from Soap Window
- Fixed Editor namespace
- Fixed naming of variable reference created from inspector button
- Added Soap Window
- Added MinMax property for IntVariable and FloatVariable

## [1.5.0] - 2023-04-08
- Fixed null ref debugging events
- Fixed InitialValue is public
- Fixed Modifying the value of a SV by code shows in Version Control
- Added create button from classes (SoapPropertyDrawer)
- Added Embedded inspector for all Soap SO (SoapPropertyDrawer)
- Added SoapSettings
- Added ScriptableVariable Display Mode (Default & Minimal)
- Added Search bar in SoapWizard
- Added debug for ScriptableEventNoParam

## [1.4.1] - 2023-02-24
- Added favorite option in Soap Wizard
- Fixed path saving in Soap Wizard
- Fixed Unit Tests under Editor Folder
- Fixed Events raise button disabled when not in play mode
 
## [1.4.0] - 2023-01-30
- Added Soap Wizard
- Added Events Debug Window visual
- Added file name for ScriptableList and Events
- Fixed base type of ScriptableVariableDrawer
- Fixed Rename enum CustomVariableType to follow C# standards

## [1.3.3] - 2023-01-19
- Added AddRange and RemoveRange methods for ScriptableList
- Removed parameter of the event OnItemCountChanged
- Fixed support for multiple instances of PlayModeResetter
- Fixed Debug Logs are now being displayed when subscribing to OnRaised in ScriptableEvents by code. 

## [1.3.2] - 2022-12-17
- Added Implicit operator for ScriptableVariables
- Added Version number on documentation
- Added ResetToInitialValue Button on ScriptableVariables
- Fixed Obstacle prefab structure
- Fixed icons are now being used
- Fixed minor bugs and project folders
- Fixed OnSceneLoaded is now protected

## [1.3.1] - 2022-11-20
- Added VariableReference base class
- Added IsEmpty property on ScriptableLists
- Added Undo on Bindings custom inspectors
- Added BindRendererColor
- Added BindGraphicColor
- Removed BindImageColor and ColorChanger
- Updated documentation
- Updated BindComparisonToUnityEvent

## [1.3.0] - 2022-11-09
- Added custom icons
- Added option to subscribe to ScriptableEvents by code	
- Fix error when modifying Bindings component at runtime

## [1.2.1] - 2022-10-29
- Added IsClamped bool to clamping for IntVariables and FloatVariables
- Added IsClamped bool for BindText/TextMeshPro components
- Added Discord and Asset store link buttons in Scenes
- Fix ScriptableList Drawing GameObjects
- Fix ScriptableVariable Guid serialization and generation


## [1.2.0] - 2022-10-20
- Added Min & Max clamping for IntVariables and FloatVariables
- Added Min & Max clamping for BindText/TextMeshPro components
- Added Uid as PlayerPrefs key for ScriptableVariables
- Added warning in PlayModeResetter
- Added Discord and Asset store link buttons in Scenes
- Updated custom inspector for ScriptableLists
- Fixed various custom inspectors

## [1.1.0] - 2022-10-10
- Fixed BindText and BindTextMeshPro when binding to a StringVariable
- Added default GameObjectScriptableEvent and EventListenerGameObject
- Added package.json and changelog
- Updated custom inspector for SO variables
- Fixed various bug 
- Updated documentation
- Uploaded with 2019.4

## [1.0.0] - 2022-09-27
- Initial Release
