﻿/*
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

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CsvTools
{
  [Serializable]
  public class SampleRecordEntry : IEquatable<SampleRecordEntry>, ICloneable<SampleRecordEntry>
  {
    public SampleRecordEntry() : this(0, true, string.Empty)
    {
    }

    public SampleRecordEntry(long recordNumber, string error) : this(recordNumber, true, error)
    {
    }

    public SampleRecordEntry(long recordNumber, bool provideEvidence) : this(recordNumber, provideEvidence,
      string.Empty)
    {
    }

    public SampleRecordEntry(long recordNumber, bool provideEvidence, string error)
    {
      RecordNumber = recordNumber;
      ProvideEvidence = provideEvidence;
      Error = error;
    }

    public SampleRecordEntry(long recordNumber) : this(recordNumber, true, string.Empty)
    {
    }

    /// <summary>
    ///   Gets or sets the error.
    /// </summary>
    /// <value>
    ///   The error.
    /// </value>
    [XmlAttribute]
    [DefaultValue("")]
    public string Error { get; }

    /// <summary>
    ///   Gets or sets a value indicating whether [provide evidence].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [provide evidence]; otherwise, <c>false</c>.
    /// </value>
    [XmlAttribute]
    [DefaultValue(true)]
    public bool ProvideEvidence { get; }

    /// <summary>
    ///   Gets or sets the record number.
    /// </summary>
    /// <value>
    ///   The record number.
    /// </value>
    [XmlAttribute]
    public long RecordNumber { get; }


    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise,
    ///   <see langword="false" />.
    /// </returns>
    public bool Equals(SampleRecordEntry other)
    {
      if (other is null)
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return RecordNumber == other.RecordNumber && ProvideEvidence == other.ProvideEvidence &&
             string.Equals(Error, other.Error, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///   Clones this instance into a new instance of the same type
    /// </summary>
    /// <returns></returns>
    public SampleRecordEntry Clone() => new SampleRecordEntry(RecordNumber, ProvideEvidence, Error);
    public SampleRecordEntry CopyTo() => new SampleRecordEntry(RecordNumber, ProvideEvidence, Error);

    /// <summary>
    ///   Compares the current instance with another object of the same type and returns an integer that indicates whether the
    ///   current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    ///   A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
    ///   Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in
    ///   the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows
    ///   <paramref name="obj" /> in the sort order.
    /// </returns>
    public int CompareTo(object obj)
    {
      if (obj is SampleRecordEntry entry)
        return CompareTo(entry);
      return -1;
    }

    /// <summary>
    ///   Compares the current instance with another object of the same type and returns an integer that indicates whether the
    ///   current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="other">An object to compare with this instance.</param>
    /// <returns>
    ///   A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
    ///   Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance occurs
    ///   in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows
    ///   <paramref name="other" /> in the sort order.
    /// </returns>
    public int CompareTo(SampleRecordEntry other) => RecordNumber.CompareTo(other.RecordNumber);

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj">The object to compare with the current object. </param>
    /// <returns>
    ///   <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.
    /// </returns>
    public override bool Equals(object obj) => Equals(obj as SampleRecordEntry);

    /// <summary>Serves as the default hash function. </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = RecordNumber.GetHashCode();
        hashCode = (hashCode * 397) ^ ProvideEvidence.GetHashCode();
        hashCode = (hashCode * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(Error);
        return hashCode;
      }
    }
  }
}