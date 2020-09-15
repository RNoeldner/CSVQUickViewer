﻿// /*
// * Copyright (C) 2014 Raphael Nöldner : http://csvquickviewer.com *
// * This program is free software: you can redistribute it and/or modify it under the terms of the
//   GNU Lesser Public
// * License as published by the Free Software Foundation, either version 3 of the License, or (at
//   your option) any later version. *
// * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
//   without even the implied warranty
// * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser Public License for
//   more details. *
// * You should have received a copy of the GNU Lesser Public License along with this program.
// * If not, see http://www.gnu.org/licenses/ . *
// */

using FastColoredTextBoxNS;

namespace CsvTools
{
  public class SyntaxHighlighterXML : SyntaxHighlighterBase
  {
    public SyntaxHighlighterXML(FastColoredTextBox currentTb) : base(currentTb)
    {
      InitXMLRegex();
    }

    public override void Highlight(Range range)
    {
      base.XMLSyntaxHighlight(range);
    }
  }
}