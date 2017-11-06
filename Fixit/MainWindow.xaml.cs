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


        public MainWindow() {
            InitializeComponent();
            LoadFiles(oldPath, newPath);
            BrushConverter converter = new BrushConverter();
            PrefixBox.Background = (Brush)converter.ConvertFrom("#f0f0f0");
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
           
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = config.AppSettings.Settings["FixItPath"].Value;
            fbd.ShowNewFolderButton = false;
            fbd.Description = "Select Path:";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                oldPath = fbd.SelectedPath;

                config.Save(ConfigurationSaveMode.Modified);
                LoadFiles(oldPath, newPath);

            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = config.AppSettings.Settings["AuditPath"].Value;
            fbd.ShowNewFolderButton = false;
            fbd.Description = "Select Path:";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                newPath = fbd.SelectedPath;

                config.Save(ConfigurationSaveMode.Modified);
                LoadFiles(oldPath, newPath);
            }
        }


        private void LoadFiles(string oldPath, string newPath)
        {

            /*
             * Got one of the answers in Stackoverflow to help here
             * This way it will only load the files specified.
             * https://stackoverflow.com/questions/163162/can-you-call-directory-getfiles-with-multiple-filters
             * 
             */
            FixItObj = new FixFile(Directory.GetFiles(oldPath), oldPath, newPath);
            ExtensionBox.Text = FixItObj.Extension;
            PrefixBox.Text = FixItObj.Prefix;
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
            }
            else {
                PrefixBox.Background = Brushes.White;
            }
            
        }

        private void ResetNewButton_Click(object sender, RoutedEventArgs e)
        {
            LoadFiles(oldPath, newPath);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadFiles(oldPath, newPath);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ApplyFix_Click(object sender, RoutedEventArgs e)
        {
            Renamer.RenameFile(FixItObj);
        }
    }
}
