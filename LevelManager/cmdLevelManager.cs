using LevelManager.Classes;
using LevelManager.Common;

namespace LevelManager
{
    [Transaction(TransactionMode.Manual)]
    public class cmdLevelManager : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document curDoc = uidoc.Document;

            #region Get Data

            // get filtered & sorted list of levels in the document
            List<Level> filteredLevels = Utils.GetFilteredAndSortedLevels(curDoc);

            // get all the ViewSection views
            List<View> listViews = Utils.GetAllSectionViews(curDoc);

            // get the first view whose Title on Sheet is "Front Elevation"
            View elevFront = listViews
                .FirstOrDefault(v => v.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION)?.AsString() == "Front Elevation");

            // set that view as the active view
            if (elevFront != null)
            {
                uidoc.ActiveView = elevFront;
            }
            else
            {
                Utils.TaskDialogInformation("Information", "Level Manager", "Front Elevation view not found. Proceeding with level management in current view.");
            }

            // create a counter for summary report
            int countAdjusted = 0;

            #endregion

            #region Form

            // launch the form with level data
            frmLevelManager curForm = new frmLevelManager(filteredLevels);
            curForm.Topmost = true; // ensure the form is on top

            curForm.ShowDialog();

            // check if user clicked Cancel
            if (curForm.DialogResult != true)
            {
                return Result.Cancelled;
            }

            // get data from the form
            Dictionary<Level, double> levelAdjustments = curForm.LevelAdjustments;

            #endregion

            #region Level Adjustments
            // process level adjustments based on user input

            // create & start a transaction
            using (Transaction t = new Transaction(curDoc, "Plate Height Adjustements"))
            {
                // process the data from the form
                t.Start();

                // loop through the dictionary
                foreach (var kvp in levelAdjustments)
                {
                    // get the key value pairs
                    Level level = kvp.Key;
                    double adjustment = kvp.Value;

                    // only adjust if value is valid
                    if (adjustment != 0)
                    {
                        // adjust the level elevation
                        level.Elevation = level.Elevation + adjustment;

                        // increment the counter
                        countAdjusted++;
                    }
                }

                t.Commit();
            }

            #endregion

            #region Window Adjustments
            // adjust windows based on the level adjustments made

            // get all the window instances in the project
            List<FamilyInstance> allWindows = Utils.GetAllWindows(curDoc);

            // create a dictionary to hold the window data
            Dictionary<ElementId, clsWindowData> dictionaryWinData = new Dictionary<ElementId, clsWindowData>();

            // loop through the windows and get the data to store
            foreach (FamilyInstance curWindow in allWindows)
            {
                // store the data
                clsWindowData curData = new clsWindowData(curWindow);
                dictionaryWinData.Add(curWindow.Id, curData);
            }

            // check if adjust head heights is checked
            bool adjustHeadHeights = curForm.IsAdjustWindowHeadHeightsChecked();

            // check is adjust window heights is checked
            bool adjustWindowHeights = curForm.IsAdjustWindowHeightsChecked();

            // test for raising or lowering windows
            // bool raiseWindows = (selectedSpecLevel == "Complete Home Plus");

            // create counter for windows changed
            int countWindows = 0;

            // create a list for windows skipped
            List<string> skippedWindows = new List<string>();

            #region Adjust Head Heights

            // execute this code if adjust head heights is checked
            if (adjustHeadHeights)
            {
                // create and start a transaction
                using (Transaction t = new Transaction(curDoc, "Adjust Window Head Heights"))
                {
                    t.Start();

                    foreach (var kvp in dictionaryWinData)
                    {
                        clsWindowData curData = kvp.Value;
                        double plateAdjustment = 1.0;
                        double newHeadHeight;

                        if (!raiseWindows)
                        {
                            // lower window head heights by 12"
                            newHeadHeight = curData.CurHeadHeight - plateAdjustment;
                        }
                        else
                        {
                            // raise window head height by by 12"
                            newHeadHeight = curData.CurHeadHeight + plateAdjustment;
                        }

                        if (curData.HeadHeightParam != null && !curData.HeadHeightParam.IsReadOnly)
                        {
                            // adjust the head heihgt
                            curData.HeadHeightParam.Set(newHeadHeight);

                            // increment the counter
                            countWindows++;
                        }
                    }

                    t.Commit();
                }
            }

            #endregion

            #region Adjust Head Height & Window Height

            // execute this code if both boxes are checked
            if (adjustHeadHeights && adjustWindowHeights)
            {
                // create and start a transaction
                using (Transaction t = new Transaction(curDoc, "Adjust Window Head Heights & Window Heights"))
                {
                    t.Start();

                    foreach (var kvp in dictionaryWinData)
                    {
                        clsWindowData curData = kvp.Value;
                        double plateAdjustment = 1.0;
                        double newHeadHeight;

                        if (!raiseWindows)
                        {
                            // lower window head heights by 12"
                            newHeadHeight = curData.CurHeadHeight - plateAdjustment;
                        }
                        else
                        {
                            // raise window head height by by 12"
                            newHeadHeight = curData.CurHeadHeight + plateAdjustment;
                        }

                        if (curData.HeadHeightParam != null && !curData.HeadHeightParam.IsReadOnly)
                        {
                            // adjust the head heihgt
                            curData.HeadHeightParam.Set(newHeadHeight);

                            // increment the counter
                            countWindows++;

                            // adjust window heights
                            AdjustWindowHeights(curDoc, curData, plateAdjustment, raiseWindows, skippedWindows);
                        }
                    }

                    t.Commit();
                }
            }

            #endregion

            #endregion

            #region Summary Report

            // display a summary report of the adjustments made

            #endregion

            return Result.Succeeded;
        

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            clsButtonData myButtonData = new clsButtonData(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData.Data;
        }
    }
}
