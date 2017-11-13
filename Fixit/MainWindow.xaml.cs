using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using System.Windows.Controls.Primitives;

namespace Fixit {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        static System.Configuration.Configuration config =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        FixFile FixItObj = new FixFile();
        string oldPath = config.AppSettings.Settings["FixItPath"].Value;
        string newPath = config.AppSettings.Settings["AuditPath"].Value;
        string currentCellValue = "";
        int currentCellIndex = 0;
        char currentChar = 'a';
        int ctrlZCurrentIndex = 0;
        int lastCurrentCellIndex = 0;

        List<string> ctrlZNameList = new List<string>();
        List<int> ctrlZIndexList = new List<int>();


        public MainWindow() {
            InitializeComponent();
            BrushConverter converter = new BrushConverter();
            PrefixBox.Background = (Brush)converter.ConvertFrom("#f0f0f0"); 
        }

        private void NewNameListTable_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            // Handles the Ctrl+D feature
            if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.D)) {
                e.Handled = true; //prevents D from being typed
                //if last is not an int, a.k.a if it is a letter
                if (!int.TryParse(currentCellValue[currentCellValue.Length - 1].ToString(), out int output)) { 
                    FixItObj.MyFiles[currentCellIndex].NewName = currentCellValue;
                    currentChar = currentCellValue[currentCellValue.Length - 1];
                    for (int i = 1; ; i++) {
                        currentChar++;
                        if (i == 1) {
                            if(Math.Abs(currentCellIndex - lastCurrentCellIndex) > 1) {
                                ctrlZCurrentIndex = 0;
                            }
                        }

                        if (FixItObj.MyFiles[currentCellIndex + i].NewName ==
                            currentCellValue.Substring(0, currentCellValue.Length - 1) + currentChar.ToString()) {
                            continue;
                        }
                        else { //where the actual Ctrl+D action takes place
                            NewNameListTable_SelectionChanged(NewNameListTable, null);
                            ctrlZNameList.Add(currentCellValue.Substring(0, currentCellValue.Length - 1));
                            ctrlZCurrentIndex++;
                            ctrlZIndexList.Add(currentCellIndex + ctrlZCurrentIndex);

                            FixItObj.MyFiles[currentCellIndex + i].NewName = currentCellValue.Substring(0, currentCellValue.Length - 1) + currentChar.ToString();

                            NewNameListTable.Items.Refresh();
                            NewNameListTable.Focus();
                        }
                        break;
                    }
                }
            }

            else if (Keyboard.IsKeyDown(Key.Enter)) {
                ctrlZNameList.Add(currentCellValue);
                ctrlZIndexList.Add(currentCellIndex); 
            }

            // handles the Ctrl+Z undo feature
            else if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Z)) {
                if(ctrlZIndexList.Count() > 0 && ctrlZNameList.Count() > 0) {
                    string lastNameValue = ctrlZNameList[ctrlZNameList.Count() - 1];
                    int lastIndexValue = ctrlZIndexList[ctrlZIndexList.Count() - 1];
                    FixItObj.MyFiles[lastIndexValue].NewName = lastNameValue;
                    //'pop' the last values
                    ctrlZNameList.RemoveAt(ctrlZNameList.Count()-1);
                    ctrlZIndexList.RemoveAt(ctrlZIndexList.Count()-1);

                    NewNameListTable.Items.Refresh();
                    NewNameListTable.Focus();
                }         
            }
        }

        // trigger when a different cell is clicked on, or user hits
        // enter, Ctrl+D, Ctrl+Z
        private void NewNameListTable_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = NewNameListTable.SelectedItem;

            if (item != null) {          
                lastCurrentCellIndex = currentCellIndex;
                currentCellValue = (NewNameListTable.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text.ToString();
                currentCellIndex = NewNameListTable.SelectedIndex;
            }
        }

        // allows users to set the path to the fitix folder
        private void SetFixItItem_Click(object sender, RoutedEventArgs e) {        
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = config.AppSettings.Settings["FixItPath"].Value;
            fbd.ShowNewFolderButton = false;
            fbd.Description = "Select Path:";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                MessageBox.Show("Fixit Folder set successfully.", "Confirmation", MessageBoxButton.OK);
                oldPath = fbd.SelectedPath;
                config.Save(ConfigurationSaveMode.Modified);
                LoadFiles(oldPath, newPath);
            }
        }

        // allows users to set the path to the audit folder
        private void SetAuditItem_Click(object sender, RoutedEventArgs e) {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = config.AppSettings.Settings["AuditPath"].Value;
            fbd.ShowNewFolderButton = false;
            fbd.Description = "Select Path:";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                MessageBox.Show("Audit Folder set successfully.", "Confirmation", MessageBoxButton.OK);
                newPath = fbd.SelectedPath;
                config.Save(ConfigurationSaveMode.Modified);
                LoadFiles(oldPath, newPath);
            }
        }


        private void LoadFiles(string oldPath, string newPath) {
            FixItObj = new FixFile(Directory.GetFiles(oldPath), oldPath, newPath);
            ExtensionBox.Text = FixItObj.Extension;
            PrefixBox.Text = FixItObj.TN;
            CreateTable(FixItObj);
        }

       private void CreateTable(FixFile FixItObj) {
            OldNameListTable.ItemsSource = FixItObj.MyFiles;
            NewNameListTable.ItemsSource = FixItObj.MyFiles;
        }

        // enables/disables the PrefixBox where the TN is located
        private void ChangeTN_Click(object sender, RoutedEventArgs e) {
            PrefixBox.IsReadOnly = !PrefixBox.IsReadOnly;
            PrefixBox.IsHitTestVisible = !PrefixBox.IsHitTestVisible;
            if (PrefixBox.IsReadOnly) {
                //If here, then PrefixBox is disabled
                BrushConverter converter = new BrushConverter();
                PrefixBox.Background = (Brush)converter.ConvertFrom("#f0f0f0");
                if (PrefixBox.Text != FixItObj.NewTN) {
                    FixItObj.NewTN = PrefixBox.Text;
                    MessageBox.Show("TN changed successfully", "Confirmation", MessageBoxButton.OK);          
                }
            }
            else { 
                //PrefixBox is enabled
                PrefixBox.Background = Brushes.White;
            }        
        }

        private void ResetNewButton_Click(object sender, RoutedEventArgs e) {
            FixItObj.ResetNew(FixItObj);
            NewNameListTable.Items.Refresh();
        }

        private void RefreshOldButton_Click(object sender, RoutedEventArgs e) {
            LoadFiles(oldPath, newPath);
            
        }

        private void CloseItem_Click(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }

        private void ApplyFix_Click(object sender, RoutedEventArgs e) {
            try {
                Renamer.RenameFile(FixItObj);
                MessageBox.Show("Files renamed successfully.", "Confirmation", MessageBoxButton.OK);
            }
            catch {
                MessageBox.Show("Something wrong happened. Please, contact your administrator.", "Error", MessageBoxButton.OK);
            }
        }     

        public static T GetVisualChild<T>(Visual parent) where T : Visual {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++) {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null) {
                    child = GetVisualChild<T>(v);
                }
                if (child != null) {
                    break;
                }
            }
            return child;
        }

        public static DataGridRow GetSelectedRow(System.Windows.Controls.DataGrid grid) {
            return (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem);
        }

        public static DataGridRow GetRow(System.Windows.Controls.DataGrid grid, int index) {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null) {
                try {                 
                    grid.UpdateLayout();
                    grid.ScrollIntoView(grid.Items[index]);
                    row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
                } catch {}              
            }
            return row;
        }

        public static System.Windows.Controls.DataGridCell GetCell(System.Windows.Controls.DataGrid grid, DataGridRow row, int column) {
            if (row != null) {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null) {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }

                System.Windows.Controls.DataGridCell cell = (System.Windows.Controls.DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }

        public static System.Windows.Controls.DataGridCell GetCell(System.Windows.Controls.DataGrid grid, int row, int column) {
            DataGridRow rowContainer = GetRow(grid, row);
            return GetCell(grid, rowContainer, column);
        }
    }
}
