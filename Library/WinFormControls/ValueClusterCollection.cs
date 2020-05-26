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

namespace CsvTools
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Diagnostics.Contracts;
  using System.Globalization;
  using System.Linq;

  /// <summary>
  ///   ValueClusterCollection
  /// </summary>
  public class ValueClusterCollection
  {
    private const string c_IsNull = "[{0}] IS NULL";

    private const long c_TicksPerGroup = TimeSpan.TicksPerMinute * 30;

    private readonly List<ValueCluster> m_ValueClusters = new List<ValueCluster>();

    private BuildValueClustersResult m_Result = BuildValueClustersResult.NotRun;
    private ValueClustersGroupType m_Type = ValueClustersGroupType.Text;

    private enum ValueClustersGroupType
    {
      Text,
      NumericFraction,
      NumericOnes,
      NumericTens,
      NumericHundreds,
      NumericThousands,
      DateHours,
      DateDay,
      DateMonth,
      DateYear
    }

    /// <summary>
    ///   Gets the values.
    /// </summary>
    /// <value>The values.</value>
    public ICollection<ValueCluster> ValueClusters
    {
      get
      {
        Contract.Ensures(Contract.Result<IEnumerable<ValueCluster>>() != null);
        return m_ValueClusters;
      }
    }

    /// <summary>
    ///   Builds the data grid view column filter values.
    /// </summary>
    /// <param name="dataView">The data view.</param>
    /// <param name="columnType">Type of the column.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="maxNumber">The maximum number.</param>
    /// <returns></returns>
    public BuildValueClustersResult BuildValueClusters(
    DataView dataView,
    Type columnType,
    int columnIndex,
    int maxNumber)
    {
      Contract.Requires(dataView != null);

      if (m_Result == BuildValueClustersResult.NotRun)
      {
        try
        {
          if (columnType == typeof(string) || columnType == typeof(bool))
            m_Result = BuildValueClusterString(dataView.Table, columnIndex, maxNumber);
          else if (columnType == typeof(DateTime))
            m_Result = BuildValueClustersDate(dataView.Table, columnIndex, maxNumber);
          else if (columnType == typeof(byte) || columnType == typeof(short) || columnType == typeof(int)
                   || columnType == typeof(uint) || columnType == typeof(int) || columnType == typeof(float)
                   || columnType == typeof(double) || columnType == typeof(long) || columnType == typeof(ulong)
                   || columnType == typeof(decimal))
          {
            m_Result = BuildValueClustersNumeric(dataView.Table, columnIndex, maxNumber, columnType);
          }
          else
            m_Result = BuildValueClustersResult.WrongType;
        }
        catch
        {
          m_Result = BuildValueClustersResult.WrongType;
        }
      }

      if (m_Result != BuildValueClustersResult.ListFilled)
        return m_Result;
      foreach (var item in m_ValueClusters)
        item.Count = 0;

      if (m_ValueClusters.Count == 1)
      {
        m_ValueClusters[0].Count = dataView.Count;
        return m_Result;
      }

      if (dataView.Count > 10000)
        return m_Result;

      if (columnType == typeof(DateTime))
        UpdateValueClustersDate(dataView, columnIndex);
      else if (columnType == typeof(string) || columnType == typeof(bool))
        UpdateValueClustersString(dataView, columnIndex);
      else
        UpdateValueClustersNumeric(dataView, columnIndex);

      return m_Result;
    }

    /// <summary>
    ///   Gets the active value cluster.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ValueCluster> GetActiveValueCluster()
    {
      return this.m_ValueClusters.Where(value => !string.IsNullOrEmpty(value.Display) && value.Active);
    }

    private static string GetDaySort(DateTime date) => date.ToString("s", CultureInfo.CurrentCulture);

    private static long GetHourKey(DateTime date) => date.TimeOfDay.Ticks / c_TicksPerGroup;

    private static string GetHourSort(long ticks) => ticks.ToString("000000", CultureInfo.InvariantCulture);

    private static DateTime GetMonthKey(DateTime date) => new DateTime(date.Year, date.Month, 1);

    private static string GetMonthSort(DateTime date) => date.ToString("s", CultureInfo.InvariantCulture);

    private static int GetYearKey(DateTime date) => date.Year;

    private static string GetYearSort(int year) => year.ToString("000000", CultureInfo.InvariantCulture);

    /// <summary>
    ///   Builds the value clusters date.
    /// </summary>
    /// <param name="dataTable">The data view.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="maxNumber">The maximum number.</param>
    /// <returns></returns>
    private BuildValueClustersResult BuildValueClustersDate(DataTable dataTable, int columnIndex, int maxNumber)
    {
      // Get the distinct values and their counts
      var clusterYear = new HashSet<int>();
      var clusterMonth = new HashSet<DateTime>();
      var clusterDay = new HashSet<DateTime>();
      var clusterHour = new HashSet<long>();

      var columnName = dataTable.Columns[columnIndex].ColumnName;
      foreach (DataRow dataRow in dataTable.Rows)
      {
        int keyYear;
        DateTime keyMonth;
        DateTime keyDate;
        long keyHour;
        if (dataRow[columnIndex] == DBNull.Value)
        {
          keyYear = 0;
          keyHour = 0;
          keyMonth = DateTime.MinValue;
          keyDate = DateTime.MinValue;
        }
        else
        {
          var value = (DateTime)dataRow[columnIndex];
          keyHour = GetHourKey(value);
          keyDate = value.Date;
          keyYear = GetYearKey(value);
          keyMonth = GetMonthKey(value);
        }

        clusterHour.Add(keyHour);
        clusterDay.Add(keyDate);
        clusterMonth.Add(keyMonth);
        clusterYear.Add(keyYear);

        // if we have more than the maximum entries stop, no value filter will be used
        if (clusterYear.Count <= maxNumber)
          continue;
        m_ValueClusters.Clear();
        return BuildValueClustersResult.TooManyValues;
      }

      if (clusterYear.Count == 0)
      {
        m_ValueClusters.Clear();
        return BuildValueClustersResult.NoValues;
      }

      if (clusterDay.Count == 1)
      {
        m_Type = ValueClustersGroupType.DateHours;
        foreach (var dic in clusterHour.OrderBy(x => x))
        {
          if (dic != 0)
          {
            var from = StringConversion.GetTimeFromTicks(dic * c_TicksPerGroup);
            var to = StringConversion.GetTimeFromTicks((dic + 1) * c_TicksPerGroup);
            m_ValueClusters.Add(new ValueCluster($"{@from:t} - {to:t}", string.Format(
              CultureInfo.InvariantCulture, @"([{0}] >= #{1:MM\/dd\/yyyy HH:mm}# AND {0} < #{2:MM\/dd\/yyyy HH:mm}#)",
              columnName.SqlName(), @from, to), GetHourSort(dic)));
          }
          else
          {
            m_ValueClusters.Add(new ValueCluster(ColumnFilterLogic.OperatorIsNull,
              string.Format(CultureInfo.InvariantCulture, "([{0}] IS NULL)", columnName.SqlName()), string.Empty));
          }
        }
      }
      else if (clusterDay.Count < maxNumber)
      {
        m_Type = ValueClustersGroupType.DateDay;
        foreach (var dic in clusterDay.OrderBy(x => x))
        {
          if (dic != DateTime.MinValue)
          {
            m_ValueClusters.Add(
              new ValueCluster(dic.ToString("d", CultureInfo.CurrentCulture), string.Format(
                CultureInfo.InvariantCulture,
                @"([{0}] >= #{1:MM\/dd\/yyyy}# AND {0} < #{2:MM\/dd\/yyyy}#)",
                columnName.SqlName(),
                dic,
                dic.AddDays(1)), GetDaySort(dic)));
          }
          else
          {
            m_ValueClusters.Add(new ValueCluster(ColumnFilterLogic.OperatorIsNull, string.Format(CultureInfo.InvariantCulture, c_IsNull, columnName.SqlName()), string.Empty));
          }
        }
      }
      else if (clusterMonth.Count < maxNumber)
      {
        m_Type = ValueClustersGroupType.DateMonth;
        foreach (var dic in clusterMonth.OrderBy(x => x))
        {
          if (dic != DateTime.MinValue)
          {
            m_ValueClusters.Add(
              new ValueCluster(
                dic.ToString("Y", CultureInfo.CurrentCulture), // Year month pattern
               string.Format(CultureInfo.InvariantCulture,
                    @"([{0}] >= #{1:MM\/dd\/yyyy}# AND {0} < #{2:MM\/dd\/yyyy}#)",
                    columnName.SqlName(),
                    dic, dic.AddMonths(1)),
               GetMonthSort(dic)));
          }
          else
          {
            m_ValueClusters.Add(
              new ValueCluster(
              ColumnFilterLogic.OperatorIsNull,
              string.Format(CultureInfo.InvariantCulture, c_IsNull, columnName.SqlName()),
              string.Empty));
          }
        }
      }
      else
      {
        m_Type = ValueClustersGroupType.DateYear;
        foreach (var dic in clusterYear.OrderBy(x => x))
        {
          if (dic != 0)
          {
            m_ValueClusters.Add(
              new ValueCluster(dic.ToString("D", CultureInfo.CurrentCulture), // Decimal
                string.Format(
                    CultureInfo.InvariantCulture,
                    "([{0}] >= #01/01/{1:d4}# AND {0} < #01/01/{2:d4}#)",
                    columnName.SqlName(),
                    dic,
                    dic + 1),
                GetYearSort(dic)));
          }
          else
          {
            m_ValueClusters.Add(
              new ValueCluster(
              ColumnFilterLogic.OperatorIsNull,
              string.Format(CultureInfo.InvariantCulture, c_IsNull, columnName.SqlName()),
              string.Empty));
          }
        }
      }

      return BuildValueClustersResult.ListFilled;
    }

    /// <summary>
    ///   Builds the value clusters date.
    /// </summary>
    /// <param name="dataTable">The data view.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="maxNumber">The maximum number.</param>
    /// <param name="columnType">Type of the column.</param>
    /// <returns></returns>
    private BuildValueClustersResult BuildValueClustersNumeric(
      DataTable dataTable,
      int columnIndex,
      int maxNumber,
      Type columnType)
    {
      // Get the distinct values and their counts
      var clusterFractions = new HashSet<double>();
      var clusterOne = new HashSet<long>();
      var clusterTen = new HashSet<long>();
      var clusterHundred = new HashSet<long>();
      var clusterThousand = new HashSet<long>();

      var columnName = dataTable.Columns[columnIndex].ColumnName;
      foreach (DataRow dataRow in dataTable.Rows)
      {
        if (dataRow[columnIndex] == DBNull.Value)
        {
          clusterFractions.Add(int.MinValue);
          clusterOne.Add(int.MinValue);
          clusterTen.Add(int.MinValue);
          clusterHundred.Add(int.MinValue);
        }
        else
        {
          if (columnType == typeof(decimal) || columnType == typeof(float) || columnType == typeof(double))
          {
            var rounded = Math.Floor(Convert.ToDouble(dataRow[columnIndex], CultureInfo.CurrentCulture) * 10d) / 10d;
            clusterFractions.Add(rounded);
          }

          var key = Convert.ToInt64(dataRow[columnIndex], CultureInfo.CurrentCulture);
          clusterOne.Add(key);
          clusterTen.Add(key / 10);
          clusterHundred.Add(key / 100);
          clusterThousand.Add(key / 1000);
        }

        // if we have more than the maximum entries stop, no value filter will be used
        if (clusterThousand.Count <= maxNumber)
          continue;
        m_ValueClusters.Clear();
        return BuildValueClustersResult.TooManyValues;
      }

      if (clusterOne.Count == 0 && clusterFractions.Count == 0)
      {
        m_ValueClusters.Clear();
        return BuildValueClustersResult.NoValues;
      }

      var colNameEsc = $"[{columnName.SqlName()}]";
      if (clusterFractions.Count < maxNumber && clusterFractions.Count > 0)
      {
        m_Type = ValueClustersGroupType.NumericFraction;
        foreach (var dic in clusterFractions.OrderBy(x => x))
        {
          if (Math.Abs(dic - int.MinValue) > .1)
          {
            m_ValueClusters.Add(
              new ValueCluster(
                dic.ToString(CultureInfo.CurrentCulture), // Decimal
                string.Format(
                      CultureInfo.InvariantCulture,
                      "({0} >= {1} AND {0} < {2})",
                      colNameEsc,
                      dic,
                      dic + .1),
                (dic * 10d).ToString("0000000000000000000", CultureInfo.InvariantCulture)
              ));
          }
          else
          {
            m_ValueClusters.Add(
              new ValueCluster(ColumnFilterLogic.OperatorIsNull,
                string.Format(CultureInfo.InvariantCulture, "({0} IS NULL)", colNameEsc),
                 string.Empty));
          }
        }
      }
      else if (clusterOne.Count < maxNumber)
      {
        m_Type = ValueClustersGroupType.NumericOnes;
        foreach (var dic in clusterOne.OrderBy(x => x))
        {
          if (dic != int.MinValue)
          {
            m_ValueClusters.Add(
              new ValueCluster(dic.ToString("D", CultureInfo.CurrentCulture), // Decimal
                string.Format(CultureInfo.InvariantCulture, "({0} >= {1} AND {0} < {2})", colNameEsc, dic, dic + 1),
                dic.ToString("0000000000000000000", CultureInfo.InvariantCulture)
              ));
          }
          else
          {
            m_ValueClusters.Add(
              new ValueCluster(ColumnFilterLogic.OperatorIsNull,
                string.Format(CultureInfo.InvariantCulture, "({0} IS NULL)", colNameEsc),
                string.Empty));
          }
        }
      }
      else if (clusterTen.Count < maxNumber)
      {
        m_Type = ValueClustersGroupType.NumericTens;
        foreach (var dic in clusterTen.OrderBy(x => x))
        {
          if (dic != int.MinValue)
          {
            m_ValueClusters.Add(
              new ValueCluster($"{dic * 10} - {dic * 10 + 9}", // Decimal
                string.Format(
                      CultureInfo.InvariantCulture,
                      "({0} >= {1} AND {0} < {2})",
                      colNameEsc,
                      dic * 10,
                      (dic + 1) * 10),
                dic.ToString("0000000000000000000", CultureInfo.InvariantCulture)));
          }
          else
          {
            m_ValueClusters.Add(
              new ValueCluster(ColumnFilterLogic.OperatorIsNull,
                string.Format(CultureInfo.InvariantCulture, "({0} IS NULL)", colNameEsc),
                string.Empty));
          }
        }
      }
      else if (clusterHundred.Count < maxNumber)
      {
        m_Type = ValueClustersGroupType.NumericHundreds;
        foreach (var dic in clusterHundred.OrderBy(x => x))
        {
          if (dic != int.MinValue)
          {
            m_ValueClusters.Add(
              new ValueCluster($"{dic * 100} - {dic * 100 + 99}", // Decimal
              string.Format(
                      CultureInfo.InvariantCulture,
                      "({0} >= {1} AND {0} < {2})",
                      colNameEsc,
                      dic * 100,
                      (dic + 1) * 100),
              dic.ToString("0000000000000000000", CultureInfo.InvariantCulture)));
          }
          else
          {
            m_ValueClusters.Add(
              new ValueCluster(ColumnFilterLogic.OperatorIsNull,
                string.Format(CultureInfo.InvariantCulture, "({0} IS NULL)", colNameEsc),
                string.Empty));
          }
        }
      }
      else
      {
        m_Type = ValueClustersGroupType.NumericThousands;
        foreach (var dic in clusterThousand.OrderBy(x => x))
        {
          if (dic != int.MinValue)
          {
            m_ValueClusters.Add(
              new ValueCluster($"{dic * 1000} - {dic * 1000 + 999}", // Decimal
                string.Format(
                      CultureInfo.InvariantCulture,
                      "({0} >= {1} AND {0} < {2})",
                      colNameEsc,
                      dic * 1000,
                      (dic + 1) * 1000),
                dic.ToString("0000000000000000000", CultureInfo.InvariantCulture)));
          }
          else
          {
            m_ValueClusters.Add(
              new ValueCluster(ColumnFilterLogic.OperatorIsNull,
                string.Format(CultureInfo.InvariantCulture, "({0} IS NULL)", colNameEsc),
                string.Empty));
          }
        }
      }

      return BuildValueClustersResult.ListFilled;
    }

    /// <summary>
    ///   Builds the data grid view column filter values.
    /// </summary>
    /// <param name="dataTable">The data view.</param>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="maxNumber">The maximum number.</param>
    /// <returns></returns>
    private BuildValueClustersResult BuildValueClusterString(DataTable dataTable, int columnIndex, int maxNumber)
    {
      Contract.Requires(dataTable != null);
      Contract.Requires(dataTable.Rows != null);

      // Get the distinct values and their counts
      var cluster = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      foreach (DataRow dataRow in dataTable.Rows)
      {
        var key = dataRow[columnIndex] == DBNull.Value
                    ? ColumnFilterLogic.OperatorIsNull
                    : dataRow[columnIndex].ToString();
        cluster.Add(key);

        // if we have more than the maximum entries stop, no value filter will be used
        if (cluster.Count <= maxNumber)
          continue;
        m_ValueClusters.Clear();
        return BuildValueClustersResult.TooManyValues;
      }

      if (cluster.Count == 0)
      {
        m_ValueClusters.Clear();
        return BuildValueClustersResult.NoValues;
      }

      foreach (var text in cluster)
      {
        m_ValueClusters.Add(new ValueCluster(text, string.Empty, text == ColumnFilterLogic.OperatorIsNull ? string.Empty : text));
      }

      return BuildValueClustersResult.ListFilled;
    }

    private void UpdateValueClustersDate(DataView dataView, int columnIndex)
    {
      // Get the distinct values and their counts
      foreach (DataRowView dataRow in dataView)
      {
        string sort = null;
        if (dataRow[columnIndex] == DBNull.Value)
        {
          sort = string.Empty;
        }
        else
        {
          var value = (DateTime)dataRow[columnIndex];
          switch (m_Type)
          {
            case ValueClustersGroupType.DateHours:
              sort = GetHourSort(GetHourKey(value));
              break;

            case ValueClustersGroupType.DateDay:
              sort = GetDaySort(value);
              break;

            case ValueClustersGroupType.DateYear:
              sort = GetYearSort(GetYearKey(value));
              break;

            case ValueClustersGroupType.DateMonth:
              sort = GetMonthSort(GetMonthKey(value));
              break;
          }
        }

        if (sort == null)
          continue;
        foreach (var item in this.m_ValueClusters.Where(item => sort.Equals(item.Sort, StringComparison.Ordinal)))
        {
          item.Count++;
          break;
        }
      }
    }

    private void UpdateValueClustersNumeric(DataView dataView, int columnIndex)
    {
      foreach (DataRowView dataRow in dataView)
      {
        string sort = null;
        if (dataRow[columnIndex] == DBNull.Value)
        {
          sort = string.Empty;
        }
        else
        {
          var key = Convert.ToInt64(dataRow[columnIndex], CultureInfo.CurrentCulture);
          switch (m_Type)
          {
            case ValueClustersGroupType.NumericFraction:
              sort = Math.Floor(Convert.ToDouble(dataRow[columnIndex], CultureInfo.CurrentCulture) * 10d).ToString(
                "0000000000000000000",
                CultureInfo.InvariantCulture);
              break;

            case ValueClustersGroupType.NumericOnes:
              sort = key.ToString("0000000000000000000", CultureInfo.InvariantCulture);
              break;

            case ValueClustersGroupType.NumericTens:
              sort = (key / 10).ToString("0000000000000000000", CultureInfo.InvariantCulture);
              break;

            case ValueClustersGroupType.NumericHundreds:
              sort = (key / 100).ToString("0000000000000000000", CultureInfo.InvariantCulture);
              break;

            case ValueClustersGroupType.NumericThousands:
              sort = (key / 1000).ToString("0000000000000000000", CultureInfo.InvariantCulture);
              break;
          }
        }

        if (sort == null)
          continue;
        foreach (var item in this.m_ValueClusters.Where(item => sort.Equals(item.Sort, StringComparison.Ordinal)))
        {
          item.Count++;
          break;
        }
      }
    }

    private void UpdateValueClustersString(DataView dataView, int columnIndex)
    {
      foreach (DataRowView dataRow in dataView)
      {
        var key = dataRow[columnIndex] == DBNull.Value
                    ? ColumnFilterLogic.OperatorIsNull
                    : dataRow[columnIndex].ToString();

        foreach (var item in this.m_ValueClusters.Where(item => key.Equals(item.Sort, StringComparison.Ordinal)))
        {
          item.Count++;
          break;
        }
      }
    }
  }
}