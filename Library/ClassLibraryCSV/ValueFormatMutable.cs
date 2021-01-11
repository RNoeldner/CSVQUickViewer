/*
 * Copyright (C) 2014 Raphael Nöldner : http://csvquickviewer.com
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser Public
 * License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser Public License for more details.
 *
 * You should have received a copy of the GNU Lesser Public License along with this program.
 * If not, see http://www.gnu.org/licenses/ .
 *
 */

using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CsvTools
{
	/// <summary>
	///   Setting for a value format
	/// </summary>
	[Serializable]
	public sealed class ValueFormatMutable : IValueFormat, INotifyPropertyChanged
	{
		private ImmutableValueFormat m_ImmutableValueFormat;

		public ValueFormatMutable() => m_ImmutableValueFormat = new ImmutableValueFormat();

		public ValueFormatMutable([NotNull] IValueFormat other) => CopyFrom(other);

		public bool Specified => m_ImmutableValueFormat.Specified;

		/// <summary>
		///   Gets or sets the date format.
		/// </summary>
		/// <value>The date format.</value>
		[XmlElement]
		[DefaultValue(ValueFormatExtension.cDateFormatDefault)]
		public string DateFormat
		{
			[NotNull]
			get => m_ImmutableValueFormat.DateFormat;
			[CanBeNull]
			set
			{
				var newVal = value ?? string.Empty;
				if (m_ImmutableValueFormat.DateFormat.Equals(newVal, StringComparison.Ordinal))
					return;
				m_ImmutableValueFormat = new ImmutableValueFormat(m_ImmutableValueFormat.DataType, newVal,
					m_ImmutableValueFormat.DateSeparator, m_ImmutableValueFormat.TimeSeparator, m_ImmutableValueFormat.NumberFormat,
					m_ImmutableValueFormat.GroupSeparator, m_ImmutableValueFormat.DecimalSeparator, m_ImmutableValueFormat.True,
					m_ImmutableValueFormat.False, m_ImmutableValueFormat.DisplayNullAs);
				NotifyPropertyChanged(nameof(DateFormat));
			}
		}

		/// <summary>
		///   Gets or sets the date separator.
		/// </summary>
		/// <value>The date separator.</value>
		[XmlElement]
		[DefaultValue(ValueFormatExtension.cDateSeparatorDefault)]
		public string DateSeparator
		{
			[NotNull]
			get => m_ImmutableValueFormat.DateSeparator;
			[CanBeNull]
			set
			{
				// Translate written punctuation into a character
				var newVal = value?? string.Empty;
				if (m_ImmutableValueFormat.DateSeparator.Equals(newVal, StringComparison.Ordinal))
					return;
				m_ImmutableValueFormat = new ImmutableValueFormat(m_ImmutableValueFormat.DataType, m_ImmutableValueFormat.DateFormat,
					newVal, m_ImmutableValueFormat.TimeSeparator, m_ImmutableValueFormat.NumberFormat,
					m_ImmutableValueFormat.GroupSeparator, newVal, m_ImmutableValueFormat.True,
					m_ImmutableValueFormat.False, m_ImmutableValueFormat.DisplayNullAs);
				NotifyPropertyChanged(nameof(DateSeparator));
			}
		}

		/// <summary>
		///   Gets or sets the decimal separator.
		/// </summary>
		/// <value>The decimal separator.</value>
		[XmlElement]
		public string DecimalSeparator
		{
			[NotNull]
			get => m_ImmutableValueFormat.DecimalSeparator;
			set
			{
				// Translate written punctuation into a character
				var newValDecimal = value?? string.Empty;
				if (m_ImmutableValueFormat.DecimalSeparator.Equals(newValDecimal))
					return;

				var newValGroup = m_ImmutableValueFormat.GroupSeparator;
				if (newValGroup.Equals(newValDecimal))
				{
					newValGroup = "";
					NotifyPropertyChanged(nameof(GroupSeparator));
				}

				m_ImmutableValueFormat = new ImmutableValueFormat(m_ImmutableValueFormat.DataType, m_ImmutableValueFormat.DateFormat,
					m_ImmutableValueFormat.DateSeparator, m_ImmutableValueFormat.TimeSeparator, m_ImmutableValueFormat.NumberFormat,
					newValGroup, newValDecimal, m_ImmutableValueFormat.True,
					m_ImmutableValueFormat.False, m_ImmutableValueFormat.DisplayNullAs);
				NotifyPropertyChanged(nameof(DecimalSeparator));
			}
		}

		[XmlIgnore] [UsedImplicitly] public bool DecimalSeparatorSpecified => m_ImmutableValueFormat.DecimalSeparator != ValueFormatExtension.cDecimalSeparatorDefault;

		/// <summary>
		///   Gets or sets the representation for false.
		/// </summary>
		/// <value>The false.</value>
		[XmlElement]
		[DefaultValue(ValueFormatExtension.cFalseDefault)]
		public string False
		{
			[NotNull]
			get => m_ImmutableValueFormat.False;
			[CanBeNull]
			set
			{
				var newVal = value ?? string.Empty;
				if (m_ImmutableValueFormat.False.Equals(newVal, StringComparison.OrdinalIgnoreCase))
					return;
				m_ImmutableValueFormat = new ImmutableValueFormat(m_ImmutableValueFormat.DataType, m_ImmutableValueFormat.DateFormat,
					m_ImmutableValueFormat.DateSeparator, m_ImmutableValueFormat.TimeSeparator, m_ImmutableValueFormat.NumberFormat,
					m_ImmutableValueFormat.GroupSeparator, m_ImmutableValueFormat.DecimalSeparator, m_ImmutableValueFormat.True,
					newVal, m_ImmutableValueFormat.DisplayNullAs);
				NotifyPropertyChanged(nameof(False));
			}
		}

		/// <summary>
		///   Gets or sets the group separator.
		/// </summary>
		/// <value>The group separator.</value>
		[XmlElement]
		public string GroupSeparator
		{
			[NotNull]
			get => m_ImmutableValueFormat.GroupSeparator;
			set
			{
				var newValGroup = value ?? string.Empty;
				if (m_ImmutableValueFormat.GroupSeparator.Equals(newValGroup))
					return;
				// If we set the GroupSeparator to be the decimal separator, do not save
				var newValDecimal = m_ImmutableValueFormat.DecimalSeparator;
				if (newValGroup.Equals(newValDecimal))
				{
					newValDecimal=m_ImmutableValueFormat.GroupSeparator;
					NotifyPropertyChanged(nameof(DecimalSeparator));
				}

				m_ImmutableValueFormat = new ImmutableValueFormat(m_ImmutableValueFormat.DataType, m_ImmutableValueFormat.DateFormat,
					m_ImmutableValueFormat.DateSeparator, m_ImmutableValueFormat.TimeSeparator, m_ImmutableValueFormat.NumberFormat,
					newValGroup, newValDecimal, m_ImmutableValueFormat.True,
					m_ImmutableValueFormat.False, m_ImmutableValueFormat.DisplayNullAs);

				NotifyPropertyChanged(nameof(GroupSeparator));
			}
		}

		[XmlIgnore] [UsedImplicitly] public bool GroupSeparatorSpecified => m_ImmutableValueFormat.GroupSeparator != ValueFormatExtension.cGroupSeparatorDefault;

		/// <summary>
		///   Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///   Gets or sets the type of the data.
		/// </summary>
		/// <value>The type of the data.</value>
		[XmlAttribute]
		[DefaultValue(DataType.String)]
		public DataType DataType
		{
			get => m_ImmutableValueFormat.DataType;
			set
			{
				if (m_ImmutableValueFormat.DataType.Equals(value))
					return;
				m_ImmutableValueFormat = new ImmutableValueFormat(value, m_ImmutableValueFormat.DateFormat,
					m_ImmutableValueFormat.DateSeparator, m_ImmutableValueFormat.TimeSeparator, m_ImmutableValueFormat.NumberFormat,
					m_ImmutableValueFormat.GroupSeparator, m_ImmutableValueFormat.DecimalSeparator, m_ImmutableValueFormat.True,
					m_ImmutableValueFormat.False, m_ImmutableValueFormat.DisplayNullAs);
				NotifyPropertyChanged(nameof(DataType));
			}
		}

		/// <summary>
		///   Writing data you can specify how a NULL value should be written, commonly its empty, in
		///   some circumstances you might want to have n/a etc.
		/// </summary>
		/// <value>Text used if the value is NULL</value>
		[XmlAttribute]
		[DefaultValue("")]
		public string DisplayNullAs
		{
			[NotNull]
			get => m_ImmutableValueFormat.DisplayNullAs;

			set
			{
				var newVal = value ?? string.Empty;
				if (m_ImmutableValueFormat.DisplayNullAs.Equals(newVal))
					return;
				m_ImmutableValueFormat = new ImmutableValueFormat(m_ImmutableValueFormat.DataType, m_ImmutableValueFormat.DateFormat,
					m_ImmutableValueFormat.DateSeparator, m_ImmutableValueFormat.TimeSeparator, m_ImmutableValueFormat.NumberFormat,
					m_ImmutableValueFormat.GroupSeparator, m_ImmutableValueFormat.DecimalSeparator, m_ImmutableValueFormat.True,
					m_ImmutableValueFormat.False, newVal);
				NotifyPropertyChanged(nameof(DisplayNullAs));
			}
		}

		/// <summary>
		///   Gets or sets the number format.
		/// </summary>
		/// <value>The number format.</value>
		[XmlElement]
		[DefaultValue(ValueFormatExtension.cNumberFormatDefault)]
		public string NumberFormat
		{
			[NotNull]
			get => m_ImmutableValueFormat.NumberFormat;
			[CanBeNull]
			set
			{
				var newVal = value ?? string.Empty;
				if (m_ImmutableValueFormat.NumberFormat.Equals(newVal, StringComparison.Ordinal))
					return;
				m_ImmutableValueFormat = new ImmutableValueFormat(m_ImmutableValueFormat.DataType, m_ImmutableValueFormat.DateFormat,
					m_ImmutableValueFormat.DateSeparator, m_ImmutableValueFormat.TimeSeparator, newVal,
					m_ImmutableValueFormat.GroupSeparator, m_ImmutableValueFormat.DecimalSeparator, m_ImmutableValueFormat.True,
					m_ImmutableValueFormat.False, m_ImmutableValueFormat.DisplayNullAs);
				NotifyPropertyChanged(nameof(NumberFormat));
			}
		}

		/// <summary>
		///   Gets or sets the time separator.
		/// </summary>
		/// <value>The time separator.</value>
		[XmlElement]
		[DefaultValue(ValueFormatExtension.cTimeSeparatorDefault)]
		public string TimeSeparator
		{
			[NotNull]
			get => m_ImmutableValueFormat.TimeSeparator;
			[NotNull]
			set
			{
				var newVal = (value??string.Empty).WrittenPunctuation();
				if (m_ImmutableValueFormat.TimeSeparator.Equals(newVal, StringComparison.Ordinal))
					return;
				m_ImmutableValueFormat = new ImmutableValueFormat(m_ImmutableValueFormat.DataType, m_ImmutableValueFormat.DateFormat,
					m_ImmutableValueFormat.DateSeparator, newVal, m_ImmutableValueFormat.NumberFormat,
					m_ImmutableValueFormat.GroupSeparator, m_ImmutableValueFormat.DecimalSeparator, m_ImmutableValueFormat.True,
					m_ImmutableValueFormat.False, m_ImmutableValueFormat.DisplayNullAs);
				NotifyPropertyChanged(nameof(TimeSeparator));
			}
		}

		/// <summary>
		///   Gets or sets the representation for true.
		/// </summary>
		/// <value>The true.</value>
		[XmlElement]
		[DefaultValue(ValueFormatExtension.cTrueDefault)]
		public string True
		{
			[NotNull]
			get => m_ImmutableValueFormat.True;
			[CanBeNull]
			set
			{
				var newVal = value ?? string.Empty;
				if (m_ImmutableValueFormat.True.Equals(newVal, StringComparison.OrdinalIgnoreCase))
					return;
				m_ImmutableValueFormat = new ImmutableValueFormat(m_ImmutableValueFormat.DataType, m_ImmutableValueFormat.DateFormat,
					m_ImmutableValueFormat.DateSeparator, m_ImmutableValueFormat.TimeSeparator, m_ImmutableValueFormat.NumberFormat,
					m_ImmutableValueFormat.GroupSeparator, m_ImmutableValueFormat.DecimalSeparator, newVal,
					m_ImmutableValueFormat.False, m_ImmutableValueFormat.DisplayNullAs);
				NotifyPropertyChanged(nameof(True));
			}
		}

		public bool Equals(IValueFormat other)
		{
			if (other is null)
				return false;
			if (ReferenceEquals(this, other))
				return true;

			if (other.DataType != DataType || !other.DisplayNullAs.Equals(DisplayNullAs, StringComparison.Ordinal))
				return false;

			switch (DataType)
			{
				case DataType.Integer:
					return string.Equals(other.NumberFormat, NumberFormat, StringComparison.Ordinal);

				case DataType.Numeric:
				case DataType.Double:
					return other.GroupSeparator == GroupSeparator &&
								 other.DecimalSeparator == DecimalSeparator &&
								 string.Equals(other.NumberFormat, NumberFormat, StringComparison.Ordinal);

				case DataType.DateTime:
					return string.Equals(other.DateFormat, DateFormat, StringComparison.Ordinal) &&
								 string.Equals(other.DateSeparator, DateSeparator, StringComparison.Ordinal) &&
								 string.Equals(other.TimeSeparator, TimeSeparator, StringComparison.Ordinal);

				case DataType.Boolean:
					return string.Equals(other.False, False, StringComparison.OrdinalIgnoreCase) &&
								 string.Equals(other.True, True, StringComparison.OrdinalIgnoreCase);

				default:
					// compare everything
					return string.Equals(other.DateFormat, DateFormat, StringComparison.Ordinal) &&
								 string.Equals(other.DateSeparator, DateSeparator, StringComparison.Ordinal) &&
								 string.Equals(other.TimeSeparator, TimeSeparator, StringComparison.Ordinal) &&
								 string.Equals(other.False, False, StringComparison.OrdinalIgnoreCase) &&
								 string.Equals(other.True, True, StringComparison.OrdinalIgnoreCase) &&
								 other.GroupSeparator == GroupSeparator &&
								 other.DecimalSeparator == DecimalSeparator &&
								 string.Equals(other.NumberFormat, NumberFormat, StringComparison.Ordinal);
			}
		}

		public void CopyFrom(IValueFormat other) =>
		 m_ImmutableValueFormat = new ImmutableValueFormat(other.DataType, other.DateFormat, other.DateSeparator, other.TimeSeparator, other.NumberFormat, other.GroupSeparator, other.DecimalSeparator, other.True, other.False, other.DisplayNullAs);

		/// <summary>
		///   Notifies the property changed.
		/// </summary>
		/// <param name="info">The info.</param>
		public void NotifyPropertyChanged(string info) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
	}
}