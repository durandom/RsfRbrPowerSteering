﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RsfRbrPowerSteering.ViewModel {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ViewModelTexts {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ViewModelTexts() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RsfRbrPowerSteering.ViewModel.ViewModelTexts", typeof(ViewModelTexts).Assembly);
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
        ///   Looks up a localized string similar to Do you really want to apply scaling? This will overwrite any existing personal FFB settings for the existing cars..
        /// </summary>
        internal static string ApplyScalingConfirmation {
            get {
                return ResourceManager.GetString("ApplyScalingConfirmation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to JSON File (*.json)|*.json.
        /// </summary>
        internal static string CarsFileDialogFilter {
            get {
                return ResourceManager.GetString("CarsFileDialogFilter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do you really want to clear the personal FFB sensitivity settings for all cars?.
        /// </summary>
        internal static string ClearFfbSensQuestion {
            get {
                return ResourceManager.GetString("ClearFfbSensQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Personal Car Settings.json.
        /// </summary>
        internal static string ExportCarsSaveFileDialogFileName {
            get {
                return ResourceManager.GetString("ExportCarsSaveFileDialogFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Export cars into….
        /// </summary>
        internal static string ExportCarsSaveFileDialogTitle {
            get {
                return ResourceManager.GetString("ExportCarsSaveFileDialogTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do you really want to import cars? This will overwrite any existing personal FFB settings for the imported cars..
        /// </summary>
        internal static string ImportCarsConfirmation {
            get {
                return ResourceManager.GetString("ImportCarsConfirmation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Import cars from….
        /// </summary>
        internal static string ImportCarsSaveFileDialogTitle {
            get {
                return ResourceManager.GetString("ImportCarsSaveFileDialogTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please enter a whole number between {0} and {1}..
        /// </summary>
        internal static string RangeMessageFormat {
            get {
                return ResourceManager.GetString("RangeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please enter a value..
        /// </summary>
        internal static string ValueMissingError {
            get {
                return ResourceManager.GetString("ValueMissingError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Version: {0}.{1}.{2}.
        /// </summary>
        internal static string VersionTextFormat {
            get {
                return ResourceManager.GetString("VersionTextFormat", resourceCulture);
            }
        }
    }
}