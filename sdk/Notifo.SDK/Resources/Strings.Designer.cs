﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Notifo.SDK.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Notifo.SDK.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to You were trying to subscribe to the NotificationOpened event, but not supply the push notification events provider. Use the SetPushEventsProvider method to inject push events provider implementation..
        /// </summary>
        internal static string NotificationOpenedEventSubscribeException {
            get {
                return ResourceManager.GetString("NotificationOpenedEventSubscribeException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You were trying to unsubscribe from the NotificationOpened event, but not supply the push notification events provider. Use the SetPushEventsProvider method to inject push events provider implementation..
        /// </summary>
        internal static string NotificationOpenedEventUnsubscribeException {
            get {
                return ResourceManager.GetString("NotificationOpenedEventUnsubscribeException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You were trying to subscribe to the NotificationReceived event, but not supply the push notification events provider. Use the SetPushEventsProvider method to inject push events provider implementation..
        /// </summary>
        internal static string NotificationReceivedEventSubscribeException {
            get {
                return ResourceManager.GetString("NotificationReceivedEventSubscribeException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You were trying to unsubscribe from the NotificationReceived event, but not supply the push notification events provider. Use the SetPushEventsProvider method to inject push events provider implementation..
        /// </summary>
        internal static string NotificationReceivedEventUnsubscribeException {
            get {
                return ResourceManager.GetString("NotificationReceivedEventUnsubscribeException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Received notification: {0}.
        /// </summary>
        internal static string ReceivedNotification {
            get {
                return ResourceManager.GetString("ReceivedNotification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Token refresh end. Executing count: {0}.
        /// </summary>
        internal static string TokenRefreshEndExecutingCount {
            get {
                return ResourceManager.GetString("TokenRefreshEndExecutingCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fail to refresh push token on the server..
        /// </summary>
        internal static string TokenRefreshFailException {
            get {
                return ResourceManager.GetString("TokenRefreshFailException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Token refresh start. Executing count: {0}.
        /// </summary>
        internal static string TokenRefreshStartExecutingCount {
            get {
                return ResourceManager.GetString("TokenRefreshStartExecutingCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Token was successfully refreshed. Token: {0}.
        /// </summary>
        internal static string TokenRefreshSuccess {
            get {
                return ResourceManager.GetString("TokenRefreshSuccess", resourceCulture);
            }
        }
    }
}
