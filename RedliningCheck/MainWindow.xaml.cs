using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

//Windows 10 Notification
using NotificationsExtensions.Toasts;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace RedliningCheck
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ToastNotify();
        }

        private void ToastNotify()
        {
            string title = "asd Asker has changed his profile picture";
            string content = "Das ist der Content asdf kdsafjds kfjsadfjsd fksdjfa";
            string logo = "Assets/StoreLogo.png";
           // string image = "Assets/StoreLogo.png";

            ToastVisual visual = new ToastVisual()
            {
                TitleText = new ToastText() { Text = title },
                BodyTextLine1 = new ToastText() { Text = content },
                AppLogoOverride = new ToastAppLogo() { Source = new ToastImageSource(logo), Crop = ToastImageCrop.Circle }
                //    AddImageQuery = new ToastImage() { Source = new ToastImageSource(image) } 
            };

            ToastActionsCustom action = new ToastActionsCustom
            {
                Inputs = { new ToastTextBox("txt") { PlaceholderContent = "Write a comment here .." } },
                //      Buttons = { new ToastButton("Reply", new QueryString() { "action", "Reply" }.ToString()) }
            };

            ToastContent Content = new ToastContent() { Visual = visual, Actions = action };

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Content.GetContent());
            ToastNotification notification = new ToastNotification(xmlDoc);
            ToastNotificationManager.CreateToastNotifier().Show(notification);
        }


        private void ListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                listBox.Items.Clear();
                // Note that you can have more than one file.
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                Debug.WriteLine("Got Files");
                foreach (var file in files)
                {
                    if (System.IO.Directory.Exists(file))
                    {
                        FileSearch(file);
                        DirSearch(file);
                    } else if (System.IO.File.Exists(file))
                    {
                        Debug.WriteLine($"  File={file}");
                        CheckRevision(file);
                    }
                    else
                    {
                        Debug.Write("Folder/File not exist!");
                    }
                }
            }
        }

        void DirSearch(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        Debug.WriteLine($"  File={f}");
                        CheckRevision(f);
                    }
                    DirSearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        void FileSearch(string sFile)
        {
            foreach (string f in Directory.GetFiles(sFile))
            {
                Debug.WriteLine($"  File={f}");
                CheckRevision(f);
            }
        }

        void CheckRevision(string file)
        {
            //Kontrolle Erstellungsdatum mit Änderungsdatum kontrollieren
            string fileCreatedDate = File.GetCreationTime(file).ToString("yyyy - MM - dd HH: mm:ss");
            string fileEditDate = File.GetLastWriteTime(file).ToString("yyyy - MM - dd HH: mm:ss");

            if (string.Compare(fileCreatedDate,fileEditDate) < 0)
            {
                if (System.IO.Path.GetExtension(file) == ".pdf")
                {
                    Debug.WriteLine(fileCreatedDate + " < " + fileEditDate);
                    listBox.Items.Add(file);
                }
            }
        }

        private void listbox_MouseDoubleClick(object sender, EventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                System.Diagnostics.Process.Start(listBox.SelectedItem.ToString()); 
            }
        }
    }
}
