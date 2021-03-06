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

namespace CsvTools
{
  using System;
  using System.ComponentModel;
  using System.Xml.Serialization;

  /// <summary>
  ///   Class containing the all configuration, used in serialization to store the settings
  /// </summary>
  [Serializable]
  public class ViewSettings : CsvFile
  {
    [XmlElement]
    public WindowState WindowPosition = new WindowState();

    private bool m_AllowJson = true;
    private bool m_DetectFileChanges = true;
    private FillGuessSettings m_FillGuessSettings = new FillGuessSettings();
    private bool m_GuessCodePage = true;
    private bool m_GuessDelimiter = true;
    private bool m_GuessHasHeader = true;
    private bool m_GuessNewLine = true;
    private bool m_GuessComment = true;
    private bool m_GuessQualifier;
    private bool m_GuessStartRow = true;
    private HTMLStyle m_HtmlStyle = new HTMLStyle();
    private bool m_MenuDown;
    private bool m_StoreSettingsByFile;

    public enum DurationEnum
    {
      [Description("unlimited")]
      Unlimited,

      [Description("1/2 second")]
      HalfSecond,

      [Description("1 second")]
      Second,

      [Description("2 seconds")]
      TwoSecond,

      [Description("10 seconds")]
      TenSecond,
    }

    [XmlAttribute]
    [DefaultValue(true)]
    public bool AllowJson
    {
      get => m_AllowJson;
      set
      {
        if (m_AllowJson == value)
          return;
        m_AllowJson = value;
        NotifyPropertyChanged(nameof(AllowJson));
      }
    }

    [XmlAttribute]
    [DefaultValue(true)]
    public bool DetectFileChanges
    {
      get => m_DetectFileChanges;
      set
      {
        if (m_DetectFileChanges == value)
          return;
        m_DetectFileChanges = value;
        NotifyPropertyChanged(nameof(DetectFileChanges));
      }
    }

    [XmlIgnore]
    public TimeSpan Duration
    {
      get
      {
        switch (LimitDuration)
        {
          case DurationEnum.HalfSecond:
            return TimeSpan.FromSeconds(.5);

          case DurationEnum.Second:
            return TimeSpan.FromSeconds(1);

          case DurationEnum.TwoSecond:
            return TimeSpan.FromSeconds(2);

          case DurationEnum.TenSecond:
            return TimeSpan.FromSeconds(10);

          default:
            return TimeSpan.MaxValue;
        }
      }
    }

    /// <summary>
    ///   Gets or sets the fill guess settings.
    /// </summary>
    /// <value>The fill guess settings.</value>
    [XmlElement]
    public virtual FillGuessSettings FillGuessSettings
    {
      get => m_FillGuessSettings;
      set
      {
        var newVal = value ?? new FillGuessSettings();
        if (ReferenceEquals(m_FillGuessSettings, newVal))
          return;
        m_FillGuessSettings = newVal;
        NotifyPropertyChanged(nameof(FillGuessSettings));
      }
    }

    /// <summary>
    ///   Gets a value indicating whether fill guess settings as specified
    /// </summary>
    /// <value><c>true</c> if [fill guess settings specified]; otherwise, <c>false</c>.</value>
    [XmlIgnore]
    public virtual bool FillGuessSettingsSpecified => !m_FillGuessSettings.Equals(new FillGuessSettings());

    [XmlAttribute]
    [DefaultValue(true)]
    public bool GuessCodePage
    {
      get => m_GuessCodePage;
      set
      {
        if (m_GuessCodePage == value)
          return;
        m_GuessCodePage = value;
        NotifyPropertyChanged(nameof(GuessCodePage));
      }
    }

    [XmlAttribute]
    [DefaultValue(true)]
    public bool GuessDelimiter
    {
      get => m_GuessDelimiter;

      set
      {
        if (m_GuessDelimiter == value)
          return;
        m_GuessDelimiter = value;
        NotifyPropertyChanged(nameof(GuessDelimiter));
      }
    }

    [XmlAttribute]
    [DefaultValue(true)]
    public bool GuessHasHeader
    {
      get => m_GuessHasHeader;
      set
      {
        if (m_GuessHasHeader == value)
          return;
        m_GuessHasHeader = value;
        NotifyPropertyChanged(nameof(GuessHasHeader));
      }
    }

    [XmlAttribute]
    [DefaultValue(true)]
    public bool GuessNewLine
    {
      get => m_GuessNewLine;

      set
      {
        if (m_GuessNewLine == value)
          return;
        m_GuessNewLine = value;
        NotifyPropertyChanged(nameof(GuessNewLine));
      }
    }

    [XmlAttribute]
    [DefaultValue(true)]
    public bool GuessComment
    {
      get => m_GuessComment;

      set
      {
        if (m_GuessComment == value)
          return;
        m_GuessComment = value;
        NotifyPropertyChanged(nameof(GuessComment));
      }
    }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool GuessQualifier
    {
      get => m_GuessQualifier;
      set
      {
        if (m_GuessQualifier == value)
          return;
        m_GuessQualifier = value;
        NotifyPropertyChanged(nameof(GuessQualifier));
      }
    }

    [XmlAttribute]
    [DefaultValue(true)]
    public bool GuessStartRow
    {
      get => m_GuessStartRow;
      set
      {
        if (m_GuessStartRow == value)
          return;
        m_GuessStartRow = value;
        NotifyPropertyChanged(nameof(GuessStartRow));
      }
    }

    [XmlIgnore]
    public HTMLStyle HTMLStyle => m_HtmlStyle;

    [XmlElement]
    [DefaultValue(DurationEnum.Second)]
    public DurationEnum LimitDuration
    {
      get;
      set;
    } = DurationEnum.Second;

    [XmlAttribute]
    [DefaultValue(false)]
    public bool MenuDown
    {
      get => m_MenuDown;
      set
      {
        if (m_MenuDown == value)
          return;
        m_MenuDown = value;
        NotifyPropertyChanged(nameof(MenuDown));
      }
    }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool StoreSettingsByFile
    {
      get => m_StoreSettingsByFile;
      set
      {
        if (m_StoreSettingsByFile == value)
          return;
        m_StoreSettingsByFile = value;
        NotifyPropertyChanged(nameof(StoreSettingsByFile));
      }
    }

    [XmlElement(ElementName = "HTMLStyle")]
    public string Style
    {
      get => m_HtmlStyle.Style;
      set
      {
        var newVal = value ?? HTMLStyle.c_Style;
        if (m_HtmlStyle.Style.Equals(newVal))
          return;
        m_HtmlStyle = new HTMLStyle(newVal);
        NotifyPropertyChanged(nameof(Style));
      }
    }

    public static void CopyConfiguration(ICsvFile csvSrc, ICsvFile csvDest)
    {
      if (csvSrc == null || csvDest == null || ReferenceEquals(csvSrc, csvDest))
        return;

      csvDest.AllowRowCombining = csvSrc.AllowRowCombining;
      csvDest.CodePageId= csvSrc.CodePageId;
      csvDest.ByteOrderMark = csvSrc.ByteOrderMark;
      csvDest.ConsecutiveEmptyRows = csvSrc.ConsecutiveEmptyRows;
      csvDest.DisplayEndLineNo = csvSrc.DisplayEndLineNo;
      csvDest.DisplayRecordNo = csvSrc.DisplayRecordNo;
      csvDest.DisplayStartLineNo = csvSrc.DisplayStartLineNo;
      csvDest.DoubleDecode = csvSrc.DoubleDecode;
      csvDest.FileName = csvSrc.FileName;
      csvDest.HasFieldHeader = csvSrc.HasFieldHeader;
      csvDest.NoDelimitedFile = csvSrc.NoDelimitedFile;
      csvDest.NumWarnings = csvSrc.NumWarnings;
      csvDest.SkipDuplicateHeader = csvSrc.SkipDuplicateHeader;
      csvDest.SkipEmptyLines = csvSrc.SkipEmptyLines;
      csvDest.SkipRows = csvSrc.SkipRows;
      csvDest.TreatLFAsSpace = csvSrc.TreatLFAsSpace;
      csvDest.TreatNBSPAsSpace = csvSrc.TreatNBSPAsSpace;
      csvDest.TreatTextAsNull = csvSrc.TreatTextAsNull;
      csvDest.TreatUnknownCharacterAsSpace = csvSrc.TreatUnknownCharacterAsSpace;
      csvDest.TrimmingOption = csvSrc.TrimmingOption;
      csvDest.TryToSolveMoreColumns = csvSrc.TryToSolveMoreColumns;
      csvDest.WarnDelimiterInValue = csvSrc.WarnDelimiterInValue;
      csvDest.WarnEmptyTailingColumns = csvSrc.WarnEmptyTailingColumns;
      csvDest.WarnLineFeed = csvSrc.WarnLineFeed;
      csvDest.WarnNBSP = csvSrc.WarnNBSP;
      csvDest.WarnQuotes = csvSrc.WarnQuotes;
      csvDest.WarnQuotesInQuotes = csvSrc.WarnQuotesInQuotes;
      csvDest.WarnUnknownCharacter = csvSrc.WarnUnknownCharacter;
      csvSrc.FileFormat.CopyTo(csvDest.FileFormat);
    }
  }
}