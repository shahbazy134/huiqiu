/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.PowerCommands.Extensions;

namespace Microsoft.PowerCommands.OptionPages
{
    /// <summary>
    /// TypeConverter for Disabled Command collection
    /// </summary>
    public class DisabledCommandsDictionaryConverter : StringConverter
    {
        #region Fields
        IList<CommandID> disabledCommands; 
        #endregion

        #region Public Implementation
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given source type to a string using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you wish to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            disabledCommands = value as IList<CommandID>;

            StringBuilder builder = new StringBuilder();

            disabledCommands.ForEach(
                cmdId =>
                {
                    builder.Append(
                        string.Format("{0},{1};", cmdId.Guid, cmdId.ID));
                });

            return builder.ToString();
        }

        /// <summary>
        /// Converts the specified value object to a <see cref="T:System.String"/> object.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion could not be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Guid cmdGuid;
            int cmdId;
            disabledCommands = new List<CommandID>();

            try
            {
                if(!string.IsNullOrEmpty(value.ToString()))
                {
                    value.ToString().Split(';').ForEach(
                        item =>
                        {
                            string[] subItems = item.Split(',');

                            if(subItems.Count() == 2)
                            {
                                cmdGuid = new Guid(subItems[0]);
                                int.TryParse(subItems[1], out cmdId);

                                disabledCommands.Add(new CommandID(cmdGuid, cmdId));
                            }
                        });
                }
            }
            catch
            {
            }

            return disabledCommands;
        } 
        #endregion
    }
}
