using MonoDevelop.Core;
using NUnit.Framework;
using System;
using MonoDevelop.Stereo.Refactoring.Rename;
using MonoDevelop.Stereo.Gui;

namespace MonoDevelop.Stereo.NamespaceValidatorTest
{
	[TestFixture()]
	public class ValidateName
	{
		NamespaceValidator validator = new NamespaceValidator();
		
		[Test()]
		public void Invalidates_empty_name ()
		{
			Assert.IsFalse(validator.ValidateName(null));
			Assert.IsFalse(validator.ValidateName(string.Empty));
		}
		
		[Test()]
		public void Invalidates_first_char_digit ()
		{
			Assert.IsFalse(validator.ValidateName("3Test"));
		}
		
		[Test()]
		public void Invalidates_non_letter_first_char ()
		{
			Assert.IsFalse(validator.ValidateName(")Test"));
		}
		
		[Test()]
		public void Validates_first_char_underscore_or_letter ()
		{
			Assert.IsTrue(validator.ValidateName("_Test"));
			Assert.IsTrue(validator.ValidateName("Test"));
		}
		
		[Test()]
		public void Invalidates_last_char_dot ()
		{
			Assert.IsFalse(validator.ValidateName("Test."));
		}
		
		[Test()]
		public void Validates_Name_containing_a_dot ()
		{
			Assert.IsTrue(validator.ValidateName("Test.Foo"));
		}
	}
}

