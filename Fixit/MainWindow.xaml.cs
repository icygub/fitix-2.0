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
using System.Windows.Controls;
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


        public MainWindow() {
            InitializeComponent();
            BrushConverter converter = new BrushConverter();
            PrefixBox.Background = (Brush)converter.ConvertFrom("#f0f0f0"); 
        }

        private void SetFixItItem_Click(object sender, RoutedEventArgs e)
        {
           
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = config.AppSettings.Settings["FixItPath"].Value;
            fbd.ShowNewFolderButton = false;
            fbd.Description = "Select Path:";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("Fixit Folder set successfully.", "Confirmation", MessageBoxButton.OK);
                oldPath = fbd.SelectedPath;
                config.Save(ConfigurationSaveMode.Modified);
                LoadFiles(oldPath, newPath);

            }
        }

        private void SetAuditItem_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = config.AppSettings.Settings["AuditPath"].Value;
            fbd.ShowNewFolderButton = false;
            fbd.Description = "Select Path:";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("Audit Folder set successfully.", "Confirmation", MessageBoxButton.OK);
                newPath = fbd.SelectedPath;
                config.Save(ConfigurationSaveMode.Modified);
                LoadFiles(oldPath, newPath);
            }
        }


        private void LoadFiles(string oldPath, string newPath)
        {
            FixItObj = new FixFile(Directory.GetFiles(oldPath), oldPath, newPath);
            ExtensionBox.Text = FixItObj.Extension;
            PrefixBox.Text = FixItObj.TN;
            CreateTable(FixItObj);
        }

       private void CreateTable(FixFile FixItObj)
       {
           OldNameListTable.ItemsSource = FixItObj.MyFiles;
           NewNameListTable.ItemsSource = FixItObj.MyFiles;
       }


        private void ChangeTN_Click(object sender, RoutedEventArgs e)
        {
            PrefixBox.IsReadOnly = !PrefixBox.IsReadOnly;
            PrefixBox.IsHitTestVisible = !PrefixBox.IsHitTestVisible;
            if(PrefixBox.IsReadOnly) {
                BrushConverter converter = new BrushConverter();
                PrefixBox.Background = (Brush)converter.ConvertFrom("#f0f0f0");
                if (PrefixBox.Text != FixItObj.NewTN)
                {
                    FixItObj.NewTN = PrefixBox.Text;
                    MessageBox.Show("TN changed successfully", "Confirmation", MessageBoxButton.OK);
                }
            }
            else {
                PrefixBox.Background = Brushes.White;

            }
            
        }

        private void ResetNewButton_Click(object sender, RoutedEventArgs e)
        {
            FixItObj.ResetNew(FixItObj);
            NewNameListTable.Items.Refresh();
        }

        private void RefreshOldButton_Click(object sender, RoutedEventArgs e)
        {
            LoadFiles(oldPath, newPath);
        }

        private void CloseItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ApplyFix_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Renamer.RenameFile(FixItObj);
                MessageBox.Show("Files renamed successfully.", "Confirmation", MessageBoxButton.OK);
            }
            catch
            {
                MessageBox.Show("Something wrong happened. Please, contact your administrator.", "Error", MessageBoxButton.OK);
            }

        }

        private void NewNameListTable_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.D)) {
                //MessageBox.Show("Ctrl+D!!!!!");
                e.Handled = true; //prevents D from being typed
                if (! int.TryParse(currentCellValue[currentCellValue.Length - 1].ToString(), out int output)) { //if last is not an int
                    //MessageBox.Show("we are here");
                    FixItObj.MyFiles[currentCellIndex].NewName = currentCellValue;

                    currentChar = currentCellValue[currentCellValue.Length - 1];
                    currentChar++;
                    FixItObj.MyFiles[currentCellIndex + 1].NewName = currentCellValue.Substring(0,currentCellValue.Length - 1) + currentChar.ToString();

                    //NewNameListTable.Items.Refresh();
                    //NewNameListTable.Focus();

                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                }
                //FixItObj.MyFiles[currentCellIndex].NewName = currentCellValue;
            }

            
        }

        private void NewNameListTable_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = NewNameListTable.SelectedItem;
            currentCellValue = (NewNameListTable.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text.ToString();
            currentCellIndex = NewNameListTable.SelectedIndex;

            //var row = NewNameListTable.ItemContainerGenerator.ContainerFromIndex(currentCellIndex); //datagridrow

           

            //GetCell(NewNameListTable, currentCellIndex, 0).Content = "TEST";
            

            //NewNameListTable.Items[index]
            //MessageBox.Show(value);


            //var dataGrid = sender as System.Windows.Controls.DataGrid;
            //if(dataGrid == null) {
            //    return;
            //}
            //var index = dataGrid.SelectedIndex;

            //System.Windows.Controls.DataGridCell cell = dataGrid.ItemContainerGenerator.ContainerFromIndex(index) as System.Windows.Controls.DataGridCell;

            //if(cell == null) {
            //    MessageBox.Show((string)cell.Content);
            //}
            ////DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
            ////var item = dataGrid.ItemContainerGenerator.ItemFromContainer(row);
            ////MessageBox.Show(item);
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
                } catch {

                }
                
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
