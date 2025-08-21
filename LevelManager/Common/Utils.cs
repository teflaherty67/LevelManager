
namespace LevelManager.Common
{
    internal static class Utils
    {
        internal static RibbonPanel CreateRibbonPanel(UIControlledApplication app, string tabName, string panelName)
        {
            RibbonPanel curPanel;

            if (GetRibbonPanelByName(app, tabName, panelName) == null)
                curPanel = app.CreateRibbonPanel(tabName, panelName);

            else
                curPanel = GetRibbonPanelByName(app, tabName, panelName);

            return curPanel;
        }

        internal static List<View> GetAllSectionViews(Document curDoc)
        {
            throw new NotImplementedException();
        }

        internal static List<Level> GetFilteredAndSortedLevels(Document curDoc)
        {
            throw new NotImplementedException();
        }

        internal static RibbonPanel GetRibbonPanelByName(UIControlledApplication app, string tabName, string panelName)
        {
            foreach (RibbonPanel tmpPanel in app.GetRibbonPanels(tabName))
            {
                if (tmpPanel.Name == panelName)
                    return tmpPanel;
            }

            return null;
        }

        internal static void TaskDialogInformation(string v1, string v2, string v3)
        {
            throw new NotImplementedException();
        }
    }
}
