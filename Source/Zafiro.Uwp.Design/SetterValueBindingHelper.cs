﻿// Copyright (C) Microsoft Corporation. All Rights Reserved.
// This code released under the terms of the Microsoft Public License
// (Ms-PL, http://opensource.org/licenses/ms-pl.html).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace Zafiro.Uwp.Design
{
    /// <summary>
    /// Class that implements a workaround for a Silverlight XAML parser
    /// limitation that prevents the following syntax from working:
    ///    &lt;Setter Property="IsSelected" Value="{Binding IsSelected}"/&gt;.
    /// </summary>
    [ContentProperty(Name = "Values")]
    public class SetterValueBindingHelper
    {
        /// <summary>
        /// Gets or sets an optional type parameter used to specify the type
        /// of an attached DependencyProperty as an assembly-qualified name,
        /// full name, or short name.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Unambiguous in XAML.")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a property name for the normal/attached
        /// DependencyProperty on which to set the Binding.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets a Binding to set on the specified property.
        /// </summary>
        public Binding Binding { get; set; }

        /// <summary>
        /// Gets a Collection of SetterValueBindingHelper instances to apply
        /// to the target element.
        /// </summary>
        /// <remarks>
        /// Used when multiple Bindings need to be applied to the same element.
        /// </remarks>
        public Collection<SetterValueBindingHelper> Values
        {
            get
            {
                // Defer creating collection until needed
                if (null == values)
                {
                    values = new Collection<SetterValueBindingHelper>();
                }
                return values;
            }
        }

        /// <summary>
        /// Backing store for the Values property.
        /// </summary>
        private Collection<SetterValueBindingHelper> values;

        /// <summary>
        /// Gets the value of the PropertyBinding attached DependencyProperty.
        /// </summary>
        /// <param name="element">Element for which to get the property.</param>
        /// <returns>Value of PropertyBinding attached DependencyProperty.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "SetBinding is only available on FrameworkElement.")]
        public static SetterValueBindingHelper GetPropertyBinding(FrameworkElement element)
        {
            if (null == element)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return (SetterValueBindingHelper)element.GetValue(PropertyBindingProperty);
        }

        /// <summary>
        /// Sets the value of the PropertyBinding attached DependencyProperty.
        /// </summary>
        /// <param name="element">Element on which to set the property.</param>
        /// <param name="value">Value forPropertyBinding attached DependencyProperty.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "SetBinding is only available on FrameworkElement.")]
        public static void SetPropertyBinding(FrameworkElement element, SetterValueBindingHelper value)
        {
            if (null == element)
            {
                throw new ArgumentNullException(nameof(element));
            }
            element.SetValue(PropertyBindingProperty, value);
        }

        /// <summary>
        /// PropertyBinding attached DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty PropertyBindingProperty =
            DependencyProperty.RegisterAttached(
                "PropertyBinding",
                typeof(SetterValueBindingHelper),
                typeof(SetterValueBindingHelper),
                new PropertyMetadata(null, OnPropertyBindingPropertyChanged));

        /// <summary>
        /// Change handler for the PropertyBinding attached DependencyProperty.
        /// </summary>
        /// <param name="d">Object on which the property was changed.</param>
        /// <param name="e">Property change arguments.</param>
        private static void OnPropertyBindingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Get/validate parameters
            var element = (FrameworkElement)d;
            var item = (SetterValueBindingHelper)e.NewValue;

            if (null != item)
            {
                // Item value present
                if ((null == item.Values) || (0 == item.Values.Count))
                {
                    // No children; apply the relevant binding
                    ApplyBinding(element, item);
                }
                else
                {
                    // Apply the bindings of each child
                    foreach (var child in item.Values)
                    {
                        if ((null != item.Property) || (null != item.Binding))
                        {
                            throw new ArgumentException(
                                "A SetterValueBindingHelper with Values may not have its Property or Binding set.");
                        }
                        if (0 != child.Values.Count)
                        {
                            throw new ArgumentException(
                                "Values of a SetterValueBindingHelper may not have Values themselves.");
                        }
                        ApplyBinding(element, child);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the Binding represented by the SetterValueBindingHelper.
        /// </summary>
        /// <param name="element">Element to apply the Binding to.</param>
        /// <param name="item">SetterValueBindingHelper representing the Binding.</param>
        private static void ApplyBinding(FrameworkElement element, SetterValueBindingHelper item)
        {
            if ((null == item.Property) || (null == item.Binding))
            {
                throw new ArgumentException(
                    "SetterValueBindingHelper's Property and Binding must both be set to non-null values.");
            }

            // Get the type on which to set the Binding
            Type type = null;
            if (null == item.Type)
            {
                // No type specified; setting for the specified element
                type = element.GetType();
            }
            else
            {
                // Try to get the type from the type system
                type = System.Type.GetType(item.Type);
                if (null == type)
                {
                    // Search for the type in the list of assemblies
                    foreach (var assembly in AssembliesToSearch)
                    {
                        // Match on short or full name
                        type = assembly.GetTypes()
                            .Where(t => (t.FullName == item.Type) || (t.Name == item.Type))
                            .FirstOrDefault();
                        if (null != type)
                        {
                            // Found; done searching
                            break;
                        }
                    }
                    if (null == type)
                    {
                        // Unable to find the requested type anywhere
                        throw new ArgumentException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                "Unable to access type \"{0}\". Try using an assembly qualified type name.",
                                item.Type));
                    }
                }
            }

            // Get the DependencyProperty for which to set the Binding
            DependencyProperty property = null;
            var field = type.GetRuntimeProperty(item.Property + "Property");
            if (null != field)
            {
                property = field.GetValue(null) as DependencyProperty;
            }
            if (null == property)
            {
                // Unable to find the requsted property
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Unable to access DependencyProperty \"{0}\" on type \"{1}\".",
                        item.Property,
                        type.Name));
            }

            // Set the specified Binding on the specified property
            element.SetBinding(property, item.Binding);
        }

        /// <summary>
        /// Gets a sequence of assemblies to search for the provided type name.
        /// </summary>
        private static IEnumerable<Assembly> AssembliesToSearch
        {
            get
            {
                // Start with the System.Windows assembly (home of all core controls)
                yield return typeof(Control).GetTypeInfo().Assembly;

#if SILVERLIGHT && !WINDOWS_PHONE
                // Fall back by trying each of the assemblies in the Deployment's Parts list
                foreach (var part in Deployment.Current.Parts)
                {
                    var streamResourceInfo = Application.GetResourceStream(
                        new Uri(part.Source, UriKind.Relative));
                    using (var stream = streamResourceInfo.Stream)
                    {
                        yield return part.Load(stream);
                    }
                }
#endif
            }
        }
    }
}
