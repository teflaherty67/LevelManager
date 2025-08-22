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

            // get data for the form
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
                Utils.TaskDialogInformation("Information", "Spec Conversion", "Front Elevation view not found. Proceeding with level adjustments in current view.");
            }

            // create a counter for summary report
            int countAdjusted = 0;

            // launch the form with level data
            frmLevelManager curForm = new frmLevelManager(filteredLevels);
            curForm.Topmost = true;

            // check if user clicked OK and process the results
            if (curForm.ShowDialog() == true)
            {
                // get data from the form
                Dictionary<Level, double> levelAdjustments = curForm.LevelAdjustments;

                // validation check
                if (ValidateElevationOrder(levelAdjustments, out string violations))
                {
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
                                level.Elevation = level.Elevation + adjustment;

                                // increment the counter
                                countAdjusted++;
                            }
                        }

                        t.Commit();
                    }

                    // summary report
                    Utils.TaskDialogInformation("Information", "Level Adjustments",
                        $"{countAdjusted} level{(countAdjusted == 1 ? "" : "s")} {(countAdjusted == 1 ? "was" : "were")} adjusted.");
                }

                else
                {
                    // alert the user
                    // if adjustments cause violaitons of the level order, warn the user
                    TaskDialog tdViolations = new TaskDialog("Warning");
                    tdViolations.MainIcon = Icon.TaskDialogIconWarning;
                    tdViolations.Title = "Plate Changes";
                    tdViolations.TitleAutoPrefix = false;
                    tdViolations.MainContent = violations;
                    tdViolations.CommonButtons = TaskDialogCommonButtons.Ok & TaskDialogCommonButtons.Cancel;

                    TaskDialogResult tdVio9lationsRes = tdViolations.Show();
                }
            }

            return Result.Succeeded;
        }

        private bool ValidateElevationOrder(Dictionary<Level, double> levelAdjustments, out string violations)
        {
            violations = "";
            List<string> violationList = new List<string>();

            // Step 1: Calculate new elevations for all levels with adjustments
            Dictionary<Level, double> newElevations = new Dictionary<Level, double>();
            foreach (var kvp in levelAdjustments)
            {
                newElevations[kvp.Key] = kvp.Key.Elevation + kvp.Value;
            }

            // Step 2: Check all pairs of levels to maintain current relationships
            foreach (var level1 in levelAdjustments.Keys)
            {
                foreach (var level2 in levelAdjustments.Keys)
                {
                    if (level1 != level2)
                    {
                        // Get current relationship
                        bool level1WasLower = level1.Elevation < level2.Elevation;

                        // Get new relationship
                        bool level1WillBeLower = newElevations[level1] < newElevations[level2];

                        // Check if relationship will change (level1 was lower but will now be higher)
                        if (level1WasLower && !level1WillBeLower)
                        {
                            violationList.Add($"{level1.Name} ({FormatElevation(newElevations[level1])}) would be higher than {level2.Name} ({FormatElevation(newElevations[level2])})");
                        }
                    }
                }
            }

            // Step 3: Build violation message
            if (violationList.Count > 0)
            {
                violations = "Based on the level adjustments provided, " + string.Join(", and ", violationList) +
                            ". Do you want to proceed?";
                return false;
            }

            return true;
        }

        // Helper method to format elevation in feet and inches
        private string FormatElevation(double elevationInFeet)
        {
            int feet = (int)elevationInFeet;
            double inches = (elevationInFeet - feet) * 12;
            int wholeInches = (int)inches;
            double fractionalInches = inches - wholeInches;

            // Convert fractional inches to nearest 1/8"
            int eighths = (int)Math.Round(fractionalInches * 8);

            if (eighths == 0)
                return $"{feet}'-{wholeInches}\"";
            else
                return $"{feet}'-{wholeInches} {eighths}/8\"";
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
