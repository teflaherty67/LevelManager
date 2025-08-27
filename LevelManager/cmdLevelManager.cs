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

            // get user input from form
            bool firstFlrHeadHeights = curForm.IsFirstFloorHeadHeightsChecked();
            bool firstFlrWinHeights = curForm.IsFirstFloorWindowHeightsChecked();

            bool secondFlrHeadHeights = curForm.IsSecondFloorHeadHeightsChecked();
            bool secondFlrWinHeights = curForm.IsSecondFloorWindowHeightsChecked();

            // process windows if any boolean is true
            if (firstFlrHeadHeights || secondFlrHeadHeights)
            {
                // determine head height adjustement based on level adjustments made
                bool adjustFirstFlrHeadHeights = DetermineHeadHeightAdjustment("Plate 1", levelAdjustments);
                bool adjustSecondFlrHeadHeights = DetermineHeadHeightAdjustment("Plate 2", levelAdjustments);

                // get windows by floor
                List<FamilyInstance> firstFlrWindows = Utils.GetWindowsByLevel(curDoc, "First Floor");
                List<FamilyInstance> secondFlrWindows = Utils.GetWindowsByLevel(curDoc, "Second Floor");

                // count variables
                int firstFlrWinHeadAdjusted = 0;
                int firstFlrWinHeightdjusted = 0;
                int secondFlrWinHeadAdjusted = 0;
                int secondFlrWinHeightdjusted = 0;

                // process first floor windows
                if (firstFlrHeadHeights)
                {
                    using (Transaction t = new Transaction(curDoc, "Adjust First Floor Winodws"))
                    {
                        t.Start();

                        foreach (FamilyInstance curWin in firstFlrWindows)
                        {
                            if (curWin != null)
                            {
                                // create a clsWindowData object
                                clsWindowData curWinData = new clsWindowData(curWin);

                                // head height adjustment code

                                // check if adjust window heights is true
                                if (firstFlrWinHeights)
                                {
                                    // adjust window heihgts
                                }
                            }
                        }

                        // commit the transaction
                        t.Commit();
                    }

                    // notify user of adjustments made
                }

                // process second floor windows
                if (secondFlrHeadHeights)
                {
                    using (Transaction t = new Transaction(curDoc, "Adjust Second Floor Winodws"))
                    {
                        t.Start();

                        foreach (FamilyInstance curWin in firstFlrWindows)
                        {
                            if (curWin != null)
                            {
                                // create a clsWindowData object
                                clsWindowData curWinData = new clsWindowData(curWin);

                                // head height adjustment code

                                // check if adjust window heights is true
                                if (secondFlrWinHeights)
                                {
                                    // adjust window heihgts
                                }
                            }
                        }

                        // commit the transaction
                        t.Commit();
                    }
                }

                // tracking & summary report
            }

            #endregion

            #region Summary Report

            // display a summary report of the adjustments made

            #endregion

            return Result.Succeeded;
        }

        private bool DetermineHeadHeightAdjustment(string plateName, Dictionary<Level, double> levelAdjustments)
        {
            foreach (var kvp in levelAdjustments)
            {
                Level level = kvp.Key;
                double adjustment = kvp.Value;

                if (level.Name == plateName)
                {
                    // return true if positive, false if negative
                    if(adjustment > 0)
                    {
                        return true; // raise head heights
                    }
                    else if (adjustment < 0)
                    {
                        return false; // lower head heights
                    }
                }
            }

            // if plate not found notify user & return false
            Utils.TaskDialogError("Error", "Level Manager", $"Could not find level '{plateName}' in the project. Window adjustments cannot proceed.");
            return false;
        }

       

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


