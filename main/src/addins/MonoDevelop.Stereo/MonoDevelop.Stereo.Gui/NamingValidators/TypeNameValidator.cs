using System;
using MonoDevelop.Core;

namespace MonoDevelop.Stereo.Gui
{
	public class TypeNameValidator : INameValidator
	{
		public TypeNameValidator ()
		{
		}

		public bool ValidateName (string name)
		{
			if (string.IsNullOrEmpty(name.Trim()))
				return false; // ValidationResult.CreateError(GettextCatalog.GetString("Name must not be empty."));
			
			char c1 = name[0];
			if (!char.IsLetter(c1))
				return false; // ValidationResult.CreateError(GettextCatalog.GetString("Name must start with a letter"));
			
			for (int index = 1; index < name.Length - 1; ++index) {
				char c2 = name[index];
				if (!char.IsLetterOrDigit(c2))
					return false; // ValidationResult.CreateError("Name can only contain letters or digits");
			}
			return true;
		}
	}
}

