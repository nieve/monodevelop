//
// FoldingTests.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (C)  2009  Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Mono.TextEditor.Tests
{
	[TestFixture()]
	public class FoldingTests
	{
		static List<FoldSegment> GetFoldSegments (Document doc)
		{
			List<FoldSegment> result = new List<FoldSegment> ();
			Stack<FoldSegment> foldSegments = new Stack<FoldSegment> ();
			
			for (int i = 0; i < doc.Length - 1; ++i) {
				char ch = doc.GetCharAt (i);
				
				if ((ch == '+' || ch == '-') && doc.GetCharAt(i + 1) == '[') {
					FoldSegment segment = new FoldSegment (doc, "...", i, 0, FoldingType.None);
					segment.IsFolded = ch == '+';
					foldSegments.Push (segment);
				} else if (ch == ']' && foldSegments.Count > 0) {
					FoldSegment segment = foldSegments.Pop ();
					segment.Length = i - segment.Offset;
					result.Add (segment);
				}
			}
			return result;
		}
		
		[Test()]
		public void TestLogicalToVisualLine ()
		{
			var data = CaretMoveActionTests.Create (
@"-[1
+[2
3
]4
5
+[6
+[7
+[8
9
]10
]11
]12
]13
14
+[15
16
]17
18");
			data.Document.UpdateFoldSegments (GetFoldSegments (data.Document), false);
			Assert.AreEqual (5, data.LogicalToVisualLine (12));
			Assert.AreEqual (8, data.LogicalToVisualLine (16));
			Assert.AreEqual (8, data.LogicalToVisualLine (17));
		}
		
		[Test()]
		public void TestLogicalToVisualLineStartLine ()
		{
			var data = CaretMoveActionTests.Create (
@"-[1
-[2
3
]4
5
-[6
+[7
-[8
9
]10
]11
]12
]13
14
-[15
16
]17
18");
			data.Document.UpdateFoldSegments (GetFoldSegments (data.Document), false);
			Assert.AreEqual (7, data.LogicalToVisualLine (7));
		}
		
		[Test()]
		public void TestVisualToLogicalLineStartLine ()
		{
			var data = CaretMoveActionTests.Create (
@"-[1
-[2
3
]4
5
-[6
+[7
-[8
9
]10
]11
]12
]13
14
-[15
16
]17
18");
			data.Document.UpdateFoldSegments (GetFoldSegments (data.Document), false);
			Assert.AreEqual (7, data.VisualToLogicalLine (7));
		}
		
		[Test()]
		public void TestVisualToLogicalLineCase2 ()
		{
			var data = CaretMoveActionTests.Create (
@"-[1
+[2
3
]4
5
+[6
+[7
+[8
9
]10
]11
]12
]13
14
+[15
16
]17
18");
			data.Document.UpdateFoldSegments (GetFoldSegments (data.Document), false);
			Assert.AreEqual (6, data.VisualToLogicalLine (4));
			Assert.AreEqual (14, data.VisualToLogicalLine (6));
			Assert.AreEqual (15, data.VisualToLogicalLine (7));
		}
		
		[Test()]
		public void TestVisualToLogicalLineCase3 ()
		{
			var data = CaretMoveActionTests.Create (
@"-[1
+[2
3
]4
5
+[6
+[7
+[8
9
]10
]11
]12
]13
14
+[15
16
]17
18");
			data.Document.UpdateFoldSegments (GetFoldSegments (data.Document), false);
			Assert.AreEqual (2, data.VisualToLogicalLine (2));
			Assert.AreEqual (2, data.LogicalToVisualLine (2));
		}

		[Test()]
		public void TestUpdateFoldSegmentBug ()
		{
			var data = CaretMoveActionTests.Create (
@"-[0
1
+[2
3
4
5
6
7
8
9
10]
11
]12
13
+[14
15
16
17
18
19
20
21
22]
23
24
25
26");
			var segments = GetFoldSegments (data.Document);
			data.Document.UpdateFoldSegments (segments, false);
			Assert.AreEqual (25, data.VisualToLogicalLine (9));
			Assert.AreEqual (3, data.Document.FoldSegments.Count ());
			segments.RemoveAt (1);
			
			
			data.Document.UpdateFoldSegments (segments, false);
			
			Assert.AreEqual (2, data.Document.FoldSegments.Count ());
			Assert.AreEqual (17, data.LogicalToVisualLine (25));
			segments.RemoveAt (1);
			data.Document.UpdateFoldSegments (segments, false);
			Assert.AreEqual (1, data.Document.FoldSegments.Count ());
			Assert.AreEqual (25, data.LogicalToVisualLine (25));
		}
		
		/// <summary>
		/// Bug 682466 - Rendering corruption and jumping in text editor
		/// </summary>
		[Test()]
		public void TestBug682466 ()
		{
			var data = CaretMoveActionTests.Create (
@"0
1
2
+[3
4
5
6]
7
8
9
10");
			var segments = GetFoldSegments (data.Document);
			data.Document.UpdateFoldSegments (segments, false);
			Assert.AreEqual (true, data.Document.FoldSegments.FirstOrDefault ().IsFolded);
			segments = GetFoldSegments (data.Document);
			segments[0].IsFolded = false;
			data.Document.UpdateFoldSegments (segments, false);
			Assert.AreEqual (5, data.LogicalToVisualLine (8));
		}
		
		[Test()]
		public void TestVisualToLogicalLine ()
		{
			var data = CaretMoveActionTests.Create (
@"-[0
+[1
2
]3
4
+[5
+[6
+[7
8
]9
]10
]11
]12
13
+[14
15
]16
17");
			data.Document.UpdateFoldSegments (GetFoldSegments (data.Document), false);
			Assert.AreEqual (13, data.VisualToLogicalLine (5));
			Assert.AreEqual (18, data.VisualToLogicalLine (8));
		}
		
		
		
		[Test()]
		public void TestCaretRight ()
		{
			var data = CaretMoveActionTests.Create (
@"1234567890
1234567890
123$4+[567890
1234]567890
1234567890");
			data.Document.UpdateFoldSegments (GetFoldSegments (data.Document), false);
			CaretMoveActions.Right (data);
			Assert.AreEqual (new DocumentLocation (3, 5), data.Caret.Location);
			CaretMoveActions.Right (data);
			Assert.AreEqual (new DocumentLocation (4, 5), data.Caret.Location);
		}
		
		[Test()]
		public void TestCaretLeft ()
		{
			var data = CaretMoveActionTests.Create (
@"1234567890
1234567890
1234+[567890
1234]5$67890
1234567890");
			data.Document.UpdateFoldSegments (GetFoldSegments (data.Document), false);
			CaretMoveActions.Left (data);
			Assert.AreEqual (new DocumentLocation (4, 6), data.Caret.Location);
			CaretMoveActions.Left (data);
			Assert.AreEqual (new DocumentLocation (3, 5), data.Caret.Location);
		}
		
		[Test()]
		public void TestCaretLeftCase2 ()
		{
			var data = CaretMoveActionTests.Create (
@"1234567890
1234567890
1234+[567890
1234567890]
$1234567890");
			data.Document.UpdateFoldSegments (GetFoldSegments (data.Document), false);
			CaretMoveActions.Left (data);
			Assert.AreEqual (new DocumentLocation (4, 12), data.Caret.Location);
			CaretMoveActions.Left (data);
			Assert.AreEqual (new DocumentLocation (3, 5), data.Caret.Location);
		}
		
		
		[Test()]
		public void TestUpdateFoldSegmentBug2 ()
		{
			var data = CaretMoveActionTests.Create (
@"-[1
2
+[3
4]
5
+[6
7]
8
9
10
11
12
13
14]
15
16");
			var segments = GetFoldSegments (data.Document);
			data.Document.UpdateFoldSegments (segments, false);
			Assert.AreEqual (10, data.VisualToLogicalLine (8));
			Assert.AreEqual (3, data.Document.FoldSegments.Count ());
			int start = data.GetLine (2).Offset;
			int end = data.GetLine (8).Offset;
			data.Remove (start, end - start);
			Assert.AreEqual (1, data.Document.FoldSegments.Count ());
			Assert.AreEqual (10, data.LogicalToVisualLine (10));
		}
		
		[Test()]
		public void TestGetStartFoldingsGetStartFoldings ()
		{
			var data = CaretMoveActionTests.Create (
@"+[1
2
3
+[4
5
+[6
7]
8]
+[9
10
11]
12]
+[13
14]
15
16");
			var segments = GetFoldSegments (data.Document);
			data.Document.UpdateFoldSegments (segments, false);
			data.Document.UpdateFoldSegments (segments, false);
			data.Document.UpdateFoldSegments (segments, false);
			
			Assert.AreEqual (1, data.Document.GetStartFoldings (1).Count ());
			Assert.AreEqual (1, data.Document.GetStartFoldings (4).Count ());
			Assert.AreEqual (1, data.Document.GetStartFoldings (6).Count ());
			Assert.AreEqual (1, data.Document.GetStartFoldings (9).Count ());
			Assert.AreEqual (1, data.Document.GetStartFoldings (13).Count ());
		}
		
		[Test()]
		public void TestIsFoldedSetFolded ()
		{
			var data = CaretMoveActionTests.Create (
@"-[1
2
3
-[4
5
-[6
7]
8]
-[9
10
11]
12]
-[13
14]
15
16");
			var segments = GetFoldSegments (data.Document);
			data.Document.UpdateFoldSegments (segments, false);
			Assert.AreEqual (15, data.LogicalToVisualLine (15));
			data.Document.GetStartFoldings (6).First ().IsFolded = true;
			data.Document.GetStartFoldings (4).First ().IsFolded = true;
			Assert.AreEqual (11, data.LogicalToVisualLine (15));
		}
		
		[Test()]
		public void TestIsFoldedUnsetFolded ()
		{
			var data = CaretMoveActionTests.Create (
@"-[1
2
3
+[4
5
+[6
7]
8]
-[9
10
11]
12]
-[13
14]
15
16");
			var segments = GetFoldSegments (data.Document);
			data.Document.UpdateFoldSegments (segments, false);
			Assert.AreEqual (11, data.LogicalToVisualLine (15));
			data.Document.GetStartFoldings (6).First ().IsFolded = false;
			data.Document.GetStartFoldings (4).First ().IsFolded = false;
			Assert.AreEqual (15, data.LogicalToVisualLine (15));
		}
		
		[Test()]
		public void TestCaretDown ()
		{
			var data = CaretMoveActionTests.Create (
@"AAAAAAAA
AAAAAAAA$
AAAAAAAA+[BBBBBBB
AAAAAAAABBBBBBBBBB
AAAAAAAABBBBBBBBBB
AAAAAAAABBBBBBBBBB
AAAAAAAABBBBBBBBBB]
AAAAAAAA
");
			data.Document.UpdateFoldSegments (GetFoldSegments (data.Document), false);
			
			Assert.AreEqual (new DocumentLocation (2, 9), data.Caret.Location);
			CaretMoveActions.Down (data);
			CaretMoveActions.Down (data);
			Assert.AreEqual (true, data.Document.FoldSegments.First ().IsFolded);
			Assert.AreEqual (new DocumentLocation (8, 9), data.Caret.Location);
		}
		
		[Test()]
		public void TestCaretUp ()
		{
			var data = CaretMoveActionTests.Create (
@"AAAAAAAA
AAAAAAAA
AAAAAAAA+[BBBBBBB
AAAAAAAABBBBBBBBBB
AAAAAAAABBBBBBBBBB
AAAAAAAABBBBBBBBBB
AAAAAAAABBBBBBBBBB]
AAAAAAAA$
");
			data.Document.UpdateFoldSegments (GetFoldSegments (data.Document), false);
			
			Assert.AreEqual (new DocumentLocation (8, 9), data.Caret.Location);
			CaretMoveActions.Up (data);
			Assert.AreEqual (true, data.Document.FoldSegments.First ().IsFolded);
			Assert.AreEqual (new DocumentLocation (3, 9), data.Caret.Location);
		}
		
		[Test()]
		public void TestCaretUpCase2 ()
		{
			var data = CaretMoveActionTests.Create (
@"AAAAAAAA
AAAAAAAA
AA+[AAAAAABBBBBBB
AAAAAAAABBBBBBBBBB
AAAAAAAABBBBBBBBBB
AAAAAAAABBBBBBBBBB
AAAAAAAABBBBBBBBBB]
AAAAAAAA$
");
			data.Document.UpdateFoldSegments (GetFoldSegments (data.Document), false);
			
			Assert.AreEqual (new DocumentLocation (8, 9), data.Caret.Location);
			CaretMoveActions.Up (data);
			Assert.AreEqual (true, data.Document.FoldSegments.First ().IsFolded);
			Assert.AreEqual (new DocumentLocation (3, 3), data.Caret.Location);
		}
		
		/// <summary>
		/// Bug 1134 - Visually corrupted text when changing line
		/// </summary>
		[Test()]
		public void TestBug1134 ()
		{
			var data = CaretMoveActionTests.Create (
@"0
1
-[2
3
4]
5
-[6
7
+[8
9]
10]
11");
			var segments = GetFoldSegments (data.Document);
			var seg = segments[0];
			segments.RemoveAt (0);
			data.Document.UpdateFoldSegments (segments, false);
			Assert.AreEqual (2, data.Document.FoldSegments.Count ());
			
			segments.Insert (0, seg);
			data.Document.UpdateFoldSegments (segments, false);
			Assert.AreEqual (3, data.Document.FoldSegments.Count ());
			
		}	
	}
}
