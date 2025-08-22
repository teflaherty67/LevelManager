using LevelManager.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LevelManager
{
    /// <summary>
    /// Interaction logic for frmLevelManager.xaml
    /// </summary>
    public partial class frmLevelManager : Window
    {
        // create list to hold the levels
        private List<Level> Levels;

        // create a dictionary to hold the level-adjustment pairs
        private Dictionary<Level, TextBox> Level_TextBoxes = new Dictionary<Level, TextBox>();

        // create public property for Dictionary
        public Dictionary<Level, double> LevelAdjustments { get; private set; }

        public frmLevelManager(List<Level> levels)
        {
            // store passed data
            Levels = levels;

            // intialize the XAML controls
            InitializeComponent();

            // generate level controls
            GenerateLevelControls();
        }

        #region Dynamic Controls

        private void GenerateLevelControls()
        {
            // loop through the levels and create a control for each level
            foreach (Level curLevel in Levels)
            {
                // create a grid
                Grid levelGrid = new Grid() { Margin = new Thickness(0, 2, 0, 2) }; // Add some vertical spacing

                levelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // level name
                levelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // TextBox for adjustment

                // create & position the label
                Label lblLevel = new Label() { Content = curLevel.Name };
                Grid.SetColumn(lblLevel, 0);

                // create & postion the text box
                TextBox txbLevel = new TextBox();
                Grid.SetColumn(txbLevel, 1);

                // store the TextBox reference
                Level_TextBoxes[curLevel] = txbLevel;

                // add both controls to the grid
                levelGrid.Children.Add(lblLevel);
                levelGrid.Children.Add(txbLevel);

                // grid to the stack panel
                sp.Children.Add(levelGrid);
            }
        }

        #endregion

        #region Buttons Section

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            LevelAdjustments = LevelAdjustmentData();
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // launch the help site with user's default browser
                string helpUrl = "https://lifestyle-usa-design.atlassian.net/wiki/spaces/MFS/pages/472711169/Spec+Level+Conversion?atlOrigin=eyJpIjoiMmU4MzM3NzFmY2NlNDdiNjk1MjY2M2MyYzZkMjY2YWQiLCJwIjoiYyJ9";
                Process.Start(new ProcessStartInfo
                {
                    FileName = helpUrl,
                    UseShellExecute = true
                });

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An error occurred while trying to display help: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        private Dictionary<Level, double> LevelAdjustmentData()
        {
            var levelAdjustements = new Dictionary<Level, double>();

            foreach (var kvp in Level_TextBoxes)
            {
                Level level = kvp.Key;
                TextBox textBox = kvp.Value;

                if (double.TryParse(textBox.Text, out double adjustment))
                {
                    levelAdjustements[level] = Utils.ConvertINToFT(adjustment);
                }
                else
                {
                    levelAdjustements[level] = 0; // default to no adjustement for invalid input
                }
            }

            return levelAdjustements;
        }
    }
}
