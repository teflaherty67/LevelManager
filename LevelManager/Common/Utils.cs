using LevelManager.Classes;

namespace LevelManager.Common
{
    internal static class Utils
    {
        #region Convert

        internal static double ConvertINToFT(double INDim)
        {
            double convert = INDim / 12;

            return convert;
        }

        #endregion

        #region Elements - Architectural

        //return list of all doors in the current model
        public static List<FamilyInstance> GetAllDoors(Document curDoc)
        {
            //get all doors
            var returnList = new FilteredElementCollector(curDoc)
                .OfClass(typeof(FamilyInstance))
                .OfCategory(BuiltInCategory.OST_Doors)
                .Cast<FamilyInstance>()
                .ToList();

            return returnList;
        }

        internal static List<FamilyInstance> GetAllDoorsInActiveView(Document curDoc)
        {
            // get all the doors in the current view
            var m_returnList = new FilteredElementCollector(curDoc, curDoc.ActiveView.Id)
                 .OfClass(typeof(FamilyInstance))
                 .OfCategory(BuiltInCategory.OST_Doors)
                 .Cast<FamilyInstance>()
                 .ToList();

            // return the list
            return m_returnList;
        }



        //return list of all windows in the current model
        public static List<FamilyInstance> GetAllWindows(Document curDoc)
        {
            //get all windows
            var returnList = new FilteredElementCollector(curDoc)
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .ToList();

            return returnList;
        }

        #endregion

        #region Levels

        public static List<Level> GetAllLevels(Document curDoc)
        {
            return new FilteredElementCollector(curDoc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .OfType<Level>()
                .ToList();
        }

        internal static List<Level> GetFilteredAndSortedLevels(Document curDoc)
        {
            // get all the levels
            List<Level> m_listLevels = Utils.GetAllLevels(curDoc);

            // filter and sort using ProjectConstants
            return m_listLevels
                .Where(level => !ProjectConstants.ExcludedLevelNames.Contains(level.Name))
                .OrderBy(level => level.Elevation)
                .ToList();
        }

        #endregion

        #region Ribbon Panel
        internal static RibbonPanel CreateRibbonPanel(UIControlledApplication app, string tabName, string panelName)
        {
            RibbonPanel curPanel;

            if (GetRibbonPanelByName(app, tabName, panelName) == null)
                curPanel = app.CreateRibbonPanel(tabName, panelName);

            else
                curPanel = GetRibbonPanelByName(app, tabName, panelName);

            return curPanel;
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

        #endregion

        #region Task Dialog


        /// <summary>
        /// Displays a warning dialog to the user with custom title and message
        /// </summary>
        /// <param name="tdName">The internal name of the TaskDialog</param>
        /// <param name="tdTitle">The title displayed in the dialog header</param>
        /// <param name="textMessage">The main message content to display to the user</param>
        internal static void TaskDialogWarning(string tdName, string tdTitle, string textMessage, bool allowCancel)
        {
            // Create a new TaskDialog with the specified name
            TaskDialog m_Dialog = new TaskDialog(tdName);

            // Set the warning icon to indicate this is a warning message
            m_Dialog.MainIcon = Icon.TaskDialogIconWarning;

            // Set the custom title for the dialog
            m_Dialog.Title = tdTitle;

            // Disable automatic title prefixing to use our custom title exactly as specified
            m_Dialog.TitleAutoPrefix = false;

            // Set the main message content that will be displayed to the user
            m_Dialog.MainContent = textMessage;

            // Add appropriate buttons based on parameter
            m_Dialog.CommonButtons = allowCancel ?
                TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel :
                TaskDialogCommonButtons.Close;

            // Display the dialog and capture the result (though we don't use it for warnings)
            TaskDialogResult m_DialogResult = m_Dialog.Show();
        }

        /// <summary>
        /// Displays an information dialog to the user with custom title and message
        /// </summary>
        /// <param name="tdName">The internal name of the TaskDialog</param>
        /// <param name="tdTitle">The title displayed in the dialog header</param>
        /// <param name="textMessage">The main message content to display to the user</param>
        internal static void TaskDialogInformation(string tdName, string tdTitle, string textMessage)
        {
            // Create a new TaskDialog with the specified name
            TaskDialog m_Dialog = new TaskDialog(tdName);

            // Set the warning icon to indicate this is a warning message
            m_Dialog.MainIcon = Icon.TaskDialogIconInformation;

            // Set the custom title for the dialog
            m_Dialog.Title = tdTitle;

            // Disable automatic title prefixing to use our custom title exactly as specified
            m_Dialog.TitleAutoPrefix = false;

            // Set the main message content that will be displayed to the user
            m_Dialog.MainContent = textMessage;

            // Add a Close button for the user to dismiss the dialog
            m_Dialog.CommonButtons = TaskDialogCommonButtons.Close;

            // Display the dialog and capture the result (though we don't use it for warnings)
            TaskDialogResult m_DialogResult = m_Dialog.Show();
        }

        /// <summary>
        /// Displays an error dialog to the user with custom title and message
        /// </summary>
        /// <param name="tdName">The internal name of the TaskDialog</param>
        /// <param name="tdTitle">The title displayed in the dialog header</param>
        /// <param name="textMessage">The main message content to display to the user</param>
        internal static void TaskDialogError(string tdName, string tdTitle, string textMessage)
        {
            // Create a new TaskDialog with the specified name
            TaskDialog m_Dialog = new TaskDialog(tdName);

            // Set the warning icon to indicate this is a warning message
            m_Dialog.MainIcon = Icon.TaskDialogIconError;

            // Set the custom title for the dialog
            m_Dialog.Title = tdTitle;

            // Disable automatic title prefixing to use our custom title exactly as specified
            m_Dialog.TitleAutoPrefix = false;

            // Set the main message content that will be displayed to the user
            m_Dialog.MainContent = textMessage;

            // Add a Close button for the user to dismiss the dialog
            m_Dialog.CommonButtons = TaskDialogCommonButtons.Close;

            // Display the dialog and capture the result (though we don't use it for warnings)
            TaskDialogResult m_DialogResult = m_Dialog.Show();
        }

        #endregion

        #region Views

        public static List<View> GetAllSectionViews(Document curDoc)
        {
            //get all ViewSection views
            FilteredElementCollector m_colViews = new FilteredElementCollector(curDoc)
                .OfCategory(BuiltInCategory.OST_Views)
                .OfClass(typeof(ViewSection));

            // create an empty list to hold the views
            List<View> m_Views = new List<View>();

            // loop through each view & add it to the list
            foreach (View x in m_colViews)
            {
                if (x.IsTemplate == false)
                {
                    m_Views.Add(x);
                }
            }

            // return the list of views
            return m_Views;
        }

        internal static List<FamilyInstance> GetWindowsByLevel(Document curDoc, string levelName)
        {
            // get all windows
            List<FamilyInstance> m_allWindows = Utils.GetAllWindows(curDoc);

            // filter list by level name
            List<FamilyInstance> m_windowsByLevel = m_allWindows
                .Where(win => win.LevelId != null)
                .Where(win => {
                    var level = curDoc.GetElement(win.LevelId) as Level;
                    return level != null && level.Name == levelName;
                })
                .ToList();

            return m_windowsByLevel;
        }

        #endregion
    }
}
