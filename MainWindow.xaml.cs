using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.XMLParser;

namespace XMLViewer
{
    public class WindowProperties
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public WindowState WindowState { get; set; }
        public GridLength LeftTreeWidth { get; set; }
        public GridLength TopContentHeight { get; set; }
        public GridLength BottomContentHeight { get; set; }
    }
    public class XMLData
    {
        public string XMLString { get; set; }
        public XMLObject XMLObject { get; set; }
        public XMLData(string XmlString)
        {
            XMLString = XmlString;
            XMLObject = XMLParser.Parser(XmlString);
        }
        public XMLData(XmlDocument document)
        {
            XMLString = document.InnerXml;
            XMLObject = XMLParser.Parser(document);
        }
    }
    public class SelectItem
    {
        public string Content { get; set; }
        public List<XMLAttribute> Attributes { get; set; }
    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        public string SettingFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            + "\\" + Properties.Settings.Default.AppName + ".dat";

        public struct Position{ public double Top, Left, Height, Width; }
        public Position position;
        public XMLData XmlData;
        private int GetStateCode(WindowState state)
        {
            switch (state)
            {
                case WindowState.Normal:return 0;
                case WindowState.Minimized:return 1;
                case WindowState.Maximized:return 2;
                default:return -1;
            }
        }
        private WindowState FromStateCode(int code)
        {
            switch (code)
            {
                case 0: return WindowState.Normal;
                case 1: return WindowState.Minimized;
                case 2: return WindowState.Maximized;
                default: return 0;
            }
        }
        public MainWindow()
        {
            WindowProperties properties;
            if (File.Exists(SettingFile))
            {
                FileStream fs = File.OpenRead(SettingFile);
                BinaryReader br = new BinaryReader(fs);
                properties = new WindowProperties()
                {
                    Height = br.ReadDouble(),
                    Width = br.ReadDouble(),
                    Left = br.ReadDouble(),
                    Top = br.ReadDouble(),
                    WindowState = FromStateCode(br.ReadInt32()),
                    LeftTreeWidth = new GridLength(br.ReadDouble()),
                    TopContentHeight = new GridLength(br.ReadDouble(), GridUnitType.Star),
                    BottomContentHeight = new GridLength(br.ReadDouble(), GridUnitType.Star),
                };
                br.Close();
                fs.Close();
            }
            else
            {
                properties = new WindowProperties()
                {
                    Height = 450,
                    Width = 600,
                    Left = (SystemParameters.WorkArea.Width - 600) / 2,
                    Top = (SystemParameters.WorkArea.Height - 450) / 2,
                    WindowState = WindowState.Normal,
                    LeftTreeWidth = new GridLength(150),
                    TopContentHeight = new GridLength(1, GridUnitType.Star),
                    BottomContentHeight = new GridLength(1, GridUnitType.Star)
                };
            }
            DataContext = properties;
            position = new Position
            {
                Height = properties.Height,
                Width = properties.Width,
                Left = properties.Left,
                Top = properties.Top
            };
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FileStream fs = File.Create(SettingFile);
            BinaryWriter br = new BinaryWriter(fs);
            br.Write(Height);
            br.Write(Width);
            br.Write(Left);
            br.Write(Top);
            br.Write(GetStateCode(this.WindowState));
            br.Write(LeftTree.ActualWidth);
            br.Write(TopContent.Height.Value);
            br.Write(BottomContent.Height.Value);
            br.Close();
            fs.Close();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                position.Width = this.Width;
                position.Height = this.Height;
                position.Left = this.Left;
                position.Top = this.Top;
            }
        }

        private void MainTree_Selected(object sender, RoutedEventArgs e)
        {
            XMLObject xmlObject = (XMLObject)MainTree.SelectedItem;
            ContentText.Text = xmlObject.InnerXml;
            MainList.ItemsSource= xmlObject.Attributes;
        }

        private void Menu_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML 文件|*.xml";
            ofd.DefaultExt = ".xml";
            if (ofd.ShowDialog() == true)
            {
                StreamReader sr = new StreamReader(ofd.FileName);
                string XMLString = sr.ReadToEnd();
                sr.Close();
                try 
                { 
                    XmlData = new XMLData(XMLString);
                    MainText.DataContext = XmlData;
                    MainTree.ItemsSource = new List<XMLObject> { XmlData.XMLObject };
                }
                catch(Exception err)
                {
                    MessageBox.Show(err.Message, "错误", MessageBoxButton.OK);
                }
            }
        }

        private void Menu_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
