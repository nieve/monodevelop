
namespace MonoDevelop.Stereo.Gui
{
	public class NamespaceValidator : INameValidator
	{
		public NamespaceValidator ()
		{
		}

		public bool ValidateName (string name)
		{
			if (string.IsNullOrEmpty(name))
				return false; // ValidationResult.CreateError(GettextCatalog.GetString("Name must not be empty."));
			char c1 = name[0];
			if (!char.IsLetter(c1) && (int) c1 != 95)
				return false; // ValidationResult.CreateError(GettextCatalog.GetString("Name must start with a letter or '_'"));
			char lc = name[name.Length - 1];
			if (!char.IsLetterOrDigit(lc) && (int) lc != 95)
				return false; // ValidationResult.CreateError("Name can only end with a letter, digit and '_'");
			for (int index = 1; index < name.Length - 1; ++index) {
				char c2 = name[index];
				if (!char.IsLetterOrDigit(c2) && c2 != '.' && (int) c2 != 95)
					return false; // ValidationResult.CreateError("Name can only contain letters, digits, dots and '_'");
			}
			return true;
		}
	}
}

