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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace CsvTools.Tests
{
  [TestClass]
  public class HTMLStyleTests
  {
    [TestMethod]
    public void Properties()
    {
      var style = new HTMLStyle { Warning = "TestWar" };
      Assert.AreEqual("TestWar", style.Warning);

      style.BR = "TestBR";
      Assert.AreEqual("TestBR", style.BR);

      style.H1 = "TestH1";
      Assert.AreEqual("TestH1", style.H1);

      style.TDEmpty = "TestTD";
      Assert.AreEqual("TestTD", style.TDEmpty);

      style.TD = "TestTD";
      Assert.AreEqual("TestTD", style.TD);

      style.TDNonText = "TestTDNT";
      Assert.AreEqual("TestTDNT", style.TDNonText);

      style.TH = "TestTH";
      Assert.AreEqual("TestTH", style.TH);

      style.H2 = "TestH2";
      Assert.AreEqual("TestH2", style.H2);

      style.Style = "TestStyle";
      Assert.AreEqual("TestStyle", style.Style);

      style.TableClose = "TestCl";
      Assert.AreEqual("TestCl", style.TableClose);

      style.TableOpen = "TestOp";
      Assert.AreEqual("TestOp", style.TableOpen);

      style.TRClose = "TestTRCl";
      Assert.AreEqual("TestTRCl", style.TRClose);

      style.TROpen = "TestTROp";
      Assert.AreEqual("TestTROp", style.TROpen);

      style.TROpenAlt = "TestTROpA";
      Assert.AreEqual("TestTROpA", style.TROpenAlt);
    }

    [TestMethod]
    public void ConvertToHtmlFragment()
    {
      var style = new HTMLStyle();
      Assert.IsNotNull(style.ConvertToHtmlFragment("Hello"));
    }

    [TestMethod]
    public void HtmlEncodeShortTest()
    {
      Assert.IsNull(HTMLStyle.HtmlEncodeShort(null));
      Assert.AreEqual("", HTMLStyle.HtmlEncodeShort(""));
      Assert.AreEqual("&lt;&gt;", HTMLStyle.HtmlEncodeShort("<>"));
      Assert.AreEqual("HTML", HTMLStyle.HtmlEncodeShort("HTML"));
      Assert.AreEqual("K&amp;N", HTMLStyle.HtmlEncodeShort("K&N"));
      Assert.AreEqual("Line1<br>Line2", HTMLStyle.HtmlEncodeShort("Line1\r\nLine2"));
    }

    [TestMethod]
    public void HtmlEncodeTest()
    {
      Assert.AreEqual("", HTMLStyle.HtmlEncode(""));
      Assert.AreEqual("HTML", HTMLStyle.HtmlEncode("HTML"));
      Assert.AreEqual("K&amp;N", HTMLStyle.HtmlEncode("K&N"));
      Assert.AreEqual("he sated &quot;Hello&quot;", HTMLStyle.HtmlEncode("he sated \"Hello\""));

      /*ö latin small letter o with diaeresis
       * &ouml; or &#246;
       */
      Assert.IsTrue(@"Raphael N&ouml;ldner" == HTMLStyle.HtmlEncode("Raphael Nöldner") ||
                    @"Raphael N&#246;ldner" == HTMLStyle.HtmlEncode("Raphael Nöldner"));
    }

    [TestMethod]
    public void JSONElementNameTest()
    {
      Assert.AreEqual("", HTMLStyle.JsonElementName(""));
      Assert.AreEqual("HTmL", HTMLStyle.JsonElementName("HTmL"));
      Assert.AreEqual("RaphaelNöldner", HTMLStyle.JsonElementName("Raphael Nöldner"));
      Assert.AreEqual("_12", HTMLStyle.XmlElementName("12"));
    }

    [TestMethod]
    public void XMLElementNameTest()
    {
      /*
          Element names are case-sensitive
          Element names must start with a letter or underscore
          Element names cannot start with the letters xml (or XML, or Xml, etc)
          Element names can contain letters, digits, hyphens, underscores, and periods
          Element names cannot contain spaces
      */
      Assert.AreEqual("", HTMLStyle.XmlElementName(""));
      Assert.AreEqual("HTmL", HTMLStyle.XmlElementName("HTmL"));
      Assert.AreEqual("HTML", HTMLStyle.XmlElementName("HT;ML"));
      Assert.AreEqual("RaphaelNoldner", HTMLStyle.XmlElementName("Raphael Nöldner"));
      Assert.AreEqual("_12", HTMLStyle.XmlElementName("12"));
    }

    [TestMethod]
    public void TextToHtmlEncodeTest()
    {
      Assert.AreEqual("", HTMLStyle.TextToHtmlEncode(""));
      Assert.AreEqual("Raphael Nöldner", HTMLStyle.TextToHtmlEncode("Raphael  Nöldner"));
      Assert.AreEqual("Raphael Nöldner", HTMLStyle.TextToHtmlEncode("Raphael\t Nöldner"));
      Assert.AreEqual("Line1<br>Line2", HTMLStyle.TextToHtmlEncode("Line1\r\nLine2"));
    }

    [TestMethod]
    public void AddTDTest()
    {
      Assert.IsNull(HTMLStyle.AddTd(null));
      Assert.AreEqual("", HTMLStyle.AddTd(""));
      Assert.AreEqual("<1>", HTMLStyle.AddTd("<{0}>", "1"));
    }

    [TestMethod]
    public void AddHTMLCellTest()
    {
      HTMLStyle.AddHtmlCell(null, null, null, null, true);
      var sb = new StringBuilder();

      HTMLStyle.AddHtmlCell(sb, "<{0}>", "1", null, false);
      Assert.AreEqual("<1>", sb.ToString());

      sb = new StringBuilder();
      HTMLStyle.AddHtmlCell(sb, "<{0}>", "1", "Error", false);
      Assert.AreEqual("<1>", sb.ToString());
    }

    [TestMethod]
    public void AddHTMLCellTestWarnings()
    {
      var sb = new StringBuilder();
      HTMLStyle.AddHtmlCell(sb, "<{0}>", "Test", "Issue".AddWarningId(), true);
      Assert.IsTrue(sb.ToString().StartsWith("<Test"));
      Assert.IsTrue(sb.ToString().Contains("Issue"));
      Assert.IsTrue(sb.ToString().EndsWith(">"));
    }

    [TestMethod]
    public void AddHTMLCellTestErrorWarnings()
    {
      var errWar = "Some Error".AddMessage("Issue".AddWarningId());
      var sb = new StringBuilder();
      HTMLStyle.AddHtmlCell(sb, "<{0}>", "Test", errWar, true);
      Assert.IsTrue(sb.ToString().StartsWith("<Test"));
      Assert.IsTrue(sb.ToString().EndsWith(">"));
      Assert.IsTrue(sb.ToString().Contains("Issue"));
      Assert.IsTrue(sb.ToString().Contains("Some Error"));
    }

    [TestMethod]
    public void AddHTMLCellTestError()
    {
      var sb = new StringBuilder();
      HTMLStyle.AddHtmlCell(sb, "<{0}>", "Test", "Some Error", true);
      Assert.IsTrue(sb.ToString().StartsWith("<Test"));
      Assert.IsTrue(sb.ToString().Contains("Some Error"));
      Assert.IsTrue(sb.ToString().EndsWith(">"));
    }

    [TestMethod]
    public void ConvertToHtmlFragmentTest()
    {
      var style = new HTMLStyle();
      Assert.IsNotNull(style.ConvertToHtmlFragment("Test"));
      Assert.IsTrue(style.ConvertToHtmlFragment("Test").StartsWith("Version:1.0", StringComparison.Ordinal));
    }
  }
}