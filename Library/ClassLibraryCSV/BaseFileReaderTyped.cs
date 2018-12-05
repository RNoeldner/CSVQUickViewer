﻿using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace CsvTools
{
  /// <summary>
  ///   Abstract class as base for all DataReaders that are reading a typed value, e.G. Excel
  /// </summary>
  public abstract class BaseFileReaderTyped : BaseFileReader
  {
    protected object[] m_CurrentValues;

    protected BaseFileReaderTyped(IFileSetting fileSetting) : base(fileSetting)
    {
    }

    /// <summary>
    ///   Gets the boolean.
    /// </summary>
    /// <param name="columnNumber">The i.</param>
    /// <returns></returns>
    public override bool GetBoolean(int columnNumber)
    {
      Debug.Assert(columnNumber >= 0 && columnNumber < FieldCount);
      Debug.Assert(m_CurrentValues != null && columnNumber < m_CurrentValues.Length);
      if (m_CurrentValues[columnNumber] is bool b)
        return b;
      return base.GetBoolean(columnNumber);
    }

    /// <summary>
    ///   Gets the date and time data value of the specified field.
    /// </summary>
    /// <param name="columnNumber">The index of the field to find.</param>
    /// <returns>
    ///   The date and time data value of the specified field.
    /// </returns>
    public override DateTime GetDateTime(int columnNumber)
    {
      Debug.Assert(columnNumber >= 0 && columnNumber < FieldCount);
      Debug.Assert(m_CurrentValues != null && columnNumber < m_CurrentValues.Length);

      object timePart = null;
      string timePartText = null;
      if (AssociatedTimeCol[columnNumber] > -1)
      {
        timePart = m_CurrentValues[AssociatedTimeCol[columnNumber]];
        timePartText = CurrentRowColumnText[AssociatedTimeCol[columnNumber]];
      }

      var dt = GetDateTimeNull(m_CurrentValues[columnNumber], CurrentRowColumnText[columnNumber], timePart, timePartText, GetColumn(columnNumber));
      if (dt.HasValue)
        return dt.Value;
      // Warning was added by GetDecimalNull
      throw WarnAddFormatException(columnNumber,
        $"'{CurrentRowColumnText[columnNumber]}' is not a datetime");
    }

    public override bool IsDBNull(int columnNumber)
    {
      Debug.Assert(columnNumber >= 0 && columnNumber < FieldCount);
      if (m_CurrentValues == null || m_CurrentValues.Length <= columnNumber)
        return true;
      if (Column[columnNumber].DataType == DataType.DateTime)
      {
        if (AssociatedTimeCol[columnNumber] == -1)
          return (m_CurrentValues[columnNumber] == null || m_CurrentValues[columnNumber] == DBNull.Value);

        return (m_CurrentValues[columnNumber] == null || m_CurrentValues[columnNumber] == DBNull.Value) &&
               (m_CurrentValues[AssociatedTimeCol[columnNumber]] == null || m_CurrentValues[AssociatedTimeCol[columnNumber]] == DBNull.Value);
      }

      if (m_CurrentValues[columnNumber] == null || m_CurrentValues[columnNumber] == DBNull.Value)
        return true;

      if (m_CurrentValues[columnNumber] is string str)
        return string.IsNullOrEmpty(str);

      return false;
    }

    /// <summary>
    ///   Gets the decimal.
    /// </summary>
    /// <param name="columnNumber">The i.</param>
    /// <returns></returns>
    public override decimal GetDecimal(int columnNumber)
    {
      Debug.Assert(columnNumber >= 0 && columnNumber < FieldCount);
      Debug.Assert(m_CurrentValues != null && columnNumber < m_CurrentValues.Length);

      if (m_CurrentValues[columnNumber] is decimal || m_CurrentValues[columnNumber] is double || m_CurrentValues[columnNumber] is float ||
          m_CurrentValues[columnNumber] is short || m_CurrentValues[columnNumber] is int || m_CurrentValues[columnNumber] is long)
        return Convert.ToDecimal(m_CurrentValues[columnNumber]);

      return base.GetDecimal(columnNumber);
    }

    /// <summary>
    ///   Gets the double.
    /// </summary>
    /// <param name="columnNumber">The i.</param>
    /// <returns></returns>
    public override double GetDouble(int columnNumber)
    {
      Debug.Assert(columnNumber >= 0 && columnNumber < FieldCount);
      Debug.Assert(m_CurrentValues != null && columnNumber < m_CurrentValues.Length);

      if (m_CurrentValues[columnNumber] is decimal || m_CurrentValues[columnNumber] is double || m_CurrentValues[columnNumber] is float ||
          m_CurrentValues[columnNumber] is short || m_CurrentValues[columnNumber] is int || m_CurrentValues[columnNumber] is long)
        return Convert.ToDouble(m_CurrentValues[columnNumber]);

      return base.GetDouble(columnNumber);
    }

    /// <summary>
    ///   Gets the single-precision floating point number of the specified field.
    /// </summary>
    /// <param name="columnNumber">The index of the field to find.</param>
    /// <returns>
    ///   The single-precision floating point number of the specified field.
    /// </returns>
    public override float GetFloat(int columnNumber)
    {
      Debug.Assert(columnNumber >= 0 && columnNumber < FieldCount);
      Debug.Assert(m_CurrentValues != null && columnNumber < m_CurrentValues.Length);

      if (m_CurrentValues[columnNumber] is decimal || m_CurrentValues[columnNumber] is double || m_CurrentValues[columnNumber] is float ||
          m_CurrentValues[columnNumber] is short || m_CurrentValues[columnNumber] is int || m_CurrentValues[columnNumber] is long)
        return Convert.ToSingle(m_CurrentValues[columnNumber]);

      return base.GetFloat(columnNumber);
    }

    /// <summary>
    ///   Gets the unique identifier.
    /// </summary>
    /// <param name="columnNumber">The i.</param>
    /// <returns></returns>
    public override Guid GetGuid(int columnNumber)
    {
      Debug.Assert(columnNumber >= 0 && columnNumber < FieldCount);
      Debug.Assert(m_CurrentValues != null && columnNumber < m_CurrentValues.Length);

      if (m_CurrentValues[columnNumber] is Guid val)
        return val;

      return base.GetGuid(columnNumber);
    }

    /// <summary>
    ///   Gets the 16-bit signed integer value of the specified field.
    /// </summary>
    /// <param name="columnNumber">The index of the field to find.</param>
    /// <returns>
    ///   The 16-bit signed integer value of the specified field.
    /// </returns>
    public override short GetInt16(int columnNumber)
    {
      Debug.Assert(columnNumber >= 0 && columnNumber < FieldCount);
      Debug.Assert(m_CurrentValues != null && columnNumber < m_CurrentValues.Length);

      if (m_CurrentValues[columnNumber] is decimal || m_CurrentValues[columnNumber] is double || m_CurrentValues[columnNumber] is float ||
          m_CurrentValues[columnNumber] is short || m_CurrentValues[columnNumber] is int || m_CurrentValues[columnNumber] is long)
        return Convert.ToInt16(m_CurrentValues[columnNumber]);

      return base.GetInt16(columnNumber);
    }

    /// <summary>
    ///   Gets the int32.
    /// </summary>
    /// <param name="columnNumber">The i.</param>
    /// <returns></returns>
    public override int GetInt32(int columnNumber)
    {
      Debug.Assert(columnNumber >= 0 && columnNumber < FieldCount);
      Debug.Assert(m_CurrentValues != null && columnNumber < m_CurrentValues.Length);

      if (m_CurrentValues[columnNumber] is decimal || m_CurrentValues[columnNumber] is double || m_CurrentValues[columnNumber] is float ||
          m_CurrentValues[columnNumber] is short || m_CurrentValues[columnNumber] is int || m_CurrentValues[columnNumber] is long)
        return Convert.ToInt32(m_CurrentValues[columnNumber]);

      return base.GetInt32(columnNumber);
    }

    /// <summary>
    ///   Gets the int64.
    /// </summary>
    /// <param name="columnNumber">The i.</param>
    /// <returns></returns>
    public override long GetInt64(int columnNumber)
    {
      Debug.Assert(columnNumber >= 0 && columnNumber < FieldCount);
      Debug.Assert(m_CurrentValues != null && columnNumber < m_CurrentValues.Length);

      if (m_CurrentValues[columnNumber] is decimal || m_CurrentValues[columnNumber] is double || m_CurrentValues[columnNumber] is float ||
          m_CurrentValues[columnNumber] is short || m_CurrentValues[columnNumber] is int || m_CurrentValues[columnNumber] is long)
        return Convert.ToInt64(m_CurrentValues[columnNumber]);

      return base.GetInt64(columnNumber);
    }

    public override string GetString(int columnNumber)
    {
      Debug.Assert(columnNumber >= 0 && columnNumber < FieldCount);
      Debug.Assert(m_CurrentValues != null && columnNumber < m_CurrentValues.Length);
      if (m_CurrentValues[columnNumber] is string val)
        return val;

      return base.GetString(columnNumber);
    }

    public override int GetValues(object[] values)
    {
      Contract.Assume(m_CurrentValues != null);
      Array.Copy(m_CurrentValues, values, FieldCount);
      return FieldCount;
    }
  }
}