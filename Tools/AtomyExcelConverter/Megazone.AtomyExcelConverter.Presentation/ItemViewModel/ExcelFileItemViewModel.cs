using Megazone.AtomyExcelConverter.Presentation.Infrastructure;

namespace Megazone.AtomyExcelConverter.Presentation.ItemViewModel
{
	internal class ExcelFileItemViewModel : ViewModelBase
	{
		public ExcelFileItemViewModel(string fileName, string fullPath)
		{
			DisplayName = fileName;
			DisplayFileFullPath = fullPath;
		}

		public string DisplayFileFullPath { get; }

		public string DisplayName { get; }
	}
}