namespace Megazone.AtomyExcelConverter.Presentation.Infrastructure.View
{
    public class ConfirmWindowCheckBoxParameter
    {
        public ConfirmWindowCheckBoxParameter(bool defaultCheckedValue, string checkBoxContent)
        {
            DefaultCheckedValue = defaultCheckedValue;
            CheckBoxContent = checkBoxContent;
        }

        public string CheckBoxContent { get; }

        public bool DefaultCheckedValue { get; }
    }
}
