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


    }
}
