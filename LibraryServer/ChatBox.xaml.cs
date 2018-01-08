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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibraryServer
{
    /// <summary>
    /// Interaction logic for ChatBox.xaml
    /// </summary>
    public partial class ChatBox : UserControl
    {
        public ChatBox()
        {
            InitializeComponent();
        }
        public ChatBox(bool isLeft,String Text,String Nume)
        {
            InitializeComponent();
            if (isLeft)
            {
                ChipName.HorizontalAlignment = HorizontalAlignment.Left;
                TextBlockChat.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                ChipName.HorizontalAlignment = HorizontalAlignment.Right;
                TextBlockChat.HorizontalAlignment = HorizontalAlignment.Right;
            }
            TextBlockChat.Text = Text;
            ChipName.Content = Nume;
            ChipName.Icon = Nume[0];
        }
        public ChatBox(bool isLeft, String Text, String Nume, String ProfilePic)
        {
            InitializeComponent();
            if (isLeft)
            {
                ChipName.HorizontalAlignment = HorizontalAlignment.Left;
                TextBlockChat.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                ChipName.HorizontalAlignment = HorizontalAlignment.Right;
                TextBlockChat.HorizontalAlignment = HorizontalAlignment.Right;
            }
            TextBlockChat.Text = Text;
            ChipName.Content = Nume;
            Image Img = new Image();
            Img.Source=new BitmapImage(new Uri(ProfilePic, UriKind.Absolute)) ;
            ChipName.Icon = Img;
        }
    }
}
