//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Practices.Modeling.Dsl.Integration.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Practices.Modeling.Dsl.Integration.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Applies to a DSL Shape..
        /// </summary>
        internal static string DSLShape {
            get {
                return ResourceManager.GetString("DSLShape", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Applies to DSL Surface Area..
        /// </summary>
        internal static string DSLSurfaceArea {
            get {
                return ResourceManager.GetString("DSLSurfaceArea", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Intermediate Folders NotSupported In SolutionFolders..
        /// </summary>
        internal static string IntermediateFoldersNotSupportedInSolutionFolders {
            get {
                return ResourceManager.GetString("IntermediateFoldersNotSupportedInSolutionFolders", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid MEL moniker format: {0}..
        /// </summary>
        internal static string InvalidMelMonikerFormat {
            get {
                return ResourceManager.GetString("InvalidMelMonikerFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MissingTargetFolder.
        /// </summary>
        internal static string MissingTargetFolder {
            get {
                return ResourceManager.GetString("MissingTargetFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not locate the &apos;{0}&apos; model project. Update the element reference &apos;{1}&apos; with an existing project or specify a valid reference..
        /// </summary>
        internal static string ProjectNotFoundInMelMoniker {
            get {
                return ResourceManager.GetString("ProjectNotFoundInMelMoniker", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to outside the project directory. Linked items are not supported..
        /// </summary>
        internal static string SaveOutsideProjectDirException {
            get {
                return ResourceManager.GetString("SaveOutsideProjectDirException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TargetItemNotAFolder.
        /// </summary>
        internal static string TargetItemNotAFolder {
            get {
                return ResourceManager.GetString("TargetItemNotAFolder", resourceCulture);
            }
        }
    }
}