namespace TheSaga.Persistance.Sqlite.Utils
{
    public static class TemplateHelper
    {
        public static string CorrectTemplateName(string name)
        {
            var changedName = name;

            if (changedName.Trim() != "")
                changedName = changedName.Trim();

            changedName = changedName.
                Replace(".", "_").Replace("-", "_").Replace(" ", "_").
                Replace("[", "_").Replace("]", "_").
                Replace("__", "_").Replace("__", "_");

            return changedName;
        }
    }
}
